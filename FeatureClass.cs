
namespace MetadataDirectory
{
	public class FeatureClass
	{
		public int Id { get; set; }
		public string DatabaseName { get; set; }
		public string Owner { get; set; }
		public string Name { get; set; }
		public DatasetType DatasetType { get; set; }
	}
}