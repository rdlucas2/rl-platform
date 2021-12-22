appsettings.json or appsettings.Development.json file should look like this (gitignored):
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Twitter":{
    "ConsumerKey": "",
    "ConsumerSecret": "",
    "AccessToken": "",
    "AccessTokenSecret": "",
    "SearchTerm": "dotnet"
  },
  "RetryFrequencyInMs": 60000
}
```