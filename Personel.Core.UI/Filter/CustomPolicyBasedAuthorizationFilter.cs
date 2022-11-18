using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Personel.Core.UI.Filter
{
    public class PolicyBasedAuthorizeAttribute : TypeFilterAttribute
    {
        public PolicyBasedAuthorizeAttribute(string policy) : base(typeof(PolicyBasedAuthorize))
        {
            Arguments = new object[] { policy };
        }
    };

    public class PolicyBasedAuthorize : IAuthorizationFilter
    {
        readonly string policy;

        readonly IAuthorizationService authorizationService;

        public PolicyBasedAuthorize(string policy,
            IAuthorizationService authorizationService)
        {
            this.policy = policy;
            this.authorizationService = authorizationService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Check active user policy.
            var authResult = authorizationService.AuthorizeAsync(context.HttpContext.User, policy);

            if (authResult.Result.Succeeded)
            {
                return;
            }

            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            string returnType = controllerActionDescriptor.MethodInfo.ReturnType.Name;

            if (returnType == nameof(JsonResult))
            {
                context.Result = new JsonResult("TEST"/*BaseResult.Create(false, Messages.NotAuthorizedTransaction)*/);
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
