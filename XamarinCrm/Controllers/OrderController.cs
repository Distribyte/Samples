

using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using XamarinCRM.Models;

namespace XamarinCRMAppService.Controllers
{
    public class OrderController : BaseController
    {
        // GET tables/Order
        [EnableQuery]
        public IQueryable<Order> GetAllOrder()
        {
            return Context.Orders.AsQueryable();
        }

        // GET tables/Order/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [EnableQuery]
        public SingleResult<Order> GetOrder(string id)
        {
            return new SingleResult<Order>(Context.Orders.Where(c => c.Id == id).AsQueryable());
        }
    }
}
