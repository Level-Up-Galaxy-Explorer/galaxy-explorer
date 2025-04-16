using System.Net;

namespace galaxy_cli.Commands.Base;

public class ApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ReasonPhrase { get; }
    public Uri? RequestUrl { get; }

    public ApiException(string message, Uri? requestUrl = null, Exception? innerException = null)
           : base(message, innerException)
    {
        RequestUrl = requestUrl;
    }

    protected ApiException(string message, HttpResponseMessage response, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = response.StatusCode;
        ReasonPhrase = response.ReasonPhrase;
        RequestUrl = response.RequestMessage?.RequestUri;
    }

    public static ApiException CreateFromHttpResponse(HttpResponseMessage response, string? messagePrefix = null)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));

        Uri? requestUrl = response.RequestMessage?.RequestUri;
        string prefix = string.IsNullOrWhiteSpace(messagePrefix)
            ? $"API request failed"
            : messagePrefix;


        HttpStatusCode statusCode = response.StatusCode;
        string reason = response.ReasonPhrase ?? "Unknown Reason";
        

        string message = $"{prefix}: Status code {(int)statusCode} ({reason}) received from {requestUrl ?? null}";

        ApiException exception;

        if ((int)statusCode >= 500)
            exception = new ApiServerException(message, response);
        else if (statusCode == HttpStatusCode.NotFound)
            exception = new ApiNotFoundException(message, response);
        else if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.Forbidden)
            exception = new ApiAuthorizationException(message, response);
        else
            exception = new ApiClientException(message, response);

        return exception;
    }


}
public class ApiNetworkException : ApiException
{
    public ApiNetworkException(string message, Uri? requestUrl, HttpRequestException innerException)
        : base(message, requestUrl, innerException) { }
}

public class ApiClientException : ApiException
{
    protected internal ApiClientException(string message, HttpResponseMessage response, Exception? innerException = null)
        : base(message, response, innerException) { }
}
public class ApiNotFoundException : ApiClientException
{
    protected internal ApiNotFoundException(string message, HttpResponseMessage response)
         : base(message, response) { }
}
public class ApiAuthorizationException : ApiClientException
{
    protected internal ApiAuthorizationException(string message, HttpResponseMessage response)
       : base(message, response) { }
}

public class ApiServerException : ApiException
{
    protected internal ApiServerException(string message, HttpResponseMessage response, Exception? innerException = null)
       : base(message, response, innerException) { }
}

public class ApiDeserializationException : ApiException
{
    public ApiDeserializationException(string message, Uri? requestUrl, Exception innerException)
        : base(message, requestUrl, innerException) { }
}