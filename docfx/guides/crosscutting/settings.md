# Settings

When other settings are needed the following can be used:

Settings can be added like this (showed in a sample .settings.json):

    {
      "IsEncrypted": false, // prevents: Failed to decrypt settings
      "Values": {
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "AzureWebJobsStorage": "",
        "AzureWebJobsDashboard": "",
        
        "mySetting": "setting_value"
      }
    }
    
The settings can be used like this:

    var setting = Environment.GetEnvironmentVariable("mySetting");
    
Of course in a azure hosted situation the functions app settings can be used, the setting retrieve code stays the same.
