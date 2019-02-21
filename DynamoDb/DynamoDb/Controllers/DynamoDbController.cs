using System;
using System.Collections.Generic;
using System.Linq;
using DynamoDB.Libs.DynamoDb;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;

namespace DynamoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/DynamoDb")]
    //[ApiController] this came by default and i added the produces
    public class DynamoDbController : ControllerBase
    {
        private readonly IDynamoDbExamples _dynamoDbExamples; // dependency injection 
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public DynamoDbController(IDynamoDbExamples dynamoDbExamples)
        {
            _dynamoDbExamples = dynamoDbExamples;
        }

        [Route("createtable")]
        public IActionResult CreateDynamoDbTable()
        {
            _dynamoDbExamples.createDynamoDbTable();
            return Ok();
        }

        // POST api/values
        [HttpPost]
        [Route("Inserttable")]
        public async Task Post([FromBody] PostInput input)
        //public async Task Post()
        {
            Console.WriteLine("Inserting Data");
            _dynamoDbExamples.InsertToDynamoDbTable(input.Id, input.Number, input.Desc);

        }

        [HttpGet("{id}")]
        [Route("GetById")]
        public async Task<ActionResult<string>> Get([FromQuery]int id)
        {
            Console.WriteLine("Fetching Data");
            return await _dynamoDbExamples.GetdataFromDynamoDB(id);
        }
    }
}