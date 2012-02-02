using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MetadataDirectory.Properties;

namespace MetadataDirectory
{
	/// <summary>
	/// Generates an index of all of the feature classes on the SDE server.
	/// </summary>
	public class index : IHttpHandler
	{

		/// <summary>
		/// Compares strings case-insensitively.
		/// </summary>
		internal class StringComparer : EqualityComparer<string>
		{

			public override bool Equals(string x, string y)
			{
				return string.Compare(x, y, true) == 0;
			}

			public override int GetHashCode(string obj)
			{
				return obj.ToUpperInvariant().GetHashCode();
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write("<!DOCTYPE html ><html><head><title>Metadata</title>");
			context.Response.Write("<link rel='stylesheet' href='http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.17/themes/base/jquery-ui.css' type='text/css' media='all' />");
			context.Response.Write("<link rel='stylesheet' href='style/index.css' type='text/css' media='all' />");
			context.Response.Write("<script src='scripts/modernizr.js'></script>");
			context.Response.Write("</head><body>");

			context.Response.Write("<h1>Metadata</h1>");
			
			// Connect to the SDE server and create a list of feature classes.
			var featureClasses = new List<FeatureClass>();
			using (var conn = new SqlConnection(ConfigurationManager.AppSettings["SdeConnectionString"]))
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = Resources.ListFeatureClasses;
				var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					featureClasses.Add(new FeatureClass
					{
						Id = reader.GetInt32(0),
						DatabaseName = reader.GetString(1),
						Owner = reader.GetString(2),
						Name = reader.GetString(3),
						DatasetType = (DatasetType)reader.GetInt32(4)
					});
				}
				
			}

			var stringComparer = new StringComparer();

			// Group the feature classes by Database Name.
			var groupedByDb = featureClasses.GroupBy(fc => fc.DatabaseName, stringComparer);

			foreach (var dbGroup in groupedByDb)
			{
				// Group the sub group of feature classes by owner.
				var groupedByOwner = dbGroup.GroupBy(fc => fc.Owner, stringComparer);
				// Create an HTML section for the database.
				context.Response.Write("<section>");
				context.Response.Write(string.Format("<h2>{0}</h2>", dbGroup.Key));
				foreach (var ownerGroup in groupedByOwner)
				{
					// Create an HTML section for the owner.
					context.Response.Write("<section>");
					context.Response.Write(string.Format("<h3>{0}</h3>", ownerGroup.Key));
					// Create a list of feature classes.  All items in this list will belong to the same owner and database.
					context.Response.Write("<ul>");

					foreach (var fc in ownerGroup)
					{
						context.Response.Write(string.Format("<li class='{3}'><a class='{3}' href='Metadata.ashx?name={0}.{1}.{2}'>{0}.{1}.{2}</a></li>", fc.DatabaseName, fc.Owner, fc.Name, fc.DatasetType));
					}
					context.Response.Write("</ul>");
					context.Response.Write("</section>");
				}
				context.Response.Write("</section>");
			}
			context.Response.Write("<script src='http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js'></script>");
			context.Response.Write("<script src='http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.16/jquery-ui.min.js'></script>");
			context.Response.Write("<script src='scripts/toc.js'></script>");
			context.Response.Write("<script src='scripts/index.js'></script>");
			context.Response.Write("</body></html>");

			context.Response.ContentType = "text/html";
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