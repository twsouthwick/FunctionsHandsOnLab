using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionsHandOnLab
{
    public class CommentEntity : TableEntity
    {
        public const string Name = "comments";
        public const string Partition = "CommentPartition";

        public string Text { get; set; }
        public string Status { get; set; }
    }
}
