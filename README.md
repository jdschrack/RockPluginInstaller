# RockPluginInstaller
A tool to test installation of Rock Plugins.  This tool was created to test plugin installation using the same methods implemented in Rock RMS.  

This tool is not intended to be used to bypass the Rock Shop, only to allow developers the ability to test their plugin package before sending to Rock for verification and distribution.

## Setup
1.	Open \TestRockInstall\App.config.
2.  Add your connection string to the Rock Server you are testing with by edditing the following:
```XML
  <connectionStrings>
    <add name="RockContext" connectionString="{EnterConnectionStringToRockServer}" providerName="System.Data.SqlClient"/>
  </connectionStrings>
```
3.	Edit Program.cs and replace the packagePath (line 14) with the path to the package you are testing.
```C#
var packagePath = @"C:\files\Packages\my.plugin"; //Path to plugin to test.
```
4.	Edit Program.cs and replace the rockServerDirectory value (line 23) with the path to where Rock is installed.
```C#
var rockServerDirectory = @"C:\code\RockKIT\RockWeb"; //Path to RockServer files
```