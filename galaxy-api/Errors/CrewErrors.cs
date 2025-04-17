using ErrorOr;

namespace galaxy_api.Errors;

public static class CrewErrors
{

    public static Error NotAvailable => Error.Validation(
            code: "Crew.NotAvailable",
            description: "The specified crew is not currently available for assignment.");

    public static Error NotFound => Error.NotFound(
   code: "Crew.NotFound",
   description: "The requested crew was not found.");

    public static Error CreationFailed => Error.Failure(
           code: "Crew.CreationFailed",
           description: "Failed to create the crew record in the database.");

    public static Error MemberAssignmentFailed => Error.Validation(
            code: "Crew.MemberAssignmentFailed",
            description: "Failed to assign one or more members. Verify user IDs are valid.");

    public static Error NameConflict(string name, string? constraintName = null) => DomainErrros.Conflict.Duplicate("Crew", "Name", name);

    public static Error UserAlreadyAssigned(int userId, string? constraintName = null) =>
    DomainErrros.Conflict.General(
        $"User with ID [{userId}] is already assigned to a crew (violates unique constraint: {constraintName ?? "N/A"}).", "UserAssignment");

    public static Error InvalidUserId(int userId, string? constraintName = null) => Error.Validation(
        code: "Crew.InvalidUserId",
        description: $"Invalid User ID [{userId}] provided for crew assignment.");

    internal static ErrorOr<Success> OnActiveMission(int crewId) => Error.Validation(
        code: "Crew.InvalidUserId",
        description: $"Invalid User ID [{crewId}] provided for crew assignment.");
}