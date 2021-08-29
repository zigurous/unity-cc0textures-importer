# CC0 Textures Importer

[![](https://img.shields.io/badge/github-repo-blue?logo=github)](https://github.com/zigurous/unity-cc0textures-importer) [![](https://img.shields.io/github/package-json/v/zigurous/unity-cc0textures-importer)](https://github.com/zigurous/unity-cc0textures-importer/releases) [![](https://img.shields.io/github/license/zigurous/unity-cc0textures-importer)](https://github.com/zigurous/unity-cc0textures-importer/blob/main/LICENSE.md)

The **CC0 Textures Importer** package provides a Unity editor integration for importing textures and materials directly from [ambientcg.com](https://ambientcg.com/) by utilizing their [public API](https://help.ambientcg.com/04-API/API_v2.html).

## Installation

Use the Unity [Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) to install the **CC0 Textures Importer** package.

1. Open the Package Manager in `Window > Package Manager`
2. Click the add (`+`) button in the status bar
3. Select `Add package from git URL` from the add menu
4. Enter the following Git URL in the text box and click Add:

```http
https://github.com/zigurous/unity-cc0textures-importer.git
```

## Instructions

Once the package is installed, access the import menu from `Window > CC0 Textures Importer`.

Find the asset id of the desired texture pack on the CC0 Textures website. Once you select an asset on the website, you can find the id in the url. For example, the asset url `https://ambientcg.com/view?id=Wood051` has the id **Wood051**.

Enter the `Asset ID` into the text field in the Unity editor. Select your desired `Resolution` and image `Format` (be careful of large file sizes). Provide an optional `Output Name` if you want to rename the texture files from their original names. Provide an optional `Output Path` relative to the root project folder.

Click the `Import Textures` button then celebrate!

Alternatively, click the `Import Material` button if you want the textures to be automatically mapped to a new material asset. You can provide a material prefab that will be cloned when importing the new material, otherwise Unity's default standard shader material will be used.

**Note**: Textures will automatically be renamed to match the naming conventions of Unity texture maps. For example, _Color is converted to _Albedo which matches the name of the texture in the standard material editor.

## Asset Licensing

All assets by ambientCG are provided under the **Creative Commons CC0 1.0 Universal License**. Read the [license details](https://help.ambientcg.com/01-General/Licensing.html) for more information.
