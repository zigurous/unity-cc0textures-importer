using UnityEditor;
using UnityEngine;

namespace Zigurous.AssetDownloader
{
    public sealed class EditorWindow : UnityEditor.EditorWindow
    {
        private AssetSource source;
        private AssetSourceType sourceType;
        private ImportContext context = new ImportContext();

        private bool expandSettings = true;
        private bool expandOutput = true;

        [MenuItem("Tools/Asset Downloader")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EditorWindow), false, "Asset Downloader");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Asset Downloader", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Select the source to download assets from, enter an Asset ID, then click the import button to start the download.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            using (new EditorGUI.IndentLevelScope(1))
            {
                SettingsGUI();
                OutputGUI();
            }

            EditorGUILayout.Space(10f);
            source.OnImportGUI(context);
        }

        private void SettingsGUI()
        {
            expandSettings = EditorGUILayout.BeginFoldoutHeaderGroup(expandSettings, "Settings");

            if (expandSettings)
            {
                EditorGUILayout.BeginHorizontal();

                sourceType = (AssetSourceType)EditorGUILayout.EnumPopup("Source", sourceType);

                if (source == null || source.sourceType != sourceType) {
                    source = CreateAssetSource(sourceType);
                }

                if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
                    Application.OpenURL(source.sourceUrl);
                }

                EditorGUILayout.EndHorizontal();

                context.assetId = EditorGUILayout.TextField("Asset ID", context.assetId);
                source.OnSettingsGUI(context);

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void OutputGUI()
        {
            expandOutput = EditorGUILayout.BeginFoldoutHeaderGroup(expandOutput, "Output");

            if (expandOutput)
            {
                context.outputName = EditorGUILayout.TextField("Output Name", context.outputName);
                context.outputPath = EditorGUILayout.TextField("Output Path", context.outputPath);
                source.OnOutputGUI(context);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private AssetSource CreateAssetSource(AssetSourceType type)
        {
            switch (type)
            {
                case AssetSourceType.AmbientCG:
                    return new AmbientCG();
                case AssetSourceType.TheBaseMesh:
                    return new TheBaseMesh();
                default:
                    return null;
            }
        }

    }

}
