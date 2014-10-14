using System.Runtime.Serialization;

namespace Imod.MsbuildExtensions.Contracts.EdgeCast
{
	/// <summary>
	/// The EdgeCast purge request contract.
	/// </summary>
	[DataContract]
	public class PurgeRequest
	{
		[DataMember]
		public string MediaPath { get; set; }
		[DataMember]
		public int MediaType { get; set; }

		public PurgeRequest()
		{
			// From the EdgeCast documentation this is always 14.
			MediaType = 14;
		}
	}
}