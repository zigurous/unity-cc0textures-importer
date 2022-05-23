using UnityEngine;

namespace Zigurous.AssetDownloader
{
    internal static class Log
    {
        internal static void Message(string message)
        {
            Debug.Log("[CC0TexturesImporter] " + message);
        }

        internal static void Warning(string warning)
        {
            Debug.LogWarning("[CC0TexturesImporter] " + warning);
        }

        internal static void Error(string error)
        {
            Debug.LogError("[CC0TexturesImporter] " + error);
        }

    }

}
