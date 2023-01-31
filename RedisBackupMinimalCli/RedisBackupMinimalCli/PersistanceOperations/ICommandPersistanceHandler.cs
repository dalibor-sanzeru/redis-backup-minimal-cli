namespace RedisBackupMinimalCli.PersistanceOperations
{
    public interface ICommandPersistanceHandler
    {
        Task SaveCommands(string directory, string fileName, List<string> serializedCommads);

        Task<List<string>> LoadCommands(string fileName);
    }
}