using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Imod.MsbuildExtensions.Contracts.TeamCity
{
	[DataContract]
	public class TcChanges
	{
		[DataMember]
		public int count { get; set; }
		[DataMember]
		public string href { get; set; }
		[DataMember]
		public List<TcChange> change { get; set; }
	}
}