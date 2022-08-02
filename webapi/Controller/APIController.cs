using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace webapi.Controller
{
    public class APIController : ControllerBase
    {
        protected JsonResult ReturnJson(object jsonObj = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            this.HttpContext.Response.StatusCode = (int)statusCode;

            return new JsonResult(new
            {
                Status = (int)statusCode,
                Result = jsonObj
            });
        }
    }
}
