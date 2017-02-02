using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Samples.BotStarter
{
    public class Program
    {
        private const string botName = "MyBot";

        /// <summary>
        /// This is the application entry point. It is called once, when the application starts.
        /// This method allows you to configure your application to listen for incoming requests.
        /// </summary>
        public static void Main()
        {
            //
            // Create  anew instance of our bot service.
            //
            Service handler = new Service();

            //
            // Use WebServiceHost to host our service.
            //
            WebServiceHost host = new WebServiceHost(handler, new Uri("http://localhost/slack/"));
            host.Open();

            Console.WriteLine("Your bot is running. Press any key to exit.");
            Console.ReadLine();
        }

        [ServiceContract]
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
        private class Service
        {
            /// <summary>
            /// This method is invoked when Slack sends a message to your bot.
            /// It receives information about the message sent to the bot, and allows you to return a response back to Slack.
            /// The response is used by Slack to display information back to the sending user.
            /// </summary>
            /// <param name="request">POST form data containing information about the request, as well as authentication info.</param>
            /// <returns>A SlackPayload representing the response to display in the channel.</returns>
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/bot/", ResponseFormat = WebMessageFormat.Json)]
            public SlackPayload HandleBotAction(Stream request)
            {
                //
                // Parse the incoming request from POST data to a more usable key-value format.
                //
                NameValueCollection parsedRequest = ParseRequest(request);

                //
                // Verify the token sent by Slack as part of the request.
                // The token is a secret known only to your bot and Slack, which allows you to make sure the message originated from a Slack server.
                // This prevents malicious users from sending your bot fake messages.
                //
                // This section is currently commented to help you get started easier.
                //var tokenString = parsedRequest.Get("token");
                //if (tokenString != "YOUR_TOKEN_HERE")
                //{
                //    //
                //    // If the token doesn't match the expected value, fail this request by throwing an appropriate exception.
                //    //
                //    throw new WebFaultException(HttpStatusCode.Unauthorized);
                //}

                //
                // Filter out messages sent by a bot.
                // Slack sends a notification when a bot posts a response in a channel. This includes this bot.
                // Removing this will cause the bot to handle its own messages and attempt to reply to them.
                // This could potentially lead to an endless loop.
                //
                if (parsedRequest.Get("user_name") == "slackbot")
                {
                    return null;
                }

                //
                // Finally, let the bot logic handle this message and craft a response.
                //
                var response = GetResponse(parsedRequest.Get("text"), parsedRequest);

                return response;
            }

            /// <summary>
            /// This method parses incoming requests from Slack (which are sent as POST form data).
            /// </summary>
            /// <param name="request">The incoming request stream from Slack</param>
            /// <returns>A NameValueCollection containing the parts of the request in key-value format.</returns>
            private static NameValueCollection ParseRequest(Stream request)
            {
                //
                // Read the stream into a string
                //
                StreamReader reader = new StreamReader(request);
                string res = reader.ReadToEnd();

                //
                // And then use HttpUtility to parse the string into form encoded parts.
                //
                var parsedRequest = HttpUtility.ParseQueryString(res);
                return parsedRequest;
            }

            private SlackPayload GetResponse(string command, NameValueCollection parsedRequest)
            {
                //
                // Add your logic here to handle incoming requests from Slack and return a response.
                //

                return new SlackPayload() { Text = "This is my first response", Username = botName };
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
