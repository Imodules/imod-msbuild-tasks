using System;
using System.Collections.Generic;
using System.Xml;
using Imod.MsbuildExtensions.Helpers;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

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

		private RestXml TcRest
		{
			get
			{
				if (_TcRest == null)
				{
					_TcRest = RestXml.Create();
				}
				return _TcRest;
			}
		}
		private RestXml _TcRest = null;

		private string TeamCityRestUrl
		{
			get { return string.Format("{0}{1}", TeamCityUrl, TeamCityRest); }
		}

		private string TeamCityChangesUrl
		{
			get { return string.Format("{0}/changes?build=id:{1}", TeamCityRestUrl, TeamCityBuildNumber); }
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
				var changesetDoc = TcRest.Get(TeamCityChangesUrl);
				// Get our changeset ID's from the XML.
				var changesets = GetChangesets(changesetDoc);
				// Get our files from the changesets.
				FilesInBuild = GetFilesInBuild(changesets);
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
		/// Traverses the XML Document to get the changeset reference URL's.
		/// </summary>
		/// <param name="changesetDoc"></param>
		/// <returns></returns>
		private List<string> GetChangesets(XmlDocument changesetDoc)
		{
			var idList = new List<string>();
			var changeNodes = changesetDoc.DocumentElement.SelectNodes("/changes/change");
			foreach (XmlNode change in changeNodes)
			{
				string changeHref = (change as XmlElement).GetAttribute("href");
				idList.Add(changeHref);
			}
			return idList;
		}

		/// <summary>
		/// Gets the files from each changeset.
		/// </summary>
		/// <param name="changesets"></param>
		/// <returns></returns>
		private string[] GetFilesInBuild(List<string> changesets)
		{
			var files = new List<string>();
			foreach (string href in changesets)
			{
				Log.LogMessage("Getting files for changeset: [{0}]", href);
				var changeDoc = TcRest.Get(string.Format("{0}{1}", TeamCityUrl, href));
				var changeFiles = changeDoc.DocumentElement.SelectNodes("/change/files/file");
				foreach (XmlNode file in changeFiles)
				{
					string filePath = (file as XmlElement).GetAttribute("file");
					Log.LogMessage("Got file: [{0}]", filePath);
					files.Add(filePath);
				}
			}

			return files.ToArray();
		}
		#endregion

		#endregion
	}
}
