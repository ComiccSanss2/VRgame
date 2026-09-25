using UnityEngine;
using UnityEditor;

public class ShaderFixer : Editor
{
    [MenuItem("VR/Fix Materials (Shader)")]
    public static void FixShaders()
    {
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Graphics/Low-Poly Medieval Market" });
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");

        if (urpShader == null)
        {
            Debug.LogError("URP Lit shader not found!");
            return;
        }

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (mat != null)
            {
                Texture baseMap = null;
                if (mat.HasProperty("_MainTex")) baseMap = mat.GetTexture("_MainTex");
                else if (mat.HasProperty("_BaseMap")) baseMap = mat.GetTexture("_BaseMap");

                mat.shader = urpShader;
                
                if (baseMap != null)
                {
                    mat.SetTexture("_BaseMap", baseMap);
                    mat.SetColor("_BaseColor", Color.white);
                }
                
                EditorUtility.SetDirty(mat);
                Debug.Log("Fixed shader for: " + mat.name);
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("All materials fixed! They now use the Standard shader.");
    }
}
