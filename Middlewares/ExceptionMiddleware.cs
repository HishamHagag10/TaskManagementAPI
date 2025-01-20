using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using System.Text.Json;
using TaskManagementAPI.Erorrs;

namespace TaskManagementAPI.MeddleWare
{
    public class ExceptionMiddleware
    {
        public ExceptionMiddleware(RequestDelegate next,IWebHostEnvironment inv) 
        {
            _next = next;
            _inv = inv;
        }

        public RequestDelegate _next { get; }
        public IWebHostEnvironment _inv { get; }
        public async Task InvokeAsync(HttpContext content)
        {
            try
            {
                await _next(content);

            }catch (Exception ex) 
            {
                await HandleExceptionAsync(content,ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var response=_inv.IsDevelopment() ?
                new ApiErrorResponse(context.Response.StatusCode,exception.Message)
                : new ApiErrorResponse(context.Response.StatusCode)
                ;

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);

            return;
        }
    }
}
