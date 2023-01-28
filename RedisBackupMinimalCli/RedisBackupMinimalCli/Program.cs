using CommandLine;
using StackExchange.Redis;

namespace RedisBackupMinimalCli
{
    static class Program
    {
        static Task Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                    .WithParsedAsync<Options>(o =>
                    {
                        var m = new CliManager();
                        return m.Execute(o);
                    });
        }
    }
}