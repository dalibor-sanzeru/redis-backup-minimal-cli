namespace RedisBackupMinimalCli.PersistanceOperations
{
    public interface ICommandPersistanceHandler
    {
        Task SaveCommands(string fileName, List<string> serializedCommads);

        Task<List<string>> LoadCommands(string fileName);
    }
}