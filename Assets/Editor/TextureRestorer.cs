using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextureRestorer : EditorWindow
{
    [MenuItem("VR/Restore Medieval Textures")]
    public static void RestoreTextures()
    {
        Dictionary<string, string> matToTex = new Dictionary<string, string>()
        {
            { "Bakery matket", "Bakery market texture" },
            { "Environment", "Environment" },
            { "fish_market", "fish_market" },
            { "Laterns", "Latens" },
            { "Meat market", "Meat market texture" },
            { "Vegetables market", "vegetables market" },
            { "Weapon market", "Weapon market" }
        };

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null)
        {
            Debug.LogError("URP Lit shader not found!");
            return;
        }

        foreach (var kvp in matToTex)
        {
            string matPath = "Assets/Graphics/Low-Poly Medieval Market/Materials/" + kvp.Key + ".mat";
            string texPath = "Assets/Graphics/Low-Poly Medieval Market/Textures/" + kvp.Value + ".tif";

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

            if (mat != null)
            {
                mat.shader = urpShader;
                
                if (tex != null)
                {
                    mat.SetTexture("_BaseMap", tex);
                    mat.SetColor("_BaseColor", Color.white);
                    Debug.Log("Restored texture for: " + mat.name);
                }
                else
                {
                    Debug.LogWarning("Could not find texture at: " + texPath);
                }
                EditorUtility.SetDirty(mat);
            }
            else
            {
                Debug.LogWarning("Could not find material at: " + matPath);
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("All medieval textures restored!");
    }
}
