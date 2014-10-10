imod-msbuild-tasks
==================

Custom msbuild tasks used here on our build server.


TeamCityGetFilesInBuild
=======================
This task will get the files from team city that are in a particular build. It uses the [TeamCity REST API](https://confluence.jetbrains.com/display/TCD8/REST+API) with the [Guest Auth](https://confluence.jetbrains.com/display/TCD8/Guest+User). Just pass it your TeamCity URL and the build number you want the files for.

```xml
<PropertyGroup>
  <FilesInBuild />
</PropertyGroup>
	
<TeamCityGetFilesInBuild
	TeamCityBuildNumber="$(BuildNumber)"
	TeamCityUrl="http://teamcity.me">
	<Output TaskParameter="FilesInBuild" ItemName="FilesInBuild" />
</TeamCityGetFilesInBuild>
```
