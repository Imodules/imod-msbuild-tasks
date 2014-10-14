using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Imod.MsbuildExtensions.Contracts.TeamCity
{
	/// <summary>
	/// Team City list of files contract.
	/// </summary>
	[DataContract]
	public class TcFiles
	{
		[DataMember]
		public List<TcFile> file { get; set; }
	}
}