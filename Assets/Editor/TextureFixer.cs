using UnityEngine;
using UnityEditor;

public class TextureFixer : Editor
{
    [MenuItem("VR/Fix Low-Poly Textures")]
    public static void FixTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Graphics/Low-Poly Medieval Market/Textures" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    changed = true;
                }
                
                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    changed = true;
                }
                
                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }
                
                if (changed)
                {
                    importer.SaveAndReimport();
                    Debug.Log("Fixed texture settings for: " + path);
                }
            }
        }
        
        Debug.Log("Texture fix complete!");
    }
}
