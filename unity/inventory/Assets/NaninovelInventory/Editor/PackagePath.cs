using System.IO;
using System.Linq;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// Provides paths to various package-related folders and resources. All the returned paths are in absolute format.
    /// </summary>
    public static class PackagePath
    {
        public static string PackageRootPath => GetPackageRootPath();
        public static string PackageMarkerPath => Path.Combine(cachedPackageRootPath, markerSearchPattern); 
        public static string PrefabsPath => Path.Combine(PackageRootPath, "Prefabs");

        private const string markerSearchPattern = "Elringus.NaninovelInventory.Editor.asmdef";
        private static string cachedPackageRootPath;

        private static string GetPackageRootPath ()
        {
            if (string.IsNullOrEmpty(cachedPackageRootPath) || !File.Exists(PackageMarkerPath))
            {
                var marker = Directory.GetFiles(Application.dataPath, markerSearchPattern, SearchOption.AllDirectories).FirstOrDefault();
                if (marker is null) { Debug.LogError($"Failed to find `{markerSearchPattern}` file."); return null; }
                cachedPackageRootPath = Directory.GetParent(marker)?.Parent?.FullName;
            }
            return cachedPackageRootPath;
        }
    }
}
