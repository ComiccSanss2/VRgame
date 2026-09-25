using UnityEngine;
using UnityEditor;

public class MaterialForcer : EditorWindow
{
    [MenuItem("VR/Force Apply Bakery Materials")]
    public static void ForceMaterials()
    {
        GameObject storeRoot = GameObject.Find("VR_Store");
        if (storeRoot == null)
        {
            Debug.LogError("VR_Store not found!");
            return;
        }

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        
        Texture2D bakeryTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Graphics/Low-Poly Medieval Market/Textures/Bakery market texture.tif");
        Texture2D envTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Graphics/Low-Poly Medieval Market/Textures/Environment.tif");
        Texture2D vegTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Graphics/Low-Poly Medieval Market/Textures/vegetables market.tif");

        Material newBakeryMat = new Material(urpShader);
        newBakeryMat.SetTexture("_BaseMap", bakeryTex);
        newBakeryMat.SetColor("_BaseColor", Color.white);

        Material newEnvMat = new Material(urpShader);
        newEnvMat.SetTexture("_BaseMap", envTex);
        newEnvMat.SetColor("_BaseColor", Color.white);

        Material newVegMat = new Material(urpShader);
        newVegMat.SetTexture("_BaseMap", vegTex);
        newVegMat.SetColor("_BaseColor", Color.white);

        MeshRenderer[] renderers = storeRoot.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer r in renderers)
        {
            if (r.gameObject.name == "Scanner_Gun" || r.gameObject.name == "Sign_Board" || r.gameObject.name.StartsWith("Product_")) 
                continue;

            Material matToApply = newBakeryMat;
            string objName = r.gameObject.name.ToLower();
            
            if (objName.Contains("barrel") || objName.Contains("environment"))
                matToApply = newEnvMat;
            else if (objName.Contains("box") || objName.Contains("vegetable"))
                matToApply = newVegMat;
            
            Material[] newMats = new Material[r.sharedMaterials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = matToApply;
            }
            r.sharedMaterials = newMats;
        }

        Debug.Log("Created fresh URP materials and applied them to the stand!");
    }
}
