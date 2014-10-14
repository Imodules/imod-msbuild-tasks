using System;
using System.Runtime.Serialization;
using Imod.MsbuildExtensions.Tasks;

namespace Imod.MsbuildExtensions.Contracts.TeamCity
{
	/// <summary>
	/// Team City change contract.
	/// </summary>
	[DataContract]
	public class TcChange
	{
		[DataMember]
		public int id { get; set; }
		[DataMember]
		public string version { get; set; }
		[DataMember]
		public string username { get; set; }
		[DataMember]
		public DateTime date { get; set; }
		[DataMember]
		public string href { get; set; }
		[DataMember]
		public string webLink { get; set; }
		[DataMember]
		public string comment { get; set; }
		[DataMember]
		public TcFiles files { get; set; }
	}
}