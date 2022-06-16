namespace Zigurous.AssetDownloader
{
    public sealed class TheBaseMesh : AssetSource
    {
        public override AssetSourceType sourceType => AssetSourceType.TheBaseMesh;
        public override string sourceUrl => "https://thebasemesh.com";

        public override void OnImportGUI(ImportContext context)
        {
            if (CustomGUI.CenteredButton("Import Model")) {
                Import(context);
            }
        }

        public override string GetDownloadUrl(ImportContext context)
        {
            return $"{sourceUrl}/s/{context.assetId}.zip";
        }

        public override bool Filter(string fileName)
        {
            return fileName.Contains(".fbx");
        }

    }

}
