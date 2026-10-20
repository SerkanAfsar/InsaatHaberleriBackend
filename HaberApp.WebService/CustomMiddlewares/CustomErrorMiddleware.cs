using HaberApp.Core.DTOs.Base;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Utils;

namespace HaberApp.WebService.CustomMiddlewares
{
    public class CustomErrorMiddleware
    {
        private readonly RequestDelegate next;

        public CustomErrorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                var responseResult = new ResponseResult<BaseResponseDto>
                {
                    Success = false,
                    Errors = new()
                };

                if (ex is CustomException customError)
                {
                    responseResult.Errors = customError.Errors;
                    responseResult.StatusCode = customError.StatusCode;

                }
                else
                {
                    responseResult.Errors.Add("error", ex.Message);
                    responseResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                }

                context.Response.StatusCode = (int)responseResult.StatusCode;
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsJsonAsync(responseResult);
                return;
            }
        }
    }
}

