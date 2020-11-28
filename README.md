# CC0Textures Importer

This package allows you to import textures directly from [CC0Textures.com](https://cc0textures.com/) into Unity. This is made possible by utilizing the [CC0 Textures API](https://help.cc0textures.com/doku.php?id=api_v1:start).

## Instructions

Once the package is installed, access the import menu from `Window > CC0Textures Importer`.

Find the asset id of the desired texture pack on the CC0 Textures website. Once you select an asset on the website, you can find the id in the url. For example, the asset url `https://cc0textures.com/view?id=Wood051` has the id **Wood051**.

Enter the `Asset ID` into the text field in the Unity editor. Select your desired `Resolution` and file `Format` (be careful of large file sizes). Provide an optional `Output Name` if you want to rename the texture files from their original names. Provide an optional `Output Path`, relative to the `/Assets` directory.

Click the `Import` button then wait for your textures to be imported!

**Note**: Textures will automatically be renamed to match the naming conventions of Unity texture maps. For example _Color is converted to _Albedo.

## Asset Licensing

All assets by CC0 Textures are provided under the **Creative Commons CC0 License** (public domain). Read the [license details](https://help.cc0textures.com/doku.php?id=website:license) for more information.
