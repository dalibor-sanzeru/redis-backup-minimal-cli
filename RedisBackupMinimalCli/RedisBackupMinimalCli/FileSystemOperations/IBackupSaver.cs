namespace RedisBackupMinimalCli.FileSystemOperations
{
    public interface IBackupSaver
    {
        Task Save(string directory, string fileName, List<string> serializedCommads);
    }
}