using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using webapi.Attributes;
using webapi.Core;
using webapi.Models;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : APIController
    {
        public AuthController() { }

        [HttpPost]
        [Route("getoken")]
        [Authorization(AuthorizationType.None)]
        [ServiceFilter(typeof(ActionFilter))]
        public async Task<JsonResult> gettoken(
            [FromForm, Required, EmailAddress] string email,
            [FromForm, Required, MinLength(6)] string password
            )
        {
            User usertoken = new User();
            bool flag = false;
            string token = string.Empty;

            if (email == "customer@test.com" && password == "123456")
            {
                var userList = usertoken.createUserTest();

                var data = userList.Where(x => x.Email.ToLower() == email).FirstOrDefault();

                if (data != null)
                {
                    usertoken = data;
                    flag = true;
                }
            }

            if (email == "supplier@test.com" && password == "123456")
            {
                var userList = usertoken.createUserTest();

                var data = userList.Where(x => x.Email.ToLower() == email).FirstOrDefault();

                if (data != null)
                {
                    usertoken = data;
                    flag = true;
                }
            }

            if (flag)
            {
                token = JWT.GenerateToken(usertoken, this.HttpContext.Request.Headers.ContainsKey(HttpContextItemKey.UserInfoKey));
            }

            return this.ReturnJson(token);
        }

        [HttpGet]
        [Route("checkTokenCustomer")]
        [Authorization(AuthorizationType.Customer)]
        [ServiceFilter(typeof(ActionFilter))]
        public async Task<JsonResult> checkProfileCustomer()
        {
            var user_id = JWT.ValidateToken(this.HttpContext, AuthorizationType.Customer, AuthorizationRole.Customer);

            return this.ReturnJson(user_id);
        }

        [HttpGet]
        [Route("checkTokenSupplier")]
        [Authorization(AuthorizationType.Supplier)]
        [ServiceFilter(typeof(ActionFilter))]
        public async Task<JsonResult> checkProfileSupplier()
        {
            var user_id = JWT.ValidateToken(this.HttpContext, AuthorizationType.Supplier, AuthorizationRole.Supplier);

            return this.ReturnJson(user_id);
        }

    }
}
