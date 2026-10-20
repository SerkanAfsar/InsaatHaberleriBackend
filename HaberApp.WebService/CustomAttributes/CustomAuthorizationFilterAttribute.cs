using HaberApp.Core.Domains;
using HaberApp.Core.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace HaberApp.WebService.CustomAttributes
{
    public class CustomAuthorizationFilterAttribute<T> : Attribute, IAuthorizationFilter
        where T : BaseEntity
    {

        private readonly bool rootAdminRequired = false;
        private readonly ResponseResult<string> responseResult = new()
        {
            Success = false,
            Errors = new()
            {
                {"error","UnAuthorized" }
            },
            StatusCode = HttpStatusCode.Unauthorized,
        };
        public CustomAuthorizationFilterAttribute(bool isRootAdmin)
        {
            this.rootAdminRequired = isRootAdmin;

        }
        public CustomAuthorizationFilterAttribute() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var method = context.HttpContext.Request.Method;
            if (method == "GET")
            {
                return;
            }
            //throw new NotImplementedException();
            var user = context.HttpContext.User;
            var userInRole = user.IsInRole(RoleConstants.RootAdmin);

            //if (user == null || !user.Identity.IsAuthenticated)
            //{
            //    context.Result = new ObjectResult(this.responseResult)
            //    {
            //        StatusCode = (int)HttpStatusCode.Unauthorized
            //    };
            //    return;
            //}


            //if (rootAdminRequired && !userInRole)
            //{
            //    context.Result = new ObjectResult(this.responseResult)
            //    {
            //        StatusCode = (int)HttpStatusCode.Unauthorized
            //    };
            //    return;
            //}


            //var requiredPermission = method switch
            //{
            //    "POST" => Permissions<T>.Create,
            //    "PUT" => Permissions<T>.Update,
            //    "DELETE" => Permissions<T>.Delete,
            //    "GET" => Permissions<T>.Read,
            //    _ => null
            //};

            //var isEnabled = requiredPermission is null ||
            //    user.HasClaim("permission", requiredPermission!);


            //if (!isEnabled)
            //{
            //    this.responseResult?.Errors?.Clear();
            //    this.responseResult?.Errors?.Add("error", "Forbidden");
            //    this.responseResult?.StatusCode = HttpStatusCode.Forbidden;
            //    context.Result = new ObjectResult(this.responseResult)
            //    {
            //        StatusCode = (int)HttpStatusCode.Forbidden,
            //    };
            //    return;
            //}
            return;
        }
    }
}
