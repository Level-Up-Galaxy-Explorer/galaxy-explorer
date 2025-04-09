variable "vpc_cidr" {
  default = "10.0.0.0/16"
}

variable "subnet_cidr_a" {
  description = "CIDR block for subnet A"
  default     = "10.0.1.0/24"
}

variable "subnet_cidr_b" {
  description = "CIDR block for subnet B"
  default     = "10.0.2.0/24"
}

variable "default_region" {
  description = "Default region"
  default     = "af-south-1"
}

variable "subnet_availability_zone_a" {
  description = "Availability zone for subnet A"
  default     = "af-south-1a"
}

variable "subnet_availability_zone_b" {
  description = "Availability zone for subnet B"
  default     = "af-south-1b"
}

variable "rds_db_username" {
  description = "Username for the PostgreSQL database"
  default     = "galaxy_explorer"
}

variable "rds_db_password" {
  description = "Password for the PostgreSQL database"
  sensitive   = true
}

variable "rds_db_name" {
  description = "Name of the PostgreSQL database"
  default     = "gedatabase"
}

variable "aws_account_id" {
  description = "The AWS Account ID"
  type        = string
  sensitive   = true
}
