

using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using XamarinCRM.Models;

namespace XamarinCRMAppService.Controllers
{
    public class ProductController : BaseController
    {
        // GET tables/Product
        [EnableQuery]
        public IQueryable<Product> GetAllProduct()
        {
            return Context.Products.AsQueryable(); 
        }

        // GET tables/Product/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [EnableQuery]
        public SingleResult<Product> GetProduct(string id)
        {
            return new SingleResult<Product>(Context.Products.Where(c => c.Id == id).AsQueryable());
        }
    }
}
