using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using webapi.Core;
using webapi.Models;

namespace webapi.Attributes
{
    [Flags]
    public enum AuthorizationType
    {
        None = 0,
        Customer = 1,
        Supplier = 2,
        Admin = 3,

        CustomerAndSupplier = Customer | Supplier,
    };

    [Flags]
    public enum AuthorizationRole
    {
        None = 0,
        Customer = 1,
        Supplier = 2,
        Admin = 3,

    }

    public class AuthorizationAttribute : ActionableAttribute
    {
        public AuthorizationType authorizationType = AuthorizationType.None;
        private AuthorizationRole authorizationRole = AuthorizationRole.None;

        public AuthorizationAttribute(AuthorizationType authorizationType, AuthorizationRole authorizationRole = AuthorizationRole.None)
        {
            this.authorizationType = authorizationType;
            this.authorizationRole = authorizationRole;
        }

        override public void OnActionExecuting(ActionExecutingContext context)
        {
            // Validate
            if (this.authorizationType != AuthorizationType.None)
            {
                try
                {
                    context.HttpContext.Items.Add(HttpContextItemKey.UserInfoKey, JWT.ValidateToken(context.HttpContext, this.authorizationType, this.authorizationRole));
                }
                catch (Exception e)
                {
                    throw new AttributeException(e.Message, HttpStatusCode.Unauthorized)
                    {
                        actionResult = new UnauthorizedResult()
                    };
                }
            }
        }

        override public void OnActionExecuted(ActionExecutedContext context)
        {
            // Extend
            if (this.authorizationType != AuthorizationType.None && context.HttpContext.Items.ContainsKey(HttpContextItemKey.UserInfoKey))
            {
                JWT.ExtendToken(context.HttpContext, (User)context.HttpContext.Items[HttpContextItemKey.UserInfoKey]);
            }
        }
    }
}
