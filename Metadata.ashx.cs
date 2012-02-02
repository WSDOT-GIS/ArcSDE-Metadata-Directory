/*
 * Copyright (c) 2012 Washington State Department of Transportation
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 *
 */
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using MetadataDirectory.Properties;

namespace MetadataDirectory
{
	/// <summary>
	/// Returns metadata from an ArcSDE Feature Class.
	/// </summary>
	public class Metadata : IHttpHandler
	{
		public static Regex _nameRe = new Regex(@"(?<databaseName>\w+)\.(?<owner>\w+)\.(?<name>\w+)");
		public void ProcessRequest(HttpContext context)
		{
			string idStr = context.Request.Params["id"];
			string name = context.Request.Params["name"];
			int id = -1;
			string xml = null;

			using (var conn = new SqlConnection(ConfigurationManager.AppSettings["SdeConnectionString"]))
			{
				if (!string.IsNullOrWhiteSpace(idStr) && int.TryParse(idStr, out id))
				{
					conn.Open();
					var cmd = conn.CreateCommand();
					cmd.CommandText = Resources.GetMetadataById;
					cmd.Parameters.AddWithValue("@id", id);
					xml = cmd.ExecuteScalar() as string;
				}
				else if (!string.IsNullOrWhiteSpace(name))
				{
					Match match = _nameRe.Match(name);
					if (match.Success)
					{
						conn.Open();
						var cmd = conn.CreateCommand();
						cmd.CommandText = Resources.GetMetadataByName;
						cmd.Parameters.AddWithValue("@databaseName", match.Groups["databaseName"].Value);
						cmd.Parameters.AddWithValue("@owner", match.Groups["owner"].Value);
						cmd.Parameters.AddWithValue("@name", match.Groups["name"].Value);
						xml = cmd.ExecuteScalar() as string;
					}
				}
			}





			var xsl = new XslCompiledTransform();
			xsl.Load(HttpContext.Current.Server.MapPath("~/style/FGDC Plus HTML5.xsl"));

			var doc = new XmlDocument();
			doc.LoadXml(xml);

			context.Response.ContentType = "text/html";
			XsltArgumentList args = new XsltArgumentList();
			args.AddParam("includeDublinCore", string.Empty, false);
			args.AddParam("externalJS", string.Empty, "scripts/fgdcPlus.js");
			args.AddParam("externalCss", string.Empty, "style/fgdcPlus.css");
			xsl.Transform(doc, args, context.Response.OutputStream);


			////context.Response.ContentType = "text/xml";
			////context.Response.Write(xml);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}