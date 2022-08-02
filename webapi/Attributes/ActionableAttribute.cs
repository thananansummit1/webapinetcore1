using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace webapi.Attributes
{
    public class ActionableAttribute : Attribute
    {
        protected bool _requireFilter = true;

        public bool requireFilter
        {
            get
            {
                return this._requireFilter;
            }
        }

        virtual public void OnActionExecuting(ActionExecutingContext context)
        {
            // Validate
        }

        virtual public void OnActionExecuted(ActionExecutedContext context)
        {
            // Extend
        }
    }
}
