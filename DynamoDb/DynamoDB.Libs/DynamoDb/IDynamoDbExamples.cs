using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDB.Libs.DynamoDb
{
    public interface IDynamoDbExamples
    {
        void createDynamoDbTable();
        void InsertToDynamoDbTable(int Id, int ReplyDateTime, string desc);
        Task<ActionResult<string>> GetdataFromDynamoDB(int Id);
        //async void InsertToDynamoDbTableAsync(int id, int replyDateTime);
    }
}
