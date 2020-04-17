using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Linq;

namespace ConsoleApp_storageCtrl
{
    class Program
    {
        static string connStr = "DefaultEndpointsProtocol=https;AccountName=xxx";
        static string queueName = "queue0";
        static void Main(string[] args)
        {
            //connStr contains the storage account connection string. 
            var storageAccount = CloudStorageAccount.Parse(connStr);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            //Getting 32 messages to process (seems to be the max number using this client).
            var pendingMsgs = queue.GetMessages(32);
            if (pendingMsgs.Count() <= 0)
                return;

            Console.WriteLine("pending messages = " + pendingMsgs.Count());

            foreach (var retrievedMessage in pendingMsgs)
            {
                var InsertionTime = retrievedMessage.InsertionTime.Value.UtcDateTime;
                var diff = DateTime.UtcNow - InsertionTime;

                if (diff.TotalMinutes < 2)
                    continue;

                Console.WriteLine(retrievedMessage.AsString);
                queue.DeleteMessage(retrievedMessage);
                Console.WriteLine("message stayed more than 2 mins deleted.");
            }

            Console.ReadLine();
        }
    }
}
