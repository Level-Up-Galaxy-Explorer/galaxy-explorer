
using ErrorOr;

namespace galaxy_api.Errors;

public static class DomainErrros
{

    public static class Database
    {
        public static Error Unexpected(string message, string? code = null) => Error.Unexpected(
                code: code ?? "Database.UnexpectedError",
                description: $"An unexpected database error occurred: {message}");

        public static Error ConnectionFailed(string message) => Error.Failure(
            code: "Database.ConnectionFailed",
            description: $"Failed to connect to the database: {message}");

        public static Error ConstraintViolation(string constraintName, string? details = null) => Error.Conflict(
           code: $"Database.Constraint.{constraintName}",
           description: $"A database constraint '{constraintName}' was violated.{(details != null ? $" Details: {details}" : "")}");
    }

    public static class Validation
    {
        public static Error Required(string fieldName) => Error.Validation(
            code: $"{fieldName}.Required",
            description: $"The field '{fieldName}' is required.");

        public static Error MaxLength(string fieldName, int maxLength) => Error.Validation(
            code: $"{fieldName}.MaxLength",
            description: $"The field '{fieldName}' must not exceed {maxLength} characters.");

        public static Error InvalidFormat(string fieldName, string expectedFormat) => Error.Validation(
            code: $"{fieldName}.InvalidFormat",
            description: $"The field '{fieldName}' is not in the expected format ({expectedFormat}).");

        public static Error General(string description, string? codeSuffix = null) => Error.Validation(
             code: $"Validation.General{(codeSuffix != null ? $".{codeSuffix}" : "")}",
             description: description);
    }

    public static class NotFound
    {
        public static Error Resource(string resourceName, object identifier) => Error.NotFound(
            code: $"{resourceName}.NotFound",
            description: $"{resourceName} with identifier [{identifier}] was not found.");
    }

    public static class Conflict
    {
        public static Error Duplicate(string resourceName, string fieldName, object fieldValue) => Error.Conflict(
           code: $"{resourceName}.Duplicate.{fieldName}",
           description: $"A {resourceName.ToLower()} with the {fieldName.ToLower()} '{fieldValue}' already exists.");

        public static Error General(string description, string? codeSuffix = null) => Error.Conflict(
            code: $"Conflict.General{(codeSuffix != null ? $".{codeSuffix}" : "")}",
            description: description);
    }

    public static class Authentication
    {
        public static Error Unauthorized(string? description = null) => Error.Unauthorized(
            code: "Auth.Unauthorized",
            description: description ?? "Authentication failed or credentials required.");

        public static Error Forbidden(string? description = null) => Error.Forbidden(
            code: "Auth.Forbidden",
            description: description ?? "Access to this resource is forbidden.");
    }

    public static class General
    {
        public static Error Unexpected(string? description = null) => Error.Unexpected(
           code: "General.Unexpected",
           description: description ?? "An unexpected error occurred.");
    }

}