using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;


namespace DynamoDB.Libs.DynamoDb
{
    public class DynamoDbExamples : IDynamoDbExamples
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private string tablename = "MySecondDynaTable";

        public DynamoDbExamples(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public void createDynamoDbTable()
        {
            try
            {
                CreateMyTable();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void CreateMyTable()
        {
            Console.WriteLine("Creating Table");
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Number",
                        AttributeType = "N"
                    }
                    //},
                    //new AttributeDefinition
                    //{
                    //    AttributeName = "Description",
                    //    AttributeType = "S"
                    //},
                    //new AttributeDefinition
                    //{
                    //    AttributeName = "Comments",
                    //    AttributeType = "S"
                    //}
                },
                KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"  // This means primary key
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "Number",
                            KeyType = "Range"
                        }
                    },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                TableName = tablename
            };

            var response = _dynamoDbClient.CreateTableAsync(request);
          WaitUntilTableReady(tablename);
        }

        public void WaitUntilTableReady(String tablename)
        {
            string status = null;

            do
            {
                Thread.Sleep(5000);
                try
                {
                    var res = _dynamoDbClient.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tablename
                    });
                    status = res.Result.Table.TableStatus;
                    Console.WriteLine(status);
                }
                catch (ResourceNotFoundException)
                {

                }
            } while (status != "ACTIVE");
            {
                Console.WriteLine("Table Created Successfully");
            }
        }

        public async void InsertToDynamoDbTable(int Id, int Number, string desc)
        {
            Console.WriteLine("Inserting Data");
            var request = new PutItemRequest
            {
                TableName = tablename,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = Id.ToString() }},
                    { "Number", new AttributeValue { N = Number.ToString() }},
                    { "Description", new AttributeValue { S = desc.ToString() }}
                }
            };

            await _dynamoDbClient.PutItemAsync(request);
        }

        public async Task<ActionResult<string>> GetdataFromDynamoDB(int Id)
        {                                               
            var request = new GetItemRequest();

            request.TableName = tablename;
            //request.Key = new Dictionary<String, AttributeValue>()
            //{ 
            //    { "ReplyDateTime", new AttributeValue { N = Id.ToString() } } 
            //};
            // Needed to provide both the elements that are declared as HASH and Range. Did not work when tried with just ID or ReplyDatetime
            request.Key = new Dictionary<String, AttributeValue>()
            { 
                { "Id", new AttributeValue { N = Id.ToString() } },
                { "Number", new AttributeValue { N = (Id + 1000).ToString() } }
            };
            request.ProjectionExpression = "Description";
            var response = await _dynamoDbClient.GetItemAsync(request);

            if (!response.IsItemSet)
                return "Not Found";

            return response.Item["Description"].S.ToString();
        }
    }
}
