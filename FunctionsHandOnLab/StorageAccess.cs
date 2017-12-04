using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionsHandOnLab
{
    public class StorageAccess
    {
        private readonly CloudTable _commentTable;
        private readonly CloudQueue _commentQueue;
        private readonly Task _initialize;

        public StorageAccess()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = account.CreateCloudTableClient();
            var queueClient = account.CreateCloudQueueClient();

            _commentTable = tableClient.GetTableReference(CommentEntity.Name);
            _commentQueue = queueClient.GetQueueReference(CommentEntity.Name);

            _initialize = Task.Run(async () => await InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            await _commentTable.CreateIfNotExistsAsync();
            await _commentQueue.CreateIfNotExistsAsync();
        }

        public async Task<int> GetQueueCount()
        {
            await _initialize;
            await _commentQueue.FetchAttributesAsync();

            return _commentQueue.ApproximateMessageCount ?? 0;
        }

        public async Task AddCommentAsync(CommentModel comment)
        {
            await _initialize;

            var entity = new CommentEntity
            {
                PartitionKey = CommentEntity.Partition,
                RowKey = Guid.NewGuid().ToString(),
                Text = comment.Text,
                Status = comment.Status
            };

            await _commentTable.ExecuteAsync(TableOperation.Insert(entity));
            await _commentQueue.AddMessageAsync(new CloudQueueMessage(entity.RowKey));
        }

        public async Task<IEnumerable<CommentModel>> GetCommentsAsync()
        {
            await _initialize;

            var token = default(TableContinuationToken);
            var comments = new List<CommentModel>();

            do
            {
                var result = await _commentTable.ExecuteQuerySegmentedAsync(new TableQuery<CommentEntity>(), token);

                foreach (var r in result)
                {
                    var comment = new CommentModel
                    {
                        Text = r.Text,
                        Status = r.Status,
                        LastUpdated = r.Timestamp
                    };

                    comments.Add(comment);
                }
            } while (token != default(TableContinuationToken));

            return comments;
        }
    }
}
