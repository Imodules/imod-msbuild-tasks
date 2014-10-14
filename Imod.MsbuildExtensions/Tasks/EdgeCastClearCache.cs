using System;
using System.Collections.Generic;
using System.IO;
using Imod.MsbuildExtensions.Contracts.EdgeCast;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using RestSharp;

namespace Imod.MsbuildExtensions.Tasks
{
	public class EdgeCastClearCache : Task
	{
		#region Properties

		#region Input / Output
		/// <summary>
		/// This is the api token provided by EdgeCast and needed to authenticate
		/// the calls. This will be passed in the Authorization header.
		/// TOK:12345678-1234-1234-1234-1234567890ab
		/// </summary>
		[Required]
		public string RestApiToken { get; set; }

		/// <summary>
		/// This is the REST URl for EdgeCast. It should contain all the way up to the
		/// account number. https://api.transactcdn.com/v2/mcc/customers/0001
		/// </summary>
		[Required]
		public string RestUri { get; set; }

		/// <summary>
		/// This is the base media path that the files will be appended to when calling
		/// the purge api. http://can.0001.transactcdn.com/000001
		/// </summary>
		[Required]
		public string BaseMediaPath { get; set; }

		/// <summary>
		/// The files that were changed that we want to clear the cache on.
		/// </summary>
		[Required]
		public string[] Files { get; set; }

		/// <summary>
		/// The limit of files in a directory to clear individually before we just clear
		/// the directory itself saving calls. 0 Means it will never clear the directory
		/// and always use the individual files. To always do the directory set it to 1
		/// and so long as there is a single file for that directory the entire directory
		/// will be cleared.
		/// </summary>
		[Required]
		public int File2DirectoryThreshold { get; set; }

		/// <summary>
		/// Array of purge request ID's are the output.
		/// </summary>
		[Output]
		public string[] PurgeRequestIds { get; set; }
		#endregion

		#region Private
		/// <summary>
		/// The dictionary to hold the directories and their files we want
		/// to clear.
		/// </summary>
		private Dictionary<string, List<string>> Directories
		{
			get
			{
				if (_Directories == null)
				{
					_Directories = new Dictionary<string, List<string>>();
				}
				return _Directories;
			}
		}
		private Dictionary<string, List<string>> _Directories;

		/// <summary>
		/// List of our purge request ID's for the output.
		/// </summary>
		private List<string> PurgeRequestIdList
		{
			get
			{
				if (_PurgeRequestIdList == null)
				{
					_PurgeRequestIdList = new List<string>();
				}
				return _PurgeRequestIdList;
			}
		}
		private List<string> _PurgeRequestIdList; 
		#endregion

		#endregion

		#region Overrides
		public override bool Execute()
		{
			bool returnStatus = true;
			try
			{
				if (Files.Length > 0)
				{
					ParseInputFileArray();
					returnStatus = ClearCache();
				}
				else
				{
					Log.LogMessage("No files to clear.");
				}
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				returnStatus = false;
			}
			finally
			{
				// Set our output.
				PurgeRequestIds = PurgeRequestIdList.ToArray();
			}

			return returnStatus;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Parses the input file list and creates our internal dictionary for clearing.
		/// </summary>
		private void ParseInputFileArray()
		{
			foreach (string file in Files)
			{
				string fileName = Path.GetFileName(file);
				string filePath = file.Remove(file.LastIndexOf(fileName), fileName.Length);
				
				Log.LogMessage("Got path: [{0}] + file: [{1}]", filePath, fileName);

				// Add the filePath to our list of directories if it doesn't already exist.
				if (!Directories.ContainsKey(filePath))
				{
					Directories.Add(filePath, new List<string>());
				}

				// We do not want to clear the same file more than once. One time will do.
				if (!Directories[filePath].Contains(fileName))
				{
					Directories[filePath].Add(fileName);
				}
			}
		}

		/// <summary>
		/// This method goes through the directory list and makes the calls to EdgeCast
		/// to clear the cache.
		/// </summary>
		private bool ClearCache()
		{
			bool bReturn = true;
			foreach (string dir in Directories.Keys)
			{
				var files = Directories[dir];
				var fileCount = files.Count;
				Log.LogMessage("Working Directory: [{0}] with {1} files.", dir, fileCount);
				
				// If our threshold is set and we have at least that many files in
				// it then we just want to clear everything in that directory and continue.
				if (File2DirectoryThreshold > 0 && fileCount >= File2DirectoryThreshold)
				{
					// Clear the directory and continue.
					if (!SubmitPurgeRequest(dir + "*.*"))
					{
						bReturn = false;
					}
					continue;
				}

				// Clear each file.
				foreach (string file in files)
				{
					// Clear each file.
					if (!SubmitPurgeRequest(dir + file))
					{
						bReturn = false;
					}
				}
			}

			return bReturn;
		}

		/// <summary>
		/// Sends the purge request to edge cast for the media path passed in.
		/// </summary>
		/// <param name="mediaPath"></param>
		private bool SubmitPurgeRequest(string mediaPath)
		{
			bool bReturn = true;

			Log.LogMessage("Purging [{0}]", mediaPath);
			var client = new RestClient(RestUri);

			var request = new RestRequest("/edge/purge", Method.PUT)
			{
				RequestFormat = DataFormat.Json
			};

			request.AddHeader("Authorization", string.Format("TOK:{0}", RestApiToken));
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");

			var purge = new PurgeRequest() {MediaPath = string.Format("{0}/{1}", BaseMediaPath, mediaPath)};
			Log.LogMessage(purge.MediaPath);
			request.AddBody(purge);

			var response = client.Execute<PurgeResponse>(request);
			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				Log.LogError("Error: " + response.ErrorMessage);
				Log.LogMessage("Content: " + response.Content);
				bReturn = false;
			}
			else
			{
				PurgeRequestIdList.Add(response.Data.Id);
			}

			return bReturn;
		}
		#endregion
	}
}
