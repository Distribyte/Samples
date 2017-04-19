using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Samples.Glitter
{
    public class Class1
    {
        public static void Main()
        {
            string uriPrefix = "http://localhost";

            WebServiceHost serviceHost = new WebServiceHost(new GlitterEndpoint(), new Uri(uriPrefix + "/service"));
            serviceHost.Open();
            WebServiceHost staticFilesHost = new WebServiceHost(new StaticFilesEndpoint(), new Uri(uriPrefix + "/"));
            staticFilesHost.Open();

            Console.WriteLine("Running, press any key to stop");
            Console.ReadLine();
        }
    }

    [ServiceContract, ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GlitterEndpoint
    {
        private Dictionary<string, User> m_UsersByName = new Dictionary<string, User>();

        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/post/?user={username}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        public void Post(string username, Gleet sentGleet)
        {
            var user = GetUser(username);
            var gleet = new Gleet() { Text = sentGleet.Text, ImageData = sentGleet.ImageData, DateTimeUtc = DateTime.UtcNow, User = user, };

            user.Post(gleet);

            foreach (var follower in user.Followers)
            {
                follower.Post(gleet);
            }
        }

        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/follow/?user={username}&followeeName={followeeName}", ResponseFormat = WebMessageFormat.Json)]
        public void Follow(string username, string followeeName)
        {
            if (username == followeeName)
            {
                return;
            }

            var user = GetUser(username);
            var followee = GetUser(followeeName);

            if (!user.Following.Contains(followee))
            {
                user.Following.Add(followee);
                user.Increment();
                followee.Followers.Add(user);
                followee.Increment();
            }
        }

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "/feed/?user={username}&ver={ver}", ResponseFormat = WebMessageFormat.Json)]
        public async Task<FeedContents> GetFeed(string username, long ver)
        {
            var user = GetUser(username);
            return await user.WaitForVersion(ver);
        }

        private User GetUser(string name)
        {
            User user;
            if (!m_UsersByName.TryGetValue(name, out user))
            {
                user = new User() { Name = name, };
                m_UsersByName.Add(name, user);
            }
            return user;
        }
    }

    [DataContract]
    public class FeedContents
    {
        [DataMember]
        public long Ver { get; set; }

        [DataMember]
        public IEnumerable<Gleet> Gleets { get; set; }

        [DataMember]
        public IEnumerable<string> Following { get; set; }

        [DataMember]
        public IEnumerable<string> Followers { get; set; }
    }

    [DataContract]
    public class Gleet
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DateTime DateTimeUtc { get; set; }

        [DataMember]
        public string UserName { get { return User.Name; } set { } }

        [DataMember]
        public string ImageData { get; set; }

        public User User { get; set; }
    }

    public class User
    {
        private const int s_MaxGleetsPerFeed = 10;
        private Queue<Gleet> m_Gleets = new Queue<Gleet>();
        private VersionTracker m_VersionTracker = new VersionTracker();

        public string Name { get; set; }

        public List<User> Followers { get; private set; } = new List<User>();
        public List<User> Following { get; private set; } = new List<User>();

        public void Increment()
        {
            m_VersionTracker.Increment();
        }

        public async Task<FeedContents> WaitForVersion(long ver)
        {
            if (m_VersionTracker.Version == ver)
            {
                await m_VersionTracker.WaitOne();
            }
            return new FeedContents() { Gleets = m_Gleets.Reverse(), Ver = m_VersionTracker.Version, Following = Following.Select(f => f.Name), Followers = Followers.Select(f => f.Name), };
        }

        public void Post(Gleet gleet)
        {
            m_Gleets.Enqueue(gleet);
            while (m_Gleets.Count > s_MaxGleetsPerFeed)
            {
                m_Gleets.Dequeue();
            }
            m_VersionTracker.Increment();
        }
    }

    // This class provides infrastructure for performing long-polling. This is the server side implementation, and relies on client side logic to perform a looping versioned query.
    public class VersionTracker
    {
        private long m_Version;
        private TaskCompletionSource<object> m_TaskCompletionSource;

        public long Version { get { return m_Version; } }

        public Task WaitOne()
        {
            if (m_TaskCompletionSource == null)
            {
                m_TaskCompletionSource = new TaskCompletionSource<object>();
                QueueCompletion(m_TaskCompletionSource);
            }
            return m_TaskCompletionSource.Task;
        }

        private async void QueueCompletion(TaskCompletionSource<object> completionSource)
        {
            await Task.Delay(30000);
            Complete(completionSource);
        }

        private void Complete(TaskCompletionSource<object> completionSource)
        {
            if (!completionSource.Task.IsCompleted)
            {
                completionSource.TrySetResult(null);
            }
            if (completionSource == m_TaskCompletionSource)
            {
                m_TaskCompletionSource = null;
            }
        }

        public void Increment()
        {
            ++m_Version;
            if (m_TaskCompletionSource != null)
            {
                Complete(m_TaskCompletionSource);
            }
        }
    }
}