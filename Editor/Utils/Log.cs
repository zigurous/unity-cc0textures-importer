using UnityEngine;

namespace Zigurous.AssetDownloader
{
    internal static class Log
    {
        internal static void Message(string message)
        {
            Debug.Log("[AssetDownloader] " + message);
        }

        internal static void Warning(string warning)
        {
            Debug.LogWarning("[AssetDownloader] " + warning);
        }

        internal static void Error(string error)
        {
            Debug.LogError("[AssetDownloader] " + error);
        }

    }

}
