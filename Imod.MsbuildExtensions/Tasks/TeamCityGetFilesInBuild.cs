using System;
using System.Collections.Generic;
using Imod.MsbuildExtensions.Contracts.TeamCity;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using RestSharp;

namespace Imod.MsbuildExtensions.Tasks
{
	public class TeamCityGetFilesInBuild : Task
	{
		#region Constants
		private const string TeamCityRest = "/guestAuth/app/rest";
		#endregion

		#region Properties

		#region Input / Output
		/// <summary>
		/// The TeamCity build number that we want to get the changed files for.
		/// </summary>
		[Required]
		public int TeamCityBuildNumber { get; set; }

		/// <summary>
		/// The URL to your team city build server.
		/// </summary>
		[Required]
		public string TeamCityUrl { get; set; }

		/// <summary>
		/// Array of file names that are the output.
		/// </summary>
		[Output]
		public string[] FilesInBuild { get; set; }
		#endregion

		#region Private

		private string TeamCityChangesUrl
		{
			get { return string.Format("{0}/changes?build=id:{1}", TeamCityRest, TeamCityBuildNumber); }
		}
		#endregion

		#endregion

		#region Overrides
		/// <summary>
		/// 1. Calls out to TeamCity with the build number to get the changesets that were a part of the current build.
		/// 2. Calls out to TeamCity with the changeset id's to get the files.
		/// </summary>
		public override bool Execute()
		{
			bool returnStatus = true;
			try
			{
				// Get our change sets from TeamCity.
				Log.LogMessage("Calling TeamCity to get changes: [{0}]", TeamCityChangesUrl);
				var changes = GetChanges();
				// Get our files from the changesets.
				FilesInBuild = GetFilesInBuild(changes);
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				returnStatus = false;
			}

			return returnStatus;
		}
		#endregion

		#region Helper Methods

		#region TeamCity Methods
		/// <summary>
		/// Gets the files from each changeset.
		/// </summary>
		/// <param name="changes"></param>
		/// <returns></returns>
		private string[] GetFilesInBuild(TcChanges changes)
		{
			var files = new List<string>();
			if (changes.change == null)
			{
				return files.ToArray();
			}

			foreach (TcChange change in changes.change)
			{
				Log.LogMessage("Getting files for changeset: [{0}]", change.href);
				var changeDetail = GetChangeDetail(change);

				foreach (TcFile file in changeDetail.files.file)
				{
					Log.LogMessage("Got file: [{0}]", file.file);
					files.Add(file.file);
				}
			}

			return files.ToArray();
		}
		#endregion

		#region Rest Methods

		/// <summary>
		/// Gets the changesets in this build.
		/// </summary>
		/// <returns></returns>
		private TcChanges GetChanges()
		{
			var client = new RestClient(TeamCityUrl);

			var request = new RestRequest(TeamCityChangesUrl, Method.GET);
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");

			var response = client.Execute<TcChanges>(request);

			return response.Data;
		}

		/// <summary>
		/// Gets the file list for each change.
		/// </summary>
		/// <param name="change"></param>
		/// <returns></returns>
		private TcChange GetChangeDetail(TcChange change)
		{
			var client = new RestClient(TeamCityUrl);

			var request = new RestRequest(change.href, Method.GET);
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");

			var response = client.Execute<TcChange>(request);

			return response.Data;
		}
		#endregion

		#endregion
	}
}
