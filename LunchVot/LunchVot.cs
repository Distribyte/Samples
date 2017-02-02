using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace Samples.LunchVot
{
    public class LunchVotEntry
    {
        public static void Main()
        {
            Service handler = new Service();

            Uri serviceUri = new Uri(ConfigurationManager.AppSettings["ListenAddress"]);

            WebServiceHost host = new WebServiceHost(handler, serviceUri);

            WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.Transport)
            {
                HostNameComparisonMode = HostNameComparisonMode.Exact,
                MaxReceivedMessageSize = int.MaxValue,
            };

            var endpoint = host.AddServiceEndpoint(handler.GetType(), binding, string.Empty);

            host.Open();
        }

        [ServiceContract]
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class Service
        {
            private Dictionary<string, string> m_VotesByUserId = new Dictionary<string, string>();

            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/bot/", ResponseFormat = WebMessageFormat.Json)]
            public SlackPayload HandleBotAction(Stream request)
            {
                StreamReader reader = new StreamReader(request);

                string res = reader.ReadToEnd();

                var parsedRequest = HttpUtility.ParseQueryString(res);

                var tokenString = parsedRequest.Get("token");

                if (tokenString != "YOUR_TOKEN_HERE")
                {
                    throw new WebFaultException(HttpStatusCode.Unauthorized);
                }

                if (parsedRequest.Get("user_name") == "slackbot")
                {
                    return null;
                }

                var response = GetResponse(parsedRequest.Get("text"), parsedRequest.Get("user_id"));

                return response;
            }

            private SlackPayload GetResponse(string command, string userId)
            {
                if (command.ToLower().StartsWith("vote "))
                {
                    var place = command.Substring("vote ".Length);
                    string existingVote;
                    if (m_VotesByUserId.TryGetValue(userId, out existingVote))
                    {
                        if (existingVote != place)
                        {
                            return CreateFailurePayload($"You've already voted for {existingVote} this round. Unvote first to suggest or vote for another place.");
                        }
                    }
                    else
                    {
                        m_VotesByUserId[userId] = place;
                    }

                    return CreateSummaryPayload();
                }
                else if (command.StartsWith("unvote"))
                {
                    bool removeSucceeded = m_VotesByUserId.Remove(userId);
                    if (removeSucceeded)
                    {
                        return CreateSummaryPayload();
                    }
                    else
                    {
                        return CreateFailurePayload("You've not voted in this round yet");
                    }
                }
                else if (command.StartsWith("list"))
                {
                    return CreateSummaryPayload();
                }
                else if (command.StartsWith("reset"))
                {
                    var payload = CreateSummaryPayload();
                    m_VotesByUserId.Clear();

                    payload.Text = payload.Text + "The vote has been reset.";

                    return payload;
                }
                else
                {
                    return CreateUsagePayload();
                }
            }

            private SlackPayload CreateUsagePayload()
            {
                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine("Usage:");
                resultBuilder.AppendLine("Vote - add your vote to a location for lunch.");
                resultBuilder.AppendLine("Ex. 'lunchvot vote Cafe1'");
                resultBuilder.AppendLine("Unvote - Remove your vote from the current round (so you can vote again if you'd like).");
                resultBuilder.AppendLine("Ex. 'lunchvot unvote'");
                resultBuilder.AppendLine("List - show the current results.");
                resultBuilder.AppendLine("Ex. 'lunchvot list'");
                resultBuilder.AppendLine("Reset - show the results and then reset all votes, effectively starting a new round.");
                resultBuilder.AppendLine("Ex. 'lunchvot reset'");
                return new SlackPayload() { Text = resultBuilder.ToString(), };
            }

            private SlackPayload CreateSummaryPayload()
            {
                if (m_VotesByUserId.Count == 0)
                {
                    return new SlackPayload() { Text = "No votes have been cast", };
                }

                var results = from vote in m_VotesByUserId
                              group vote by vote.Value into place
                              orderby place.Count() descending
                              select new { Place = place.Key, Votes = place.Count(), };

                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine("Vote results are: ");
                foreach (var result in results)
                {
                    resultBuilder.AppendLine($"{result.Place} - {result.Votes} votes");
                }

                return new SlackPayload() { Text = resultBuilder.ToString() };
            }

            private SlackPayload CreateFailurePayload(string text)
            {
                return new SlackPayload() { Text = text, };
            }
        }

        [DataContract]
        public class SlackPayload
        {
            [DataMember(Name = "text")]
            public string Text { get; set; }

            [DataMember(Name = "username")]
            public string Username { get; set; }
        }
    }
}
