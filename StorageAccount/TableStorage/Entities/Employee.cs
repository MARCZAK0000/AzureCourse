using Azure;
using Azure.Data.Tables;

namespace TableStorage.Entities
{
    public class Employee : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey {  get; set; } 

        public DateTimeOffset? Timestamp {  get; set; }

        public ETag ETag { get; set; }

        public string email { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }    
    }
}
