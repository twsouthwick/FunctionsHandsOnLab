using FunctionsHandOnLab;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp1
{
    public static class CommentAnalyzer
    {
        [FunctionName("CommentAnalyzer")]
        [return: Table(CommentEntity.Name)]
        public static CommentEntity Run(
            [QueueTrigger(CommentEntity.Name)]string myQueueItem,
            [Table(CommentEntity.Name, CommentEntity.Partition, "{queueTrigger}")]CommentEntity comment,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            comment.Status = "Processed";

            return comment;
        }
    }
}
