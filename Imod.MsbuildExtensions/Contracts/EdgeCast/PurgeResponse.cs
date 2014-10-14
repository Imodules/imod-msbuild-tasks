using System.Runtime.Serialization;

namespace Imod.MsbuildExtensions.Contracts.EdgeCast
{
	/// <summary>
	/// The EdgeCast purge request response contract.
	/// </summary>
	[DataContract]
	public class PurgeResponse
	{
		[DataMember]
		public string Id { get; set; }
	}
}