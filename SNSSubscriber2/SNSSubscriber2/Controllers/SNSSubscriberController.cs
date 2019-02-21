using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Amazon.SimpleNotificationService;
using RestSharp;
using AWS.Logger.AspNetCore;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Libs.DynamoDb;

namespace SNSSubscriber2.Controllers
{
    [Produces("application/json")]
    [Route("api/SNSSubscriber")]
    public class SNSSubscriberController : ControllerBase
    {
        private readonly IDynamoDbExamples _dynamoDbExamples; // dependency injection 
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public SNSSubscriberController(IDynamoDbExamples dynamoDbExamples)
        {
            _dynamoDbExamples = dynamoDbExamples;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        // use below data to post from postman and uri to post "http://localhost:5000/api/SNSSubscriber/post" and content is Json
        //{
        //	"JsonString" : "test data"
        //   }
        //[HttpPost]
        //[Route("post")]
        //public void Post([FromBody] PostData d)
        //{
        //    var strd = d;
        //    Console.WriteLine("Entered Post" + strd.JsonString);
        //}


        // Data is posted to same  URi mentioned above. Data is just "test data 23456". Content type is Json
        //[HttpPost]
        //[Route("post")]
        //public void Post([FromBody] string d)
        //{
        //    var strd = d;
        //    Console.WriteLine("Entered Post" + strd);
        //}

        // Data is posted to same  URi "http://localhost:5000/api/SNSSubscriber/post". Data is just "test data 23456". Content type is Json
        [HttpPost]
        [Route("poststring")]
        public string Poststring(string d)
        {
            var jsonData = "";
            Stream req = Request.Body;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            String json = new StreamReader(req).ReadToEnd();
            var strd = d;
            //Console.WriteLine("Entered Post" + json);

            var sm = Amazon.SimpleNotificationService.Util.Message.ParseMessage(json);
            if (sm.Type.Equals("SubscriptionConfirmation")) //for confirmation
            {
                //logger.Info("Received Confirm subscription request");
                if (!string.IsNullOrEmpty(sm.SubscribeURL))
                {
                    var uri = new Uri(sm.SubscribeURL);
                   // logger.Info("uri:" + uri.ToString());
                    var baseUrl = uri.GetLeftPart(System.UriPartial.Authority);
                    var resource = sm.SubscribeURL.Replace(baseUrl, "");
                    var response = new RestClient
                    {
                        BaseUrl = new Uri(baseUrl),
                    }.Execute(new RestRequest
                    {
                        Resource = resource,
                        Method = Method.GET,
                        RequestFormat = RestSharp.DataFormat.Xml
                    });
                }
            }
            else // For processing of messages
            {
                _dynamoDbExamples.InsertToDynamoDbTable(5, 1005, json);
                //logger.Info("Message received from SNS:" + sm.TopicArn);
                //dynamic message = JsonConvert.DeserializeObject(sm.MessageText);
                //logger.Info("EventTime : " + message.detail.eventTime);
                //logger.Info("EventName : " + message.detail.eventName);
                //logger.Info("RequestParams : " + message.detail.requestParameters);
                //logger.Info("ResponseParams : " + message.detail.responseElements);
                //logger.Info("RequestID : " + message.detail.requestID);
            }
            return "success";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
