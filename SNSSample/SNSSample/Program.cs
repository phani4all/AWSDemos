/*******************************************************************************
* Copyright 2009-2018 Amazon.com, Inc. or its affiliates. All Rights Reserved.
* 
* Licensed under the Apache License, Version 2.0 (the "License"). You may
* not use this file except in compliance with the License. A copy of the
* License is located at
* 
* http://aws.amazon.com/apache2.0/
* 
* or in the "license" file accompanying this file. This file is
* distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied. See the License for the specific
* language governing permissions and limitations under the License.
*******************************************************************************/

using System;

using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;

namespace SNSSample
{
    class Program
    {
        public static void Main(string[] args)
        {
            var sns = new AmazonSimpleNotificationServiceClient();
            string emailAddress = string.Empty;
            var topicArn = "arn:aws:sns:us-east-1:546163609422:OrdersTopic";

            while (string.IsNullOrEmpty(emailAddress))
            {
                Console.Write("Please enter an email address to use: ");
                emailAddress = Console.ReadLine();
            }

            try
            {
                // Create topic
                //Console.WriteLine("Creating topic...");
                //var topicArn = sns.CreateTopic(new CreateTopicRequest
                //{
                //    Name = "SampleSNSTopic"
                //}).TopicArn;

                //// Set display name to a friendly value
                //Console.WriteLine();
                //Console.WriteLine("Setting topic attributes...");
                //sns.SetTopicAttributes(new SetTopicAttributesRequest
                //{
                //    TopicArn = topicArn,
                //    AttributeName = "DisplayName",
                //    AttributeValue = "Sample Notifications"
                //});

                // List all topics
                Console.WriteLine();
                Console.WriteLine("Retrieving all topics...");
                var listTopicsRequest = new ListTopicsRequest();
                ListTopicsResponse listTopicsResponse;
                do
                {
                    listTopicsResponse = sns.ListTopics(listTopicsRequest);
                    foreach (var topic in listTopicsResponse.Topics)
                    {
                        Console.WriteLine(" Topic: {0}", topic.TopicArn);

                        // Get topic attributes
                        var topicAttributes = sns.GetTopicAttributes(new GetTopicAttributesRequest
                        {
                            TopicArn = topic.TopicArn
                        }).Attributes;
                        if (topicAttributes.Count > 0)
                        {
                            Console.WriteLine(" Topic attributes");
                            foreach (var topicAttribute in topicAttributes)
                            {
                                Console.WriteLine(" -{0} : {1}", topicAttribute.Key, topicAttribute.Value);
                            }
                        }
                        Console.WriteLine();
                    }
                    listTopicsRequest.NextToken = listTopicsResponse.NextToken;
                } while (listTopicsResponse.NextToken != null);

                //// Subscribe an endpoint - in this case, an email address
                //Console.WriteLine();
                //Console.WriteLine("Subscribing email address {0} to topic...", emailAddress);
                //sns.Subscribe(new SubscribeRequest
                //{
                //    TopicArn = topicArn,
                //    Protocol = "email",
                //    Endpoint = emailAddress
                //});

                //// When using email, recipient must confirm subscription
                //Console.WriteLine();
                //Console.WriteLine("Please check your email and press enter when you are subscribed...");
                //Console.ReadLine();

                // Publish message
                Orders ord = new Orders();
                ShipToAddress st = new ShipToAddress();
                st.Addr1 = "testAddr1";
                st.Addr2 = "testAddr2";
                st.City = "MyCity";
                st.Zip = "00000";
                ord.OrderId = "12345";
                ord.OrderLocation = "Aurora";
                ord.OrderQty = "100";
                ord.OrderDate = "10222019";
                ord.ShiptoAdr = st;
                string strJason = JsonConvert.SerializeObject(ord);
                Console.WriteLine();
                Console.WriteLine("Publishing message to topic...");
                sns.Publish(new PublishRequest
                {
                    Subject = "Test",
                    Message = strJason,
                    MessageStructure = strJason,
                    TopicArn = topicArn
                });

                // Verify email receieved
                Console.WriteLine();
                Console.WriteLine("Please check your email and press enter when you receive the message...");
                Console.ReadLine();

                //// Delete topic
                //Console.WriteLine();
                //Console.WriteLine("Deleting topic...");
                //sns.DeleteTopic(new DeleteTopicRequest
                //{
                //    TopicArn = topicArn
                //});
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
            }

            Console.WriteLine();
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
    public class Orders
    {
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string OrderLocation { get; set; }
        public string OrderQty { get; set; }
        public ShipToAddress ShiptoAdr;
    }

    public class ShipToAddress
    {
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}