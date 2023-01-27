# This is in progress doc&project !!! Dont use it!!! First version will be released in FEB 2023.

# redis-backup-minimal-cli
Redis backup minimal CLI is single Use-Case tool to backup/restore redis database.


## How to get it (Use one of them):
1. Download installer: [redis-backup-minimal-cli.setup](https://installer-url-will-be-here) 
2. Use as [.NET tool](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools):
```
dotnet tool install -g redis-backup-minimal-cli
```

## 2. Take Backup:
```
redis-backup-minimal-cli --operation backup --directory C:\\MyFolder\ --redis http://redis-backup-connection-url --keys firstkey:* secondKey:* 
```

## 3. Restore backup: 

```
redis-backup-minimal-cli --operation restore --directory C:\\MyFolder\ --redis http://redis-restore-connection-url --keys firstkey:* secondKey:* 
```
