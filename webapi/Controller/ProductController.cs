using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webapi.Attributes;
using webapi.Core;
using webapi.Service.Interface;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : APIController
    {
        private readonly IProductService _ProductService;
        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }

        [HttpGet]
        [Route("getProduct")]
        [Authorization(AuthorizationType.Customer)]
        [ServiceFilter(typeof(ActionFilter))]
        public async Task<JsonResult> getProduct()
        {
            JWT.ValidateToken(this.HttpContext, AuthorizationType.Customer, AuthorizationRole.Customer);

            var res = await _ProductService.testData();

            return this.ReturnJson(res);
        }
    }
}
