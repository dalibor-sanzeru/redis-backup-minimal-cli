using CommandLine;

namespace RedisBackupMinimalCli
{
    public class Options
    {
        public const int DefaultBatchSize = 1000;

        [Option("operation", Required = true, HelpText = $"Operation to execute.")]
        public OperationType Operation { get; set; }

        [Option("redis", Required = true, HelpText = "Redis connection string.")]
        public string Redis { get; set; }

        [Option("fileName", Required = true, HelpText = "File where backup/restore files are being stored.")]
        public string FileName { get; set; }

        [Option("keys", Required = false, HelpText = "Keys to backup.")]
        public IEnumerable<string> Keys { get; set; }

        [Option("batchSize", Required = false, HelpText = $"Batch size for redis backup/restore async operations. In case you are getting TimeOutExceptions make it smaller.", Default = DefaultBatchSize)]
        public int BatchSize { get; set; } = DefaultBatchSize;
    }
}
