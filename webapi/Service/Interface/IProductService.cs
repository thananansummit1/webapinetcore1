using System.Threading.Tasks;
using webapi.ViewModels;

namespace webapi.Service.Interface
{
    public interface IProductService
    {
        public Task<ResponseViewModel> testData();
    }
}
