namespace WavesBot.IAL
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using ViewLib.Services;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILoggerService loggerService;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerService loggerService)
        {
            this.next = next;
            this.loggerService = loggerService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            catch (Exception exception)
            {
                await loggerService.LogExceptionAsync(exception).ConfigureAwait(false);
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.OK;

            var result = JsonConvert.SerializeObject(new { exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}