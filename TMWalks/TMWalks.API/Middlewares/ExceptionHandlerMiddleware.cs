using System.Net;

namespace TMWalks.API;

public class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> logger;
    private readonly RequestDelegate next;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        this.logger = logger;
        // RequestDelegate は Requestプロセスの完了を表すタスクを返す。これを使って HTTP Request を処理することができる。
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // httpContext 呼び出し中に何かあったら例外をハンドルして、ログに書く。
            await next(httpContext);
        }
        catch (System.Exception ex)
        {
            var errorId = Guid.NewGuid();

            // Log This Exception
            logger.LogError(ex, $"{errorId} : {ex.Message}");

            // Return A Custom Error Reponse
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var error =new 
            {
                Id = errorId,
                ErrorMessage = "Something went wrong! We are looking into resolving this.",
            };

            await httpContext.Response.WriteAsJsonAsync(error);
        }
    }
}
