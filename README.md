# Sample CPM Project
This sample project provides code snipets on how to call the REST api of the CPM service programmatically. 

# Prerequisites 
## Client Id and Client Secrets
Please refer to http://aka.ms/cpmapi for instructions on how to retrieve the necessary settings. This project assumes that you already have at least one topic id registered within the CPM system.

## Visual Studio Version
You must use VS2015 or VS2017. We do not support any previous versions of Visual Studio.

## NuGet packages
This sample project utilises a package called `Microsoft.Cpm.Api.Contracts` that is supplied and maintained by our team.

### Visual Studio instructions
Please configure your NuGet package manager with the following feed details:
* Name: CPM
* Source: https://microsoft.pkgs.visualstudio.com/_packaging/MCKP.CPM/nuget/v3/index.json

Install the latest version available.

### NuGet command line instructions
If you prefer to install the packages manually, use the following command:
```
Install-Package Microsoft.Cpm.Api.Contracts -Source "https://microsoft.pkgs.visualstudio.com/_packaging/MCKP.CPM/nuget/v3/index.json" -Version 3.0.1
```
Please note that you will have to locate what the latest version is.

### Getting access
If you keep getting a login screen when trying to add our NuGet feed to Visual Studio, it is most likely because your team does not have access to our internal NuGet feed. Please submit an access request through [https://aka.ms/wdgaccess](https://aka.ms/wdgaccess).

### Local installation
While access is being granted, you can temporarily unblock your development pipeline by retrieving an older version from the [Contact Permissions Master Sharepoint](https://microsoft.sharepoint.com/teams/ContactPermissionsMaster/Shared%20Documents/Technical%20Stuff) and creating a local feed.
* Unzip the .zip package; you should see 1 .nupkg file.
* Place the .nupkg file in a folder of your choice.
* Add a new NuGet feed in VS, with the full path of the folder as the source.
* In the project, open the packages.config file and remove the `Microsoft.Cpm.Api.Contracts` reference.
* Open the feed and install the packages from there.

**Please note that the packages in the Sharepoint folder are NOT maintained!** Do follow up on your access request ASAP to enjoy the latest NuGet packages.

### ClientGenerator
Please be aware that the code in the CPMClientGenerator retrieves the AAD token only once. If the token expires, this class does not retrieve a new token, and the call will simply fail with "unauthorized".

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
