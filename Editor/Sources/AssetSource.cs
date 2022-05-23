using System.Collections.Generic;
using UnityEditor;

namespace Zigurous.AssetDownloader
{
    public abstract class AssetSource
    {
        public abstract AssetSourceType sourceType { get; }
        public abstract string sourceUrl { get; }

        public abstract string GetDownloadUrl(ImportContext context);

        public virtual string Rename(string fileName) => fileName;
        public virtual bool Filter(string fileName) => fileName != ".DS_Store";

        public virtual void Import(ImportContext context)
        {
            if (context.HasAssetId()) {
                WebClient.Download(this, context);
            } else {
                Log.Error("An Asset ID has not been set");
            }
        }

        public virtual void ImportComplete(ImportContext context, List<string> files)
        {
            AssetDatabase.Refresh();

            Log.Message("Import Complete");
        }

        public virtual void OnSettingsGUI(ImportContext context) {}
        public virtual void OnOutputGUI(ImportContext context) {}
        public virtual void OnImportGUI(ImportContext context) {}
    }

    public enum AssetSourceType
    {
        AmbientCG,
        TheBaseMesh,
    }

}
