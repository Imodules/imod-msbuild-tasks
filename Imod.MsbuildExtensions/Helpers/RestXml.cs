using System.IO;
using System.Net;
using System.Xml;

namespace Imod.MsbuildExtensions.Helpers
{
	public class RestXml
	{
		public static RestXml Create()
		{
			return new RestXml();
		}

		private RestXml() { }

		public XmlDocument Get(string url)
		{
			XmlDocument doc = new XmlDocument();

			doc.LoadXml(RequestGet(url));

			return doc;
		}

		private StreamReader GetStream(string restURL)
		{
			HttpWebRequest restRequest;
			HttpWebResponse restResponse;

			restRequest = (HttpWebRequest)WebRequest.Create(restURL);
			restRequest.Method = "GET";

			restResponse = (HttpWebResponse)restRequest.GetResponse();
			return new StreamReader(restResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
		}

		private string RequestGet(string restURL)
		{
			string xmlResponse = string.Empty;
			using (StreamReader readStream = GetStream(restURL))
			{
				xmlResponse = readStream.ReadToEnd();
			}

			return xmlResponse;
		}
	}
}
