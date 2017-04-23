using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Samples.Glitter
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class StaticFilesEndpoint
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/")]
        public Stream GetFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Samples.Glitter.feed.html";

            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
