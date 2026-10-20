using FluentValidation;
using HaberApp.Core.DTOs.Base;
using HaberApp.Core.Extensions;
using HaberApp.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace HaberApp.WebService.CustomAttributes
{
    public class CustomFilterAttribute<TRequestDto, TResponseDto> : IAsyncActionFilter
        where TRequestDto : BaseRequestDto
        where TResponseDto : BaseResponseDto
    {
        private readonly ResponseResult<TResponseDto> responseResult;
        private readonly IValidator<TRequestDto> validator;

        public CustomFilterAttribute(IValidator<TRequestDto> validator)
        {

            this.responseResult = new ResponseResult<TResponseDto>()
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Errors = new()
            };

            this.validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var model = context.ActionArguments.Values.OfType<TRequestDto>().FirstOrDefault();

            if (model != null)
            {
                var result = await this.validator.ValidateAsync(model);

                if (result.IsValid)
                {
                    await next();
                }

                this.responseResult.Success = false;
                this.responseResult.StatusCode = System.Net.HttpStatusCode.BadRequest;

                result.Errors.ToList().ForEach(item =>
                {
                    this.responseResult.Errors.Add(StringExtensions.ToCamelCase(item.PropertyName), item.ErrorMessage);
                });
                context.Result = new ObjectResult(this.responseResult)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                return;
            }


            await next();
        }
    }
}
