using System.Collections.Generic;
using XamarinCRM.Models;

namespace XamarinCRMAppService.Models
{
    public class MobileServiceContext
    {
        public List<Account> Accounts { get; set; } = new List<Account>();

        public List<Category> Categories { get; set; } = new List<Category>();

        public List<Product> Products { get; set; } = new List<Product>();

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
