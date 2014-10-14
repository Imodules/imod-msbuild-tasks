imod-msbuild-tasks
==================

Custom msbuild tasks used here on our build server.


TeamCityGetFilesInBuild
=======================
This task will get the files from team city that are in a particular build. It uses the [TeamCity REST API](https://confluence.jetbrains.com/display/TCD8/REST+API) with the [Guest Auth](https://confluence.jetbrains.com/display/TCD8/Guest+User). Just pass it your TeamCity URL and the build number you want the files for.

|       | Property            | Description
| ----- | ------------------- | -----------
| Input | TeamCityUrl         | The URL to your Team City build server.
| Input | TeamCityBuildNumber | The Team City build number you want to query the files for.
| Output| FilesInBuild        | String array of the files found for this build number.

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

EdgeCastClearCache
==================
This task will take a list of input files and call out to EdgeCast CDN and issue a purge request.

|       | Property                | Description
| ----- | ----------------------- | -----------
| Input | RestUri                 | This is the REST URl for EdgeCast. It should contain all the way up to the.
| Input | RestApiToken            | his is the api token provided by EdgeCast and needed to authenticate. This will be passed in the Authorization header.
| Input | BaseMediaPath           | This is the base media path that the files will be appended to when calling the purge api.
| Input | Files                   | The files that were changed that we want to clear the cache on.
| Input | File2DirectoryThreshold | The limit of files in a directory to clear individually before we just clear the directory itself saving calls. 0 Means it will never clear the directory and always use the individual files. To always do the directory set it to 1 and so long as there is a single file for that directory the entire directory will be cleared.
| Output| PurgeRequestIds         | Array of the purge request ID's returned by EdgeCast.

```xml
<PropertyGroup>
	<FilesInBuild />
	<PurgeRequestIds />
</PropertyGroup>

<EdgeCastClearCache 
	Files="@(FilesInBuild)"
	File2DirectoryThreshold="0"
	RestApiToken="aaaaaaaa-bbbbbb-cccccc-dddddd-eeeeeeeeeeee"
	RestUri="https://api.transactcdn.com/v2/mcc/customers/000001"
	BaseMediaPath="http://can.0001.transactcdn.com/000001">
	<Output TaskParameter="PurgeRequestIds" ItemName="PurgeRequestIds" />
</EdgeCastClearCache>
```
