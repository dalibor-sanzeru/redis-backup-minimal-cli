namespace RedisBackupMinimalCli.FileSystemOperations
{
    public interface IBackupSaver
    {
        Task Save(string path, List<string> results);
    }
}