using System.Net;

namespace Model.Data
{
	public struct ModelConnection
	{
		public bool Local;
		public string DataNameRemote;
		public IPEndPoint DataIpEndPoint;
	}
}
