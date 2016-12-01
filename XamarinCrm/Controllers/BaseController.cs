using System.Web.Http;
using XamarinCRMAppService.Models;

namespace XamarinCRMAppService.Controllers
{
    public abstract class BaseController : ApiController
    {
        public static MobileServiceContext Context;
    }
}
