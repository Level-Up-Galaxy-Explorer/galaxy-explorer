
resource "aws_ecs_cluster" "ge_ecs_cluster" {
  name = "ge-ecs-cluster"
}

resource "aws_ecs_task_definition" "ge_ecs_task" {
  family                   = "ge-app"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "256"
  memory                   = "512"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn

  container_definitions = jsonencode([{
    "name" : "ge-app-container",
    "image" : "${var.aws_account_id}.dkr.ecr.af-south-1.amazonaws.com/ge-app-repository:latest",
    "essential" : true,
    "portMappings" : [
      {
        "containerPort" : 80,
        "hostPort" : 80
      }
    ],
    "secrets" : [
      {
        "name" : "ConnectionStrings__DefaultConnection",
        "valueFrom" : "arn:aws:ssm:${var.default_region}:${var.aws_account_id}:parameter/geapp-connection-string"
      }
    ]
  }])
}

resource "aws_lb" "ge_app_alb" {
  name                             = "ge-ecs-alb"
  internal                         = false
  load_balancer_type               = "application"
  security_groups                  = [aws_security_group.ecs_sg.id]
  subnets                          = [aws_subnet.subnet_a.id, aws_subnet.subnet_b.id]
  enable_deletion_protection       = false
  enable_cross_zone_load_balancing = true
  drop_invalid_header_fields       = true
}

resource "aws_lb_target_group" "ge_app_alb_target_group" {
  name        = "ge-elb-target-group"
  port        = 80
  protocol    = "HTTP"
  vpc_id      = aws_vpc.main.id
  target_type = "ip"

  health_check {
    interval            = 30
    path                = "/health"
    port                = "80"
    protocol            = "HTTP"
    timeout             = 5
    unhealthy_threshold = 2
    healthy_threshold   = 2
  }
}

resource "aws_lb_listener" "http_listener" {
  load_balancer_arn = aws_lb.ge_app_alb.arn
  port              = "80"
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.ge_app_alb_target_group.arn
  }
}

resource "aws_cloudwatch_metric_alarm" "alb_requests_high_alarm" {
  alarm_name          = "alb-high-request-count"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "RequestCountPerTarget"
  namespace           = "AWS/ApplicationELB"
  period              = "60"
  statistic           = "Average"
  threshold           = "500"
  alarm_description   = "Triggers when ALB request count per target is high"
  dimensions = {
    LoadBalancerArn = aws_lb.ge_app_alb.arn
    TargetGroupArn  = aws_lb_target_group.ge_app_alb_target_group.arn
  }

  alarm_actions = ["${aws_appautoscaling_policy.scale_up_requests_policy.arn}"]
}

resource "aws_cloudwatch_metric_alarm" "alb_requests_low_alarm" {
  alarm_name          = "alb-low-request-count"
  comparison_operator = "LessThanThreshold"
  evaluation_periods  = "5"
  metric_name         = "RequestCountPerTarget"
  namespace           = "AWS/ApplicationELB"
  period              = "60"
  statistic           = "Average"
  threshold           = "100"
  alarm_description   = "Triggers when ALB request count per target is low"
  dimensions = {
    LoadBalancerArn = aws_lb.ge_app_alb.arn
    TargetGroupArn  = aws_lb_target_group.ge_app_alb_target_group.arn
  }
  alarm_actions = ["${aws_appautoscaling_policy.scale_down_requests_policy.arn}"]
}

resource "aws_ecs_service" "ge_app_service" {
  name            = "ge-app-service"
  cluster         = aws_ecs_cluster.ge_ecs_cluster.id
  task_definition = aws_ecs_task_definition.ge_ecs_task.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = [aws_subnet.subnet_a.id, aws_subnet.subnet_b.id]
    security_groups  = [aws_security_group.ecs_sg.id]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.ge_app_alb_target_group.arn
    container_name   = "ge-app-container"
    container_port   = 80
  }

  depends_on = [aws_lb.ge_app_alb]
}

resource "aws_appautoscaling_target" "ecs_service_scaling" {
  max_capacity       = 5
  min_capacity       = 1
  resource_id        = "service/${aws_ecs_cluster.ge_ecs_cluster.name}/${aws_ecs_service.ge_app_service.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"

  depends_on = [aws_ecs_service.ge_app_service]
}

resource "aws_appautoscaling_policy" "scale_up_requests_policy" {
  name               = "scale-up-requests-policy"
  policy_type        = "StepScaling"
  resource_id        = "service/${aws_ecs_cluster.ge_ecs_cluster.name}/${aws_ecs_service.ge_app_service.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"

  step_scaling_policy_configuration {
    adjustment_type = "ChangeInCapacity"
    step_adjustment {
      metric_interval_lower_bound = "0"
      scaling_adjustment          = 1
    }
    cooldown                = 60
    metric_aggregation_type = "Average"
  }

  depends_on = [aws_appautoscaling_target.ecs_service_scaling]
}

resource "aws_appautoscaling_policy" "scale_down_requests_policy" {
  name               = "scale-down-requests-policy"
  policy_type        = "StepScaling"
  resource_id        = "service/${aws_ecs_cluster.ge_ecs_cluster.name}/${aws_ecs_service.ge_app_service.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"

  step_scaling_policy_configuration {
    adjustment_type = "ChangeInCapacity"
    step_adjustment {
      metric_interval_lower_bound = "0"
      scaling_adjustment          = -1
    }
    cooldown                = 300
    metric_aggregation_type = "Average"
  }

  depends_on = [aws_appautoscaling_target.ecs_service_scaling]
}

resource "aws_iam_role" "ecs_task_execution_role" {
  name = "ecs-task-execution-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      },
    ]
  })
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution_policy_ecr" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
  role       = aws_iam_role.ecs_task_execution_role.name
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution_policy_cloudwatch" {
  policy_arn = "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess"
  role       = aws_iam_role.ecs_task_execution_role.name
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution_policy_s3" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonS3ReadOnlyAccess"
  role       = aws_iam_role.ecs_task_execution_role.name
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution_policy_ssm" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMReadOnlyAccess"
  role       = aws_iam_role.ecs_task_execution_role.name
}
