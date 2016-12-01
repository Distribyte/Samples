

using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using XamarinCRM.Models;

namespace XamarinCRMAppService.Controllers
{
    public class CategoryController : BaseController
    {
        // GET tables/Category
        [EnableQuery]
        public IQueryable<Category> GetAllCategory()
        {
            return Context.Categories.AsQueryable(); 
        }

        // GET tables/Category/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [EnableQuery]
        public SingleResult<Category> GetCategory(string id)
        {
            return new SingleResult<Category>(Context.Categories.Where(c => c.Id == id).AsQueryable());
        }
    }
}
