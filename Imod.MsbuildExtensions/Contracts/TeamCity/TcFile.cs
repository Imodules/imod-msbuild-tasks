using System.Runtime.Serialization;
using RestSharp.Deserializers;

namespace Imod.MsbuildExtensions.Contracts.TeamCity
{
	/// <summary>
	/// Team City file contract.
	/// </summary>
	[DataContract]
	public class TcFile
	{
		[DataMember]
		[DeserializeAs(Name = "before-revision")]
		public string beforeRevision { get; set; }
		[DataMember]
		[DeserializeAs(Name = "after-revision")]
		public string afterRevision { get; set; }
		[DataMember]
		public string file { get; set; }
		[DataMember]
		[DeserializeAs(Name = "relative-file")]
		public string relativeFile { get; set; }
	}
}