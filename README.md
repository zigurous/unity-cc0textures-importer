# CC0 Textures Importer

This package allows you to import textures directly from [cc0textures.com](https://cc0textures.com/) into Unity by utilizing the [CC0 Textures API](https://help.cc0textures.com/doku.php?id=api_v1:start).

## Installation

The Unity Package Manager can load a package from a Git repository on a remote server.

To load a package from a Git URL:

1. Open the Package Manager window
2. Click the add (`+`) button in the status bar
3. Select **Add package from git URL** from the add menu
4. Enter the following Git URL in the text box and click Add:
   `https://github.com/zigurous/unity-cc0textures-importer.git`

## Instructions

Once the package is installed, access the import menu from `Window > CC0 Textures Importer`.

Find the asset id of the desired texture pack on the CC0 Textures website. Once you select an asset on the website, you can find the id in the url. For example, the asset url `https://cc0textures.com/view?id=Wood051` has the id **Wood051**.

Enter the `Asset ID` into the text field in the Unity editor. Select your desired `Resolution` and image `Format` (be careful of large file sizes). Provide an optional `Output Name` if you want to rename the texture files from their original names. Provide an optional `Output Path` relative to the root project folder.

Click the `Import Textures` button then celebrate!

Alternatively, click the `Import Material` button if you want the textures to be automatically mapped to a new material asset. You can provide a material prefab that will be cloned when importing the new material, otherwise Unity's default standard shader material will be used.

**Note**: Textures will automatically be renamed to match the naming conventions of Unity texture maps. For example, _Color is converted to _Albedo which matches the name of the texture in the standard material editor.

## Asset Licensing

All assets by CC0 Textures are provided under the **Creative Commons CC0 License** (public domain). Read the [license details](https://help.cc0textures.com/doku.php?id=website:license) for more information.
