using CommandLine;

namespace RedisBackupMinimalCli
{
    public class Options
    {
        [Option("operation", Required = true, HelpText = $"Operation to execute.")]
        public OperationType Operation { get; set; }

        [Option("redis", Required = true, HelpText = "Redis connection string.")]
        public string Redis { get; set; }

        [Option("directory", Required = true, HelpText = "File where backup/restore files are being stored.")]
        public string FileName { get; set; }

        [Option("keys", Required = false, HelpText = "Keys to backup.")]
        public IEnumerable<string> Keys { get; set; }
    }
}
