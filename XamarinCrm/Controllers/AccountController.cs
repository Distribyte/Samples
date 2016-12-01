

using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using XamarinCRM.Models;

namespace XamarinCRMAppService.Controllers
{
    public class AccountController : BaseController
    {
        // GET tables/Account
        [EnableQuery]
        public IQueryable<Account> GetAllAccount()
        {
            return Context.Accounts.AsQueryable(); 
        }

        // GET tables/Account/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [EnableQuery]
        public SingleResult<Account> GetAccount(string id)
        {
            return new SingleResult<Account>(Context.Accounts.Where(c => c.Id == id).AsQueryable());
        }
    }
}
