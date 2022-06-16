# Asset Downloader

[![](https://img.shields.io/badge/github-repo-blue?logo=github)](https://github.com/zigurous/unity-asset-downloader) [![](https://img.shields.io/github/package-json/v/zigurous/unity-asset-downloader)](https://github.com/zigurous/unity-asset-downloader/releases) [![](https://img.shields.io/github/license/zigurous/unity-asset-downloader)](https://github.com/zigurous/unity-asset-downloader/blob/main/LICENSE.md)

The **Asset Downloader** package provides editor integration for importing textures, materials, and models directly from public web sources:

- [ambientcg.com](https://ambientcg.com/)
- [thebasemesh.com](https://thebasemesh.com/)

## Installation

Use the Unity [Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) to install the **Asset Downloader** package.

1. Open the Package Manager in `Window > Package Manager`
2. Click the add (`+`) button in the status bar
3. Select `Add package from git URL` from the add menu
4. Enter the following Git URL in the text box and click Add:

```
https://github.com/zigurous/unity-asset-downloader.git
```

## Instructions

Once the package is installed, access the import menu from `Tools > Asset Downloader`.

Choose the asset source you want to download and import assets from.

Find the id of the desired asset on the source website. Once you select an asset on the website, you can find the id in the url. For example, the asset url `https://ambientcg.com/view?id=Wood051` has the id **Wood051**.

Enter the `Asset ID` into the text field in the Unity editor. For textures, select your desired `Resolution` and image `Format` (be careful of large file sizes).

Provide an optional `Output Name` if you want to rename the files from their original names. Provide an optional `Output Path` relative to the root project folder.

Click the `Import` button to start the automatic asset download and import process.

**Note**: Textures will automatically be renamed to match the naming conventions of Unity texture maps. For example, _Color is converted to _Albedo which matches the name of the texture in the standard material editor.

## Asset Licensing

All assets by ambientCG are provided under the **Creative Commons CC0 1.0 Universal License**. Read the [license details](https://help.ambientcg.com/01-General/Licensing.html) for more information.
