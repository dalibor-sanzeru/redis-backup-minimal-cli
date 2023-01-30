namespace RedisBackupMinimalCli.PersistanceOperations
{
    public interface ICommandPersistanceHandler
    {
        Task SaveCommands(string directory, string fileName, List<string> serializedCommads);

        Task<IEnumerable<string>> LoadCommands(string fileName);
    }
}