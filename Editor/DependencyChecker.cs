using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DependencyChecker
{
    static DependencyChecker()
    {
        if (!IsPackageInstalled("com.dbrizov.naughtyattributes"))
        {
            Debug.LogError("NaughtyAttributes package is missing. Please add it to your project's manifest.json.");
        }
    }

    private static bool IsPackageInstalled(string packageName)
    {
        string manifestPath = "Packages/manifest.json";
        if (System.IO.File.Exists(manifestPath))
        {
            string manifestContent = System.IO.File.ReadAllText(manifestPath);
            return manifestContent.Contains(packageName);
        }
        return false;
    }
}
