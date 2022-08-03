using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using webapi.Models.ProductService;
using webapi.Service.Interface;
using webapi.ViewModels;

namespace webapi.Service
{
    public class ProductService : IProductService
    {
        public ProductService() { }

        public async Task<ResponseViewModel> testData()
        {
            var res = new ResponseViewModel();

            var dummy = await dummyItem();

            res.status = true;
            res.Data = dummy;

            return res;
        }

        private async Task<List<ProductServiceModel>> dummyItem()
        {
            await Task.Delay(100);

            var Listitem = new List<ProductServiceModel>()
            {
                new ProductServiceModel()
                {
                    uid = Guid.NewGuid(), Name = "ประยุด", UpdateDate = DateTime.Now
                },
                new ProductServiceModel()
                {
                    uid = Guid.NewGuid(), Name = "ประวิด", UpdateDate = DateTime.Now
                },
                new ProductServiceModel()
                {
                    uid = Guid.NewGuid(), Name = "อนุทิน", UpdateDate = DateTime.Now
                }
            };

            return Listitem;
        }
    }
}
