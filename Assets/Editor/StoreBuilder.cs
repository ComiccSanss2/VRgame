using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;

public class StoreBuilder : EditorWindow
{
    [MenuItem("VR/Build Store In Scene")]
    public static void BuildStore()
    {
        // 1. Root Object
        GameObject storeRoot = GameObject.Find("VR_Store");
        if (storeRoot != null) DestroyImmediate(storeRoot);
        storeRoot = new GameObject("VR_Store");

        // 2. Main Store Stand (BakeryMarket)
        GameObject standPrefab = null;
        string[] standGuids = AssetDatabase.FindAssets("BakeryMarket t:GameObject");
        foreach(string guid in standGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("Low-Poly Medieval Market"))
            {
                standPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                break;
            }
        }
        
        GameObject stand;
        if (standPrefab != null)
        {
            stand = (GameObject)PrefabUtility.InstantiatePrefab(standPrefab);
        }
        else
        {
            stand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stand.transform.localScale = new Vector3(4, 3, 3);
        }
        stand.name = "Market_Stand";
        stand.transform.SetParent(storeRoot.transform);
        stand.transform.position = new Vector3(0, 0, 0);
        stand.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Add MeshColliders to everything in the stand so items don't fall through!
        MeshFilter[] standMeshes = stand.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in standMeshes)
        {
            if (mf.gameObject.GetComponent<Collider>() == null)
            {
                mf.gameObject.AddComponent<MeshCollider>();
            }
        }

        Rigidbody[] standRbs = stand.GetComponentsInChildren<Rigidbody>();
        foreach (var rbProp in standRbs) rbProp.isKinematic = true;

        // 3. TELEPORTATION FLOOR
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Teleport_Floor";
        floor.transform.SetParent(storeRoot.transform);
        floor.transform.position = new Vector3(0, 0.01f, 0); // slightly above 0 to avoid z-fighting with their plane
        floor.transform.localScale = new Vector3(10, 1, 10); // 100x100 meters
        
        // Hide the floor visually so we just see their nice green ground, but keep the collider!
        floor.GetComponent<MeshRenderer>().enabled = false;
        
        // Add TeleportationArea so the player can teleport everywhere in the market
        floor.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();

        // 4. Products (Grabbables)
        string[] productNames = { "Food_Apple", "Food_Cheese", "Food_Meatloaf", "Food_Pizza", "Food_Taco" };
        float[] prices = { 1.50f, 4.99f, 6.50f, 8.00f, 5.00f };
        string[] displayNames = { "Pomme", "Fromage", "Pain de Viande", "Pizza", "Taco" };

        for (int i = 0; i < productNames.Length; i++)
        {
            GameObject prodPrefab = null;
            string[] guids = AssetDatabase.FindAssets(productNames[i] + " t:GameObject");
            foreach(string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Pandazole") || path.Contains("Food_"))
                {
                    prodPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    break;
                }
            }

            GameObject prod;
            if (prodPrefab != null)
            {
                prod = (GameObject)PrefabUtility.InstantiatePrefab(prodPrefab);
            }
            else
            {
                prod = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                prod.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            
            prod.name = "Product_" + displayNames[i];
            prod.transform.SetParent(storeRoot.transform);
            
            // Spawn above the middle hanging table
            prod.transform.position = new Vector3(-0.6f + (i * 0.3f), 1.3f, -0.5f);
            
            if (prod.GetComponent<Collider>() == null) prod.AddComponent<BoxCollider>();
            
            Rigidbody pRb = prod.GetComponent<Rigidbody>();
            if (pRb == null) pRb = prod.AddComponent<Rigidbody>();
            pRb.isKinematic = false; 
            
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable prodGrab = prod.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (prodGrab == null) prodGrab = prod.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            prodGrab.interactionLayers = 3; // Layer 0 (Grab) + Layer 1 (Socket)
            
            MarketProduct info = prod.AddComponent<MarketProduct>();
            info.nomDuProduit = displayNames[i];
            info.prix = prices[i];
        }

        // 5. Shopping Basket
        GameObject basketPrefab = null;
        string[] basketGuids = AssetDatabase.FindAssets("basket_01 t:GameObject");
        foreach(string guid in basketGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("Low-Poly Medieval Market"))
            {
                basketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                break;
            }
        }
        
        GameObject basket;
        if (basketPrefab != null)
        {
            basket = (GameObject)PrefabUtility.InstantiatePrefab(basketPrefab);
            basket.name = "ShoppingBasket";
            // Place on the right hanging table
            basket.transform.position = new Vector3(1.3f, 1.3f, -0.5f);
            Rigidbody[] basketRbs = basket.GetComponentsInChildren<Rigidbody>();
            foreach(var rb in basketRbs) rb.isKinematic = false; 
        }
        else
        {
            basket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basket.transform.position = new Vector3(1.3f, 1.3f, -0.5f);
            basket.transform.localScale = new Vector3(0.5f, 0.2f, 0.5f);
        }

        basket.AddComponent<BasketManager>();
        basket.transform.SetParent(storeRoot.transform);

        // 6. UI Canvas for Scanner (Behind left table)
        GameObject canvasObj = new GameObject("StoreUI");
        canvasObj.transform.SetParent(storeRoot.transform);
        canvasObj.transform.position = new Vector3(-1.3f, 1.6f, -0.2f);
        canvasObj.transform.rotation = Quaternion.Euler(0, 0, 0); // facing -Z
        canvasObj.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f); 
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
        
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.8f);
        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 300);

        GameObject lastItemObj = new GameObject("LastItemText");
        lastItemObj.transform.SetParent(bg.transform, false);
        lastItemObj.transform.localPosition = new Vector3(0, 100, 0);
        TextMeshProUGUI itemText = lastItemObj.AddComponent<TextMeshProUGUI>();
        itemText.text = "Scannez un article";
        itemText.fontSize = 24;
        itemText.alignment = TextAlignmentOptions.Center;
        itemText.color = Color.white;
        lastItemObj.GetComponent<RectTransform>().sizeDelta = new Vector2(380, 80);

        GameObject listObj = new GameObject("ListText");
        listObj.transform.SetParent(bg.transform, false);
        listObj.transform.localPosition = new Vector3(0, 0, 0);
        TextMeshProUGUI listText = listObj.AddComponent<TextMeshProUGUI>();
        listText.text = "Liste de courses:";
        listText.fontSize = 18;
        listText.alignment = TextAlignmentOptions.TopLeft;
        listText.color = Color.white;
        listObj.GetComponent<RectTransform>().sizeDelta = new Vector2(380, 150);

        GameObject totalObj = new GameObject("TotalText");
        totalObj.transform.SetParent(bg.transform, false);
        totalObj.transform.localPosition = new Vector3(0, -90, 0);
        TextMeshProUGUI totalText = totalObj.AddComponent<TextMeshProUGUI>();
        totalText.text = "Sous-total: 0.00 Euro";
        totalText.fontSize = 22;
        totalText.alignment = TextAlignmentOptions.Right;
        totalText.color = Color.yellow;
        totalObj.GetComponent<RectTransform>().sizeDelta = new Vector2(380, 40);

        GameObject resetBtnObj = new GameObject("ResetButton");
        resetBtnObj.transform.SetParent(bg.transform, false);
        resetBtnObj.transform.localPosition = new Vector3(0, -130, 0);
        Image btnImg = resetBtnObj.AddComponent<Image>();
        btnImg.color = Color.red;
        Button resetBtn = resetBtnObj.AddComponent<Button>();
        resetBtnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 40);
        
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(resetBtnObj.transform, false);
        TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "Reinitialiser";
        btnText.fontSize = 18;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        btnTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 40);

        // 7. Scanner Object
        GameObject scannerGunPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Graphics/PolygonStarter/Prefabs/SM_Wep_Watergun_01.prefab");
        GameObject scannerGun;
        if (scannerGunPrefab != null)
        {
            scannerGun = (GameObject)PrefabUtility.InstantiatePrefab(scannerGunPrefab);
            scannerGun.transform.position = new Vector3(-1.3f, 1.2f, -0.6f); // Left table
            scannerGun.transform.rotation = Quaternion.Euler(0, 0, 0); 
        }
        else
        {
            scannerGun = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scannerGun.transform.position = new Vector3(-1.3f, 1.2f, -0.6f); 
            scannerGun.transform.localScale = new Vector3(0.1f, 0.1f, 0.2f);
            Material scanMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            scanMat.SetColor("_BaseColor", Color.blue);
            scannerGun.GetComponent<MeshRenderer>().sharedMaterial = scanMat;
        }
        
        scannerGun.name = "Scanner_Gun";
        scannerGun.transform.SetParent(storeRoot.transform);
        
        if (scannerGun.GetComponent<Collider>() == null) scannerGun.AddComponent<BoxCollider>();
        
        var scannerRb = scannerGun.GetComponent<Rigidbody>();
        if (scannerRb == null) scannerRb = scannerGun.AddComponent<Rigidbody>();
        var grab = scannerGun.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.interactionLayers = 1; // Strict TP Rule: Cannot go into basket (which accepts Layer 2)
        
        Scanner scannerScript = scannerGun.AddComponent<Scanner>();
        scannerScript.grabInteractable = grab;
        scannerScript.itemDisplay = itemText;
        scannerScript.listDisplay = listText;
        scannerScript.totalDisplay = totalText;
        scannerScript.resetButton = resetBtn;
        
        GameObject origin = new GameObject("Origin");
        origin.transform.SetParent(scannerGun.transform, false);
        origin.transform.localPosition = new Vector3(0, 0, 0.6f);
        scannerScript.raycastOrigin = origin.transform;

        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>();
        }

        // 8. Signalétique (Signboard)
        GameObject signObj = new GameObject("Store_Sign");
        signObj.transform.SetParent(storeRoot.transform);
        signObj.transform.position = new Vector3(0, 3.2f, -1.2f); 
        signObj.transform.rotation = Quaternion.Euler(0, 0, 0); // Text faces -Z correctly now!
        TextMesh signText = signObj.AddComponent<TextMesh>();
        signText.text = "BIENVENUE AU MARCHE VR";
        signText.characterSize = 0.05f;
        signText.fontSize = 120;
        signText.color = Color.white;
        signText.anchor = TextAnchor.MiddleCenter;
        signText.alignment = TextAlignment.Center;
        
        GameObject signBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        signBoard.name = "Sign_Board";
        signBoard.transform.SetParent(signObj.transform);
        signBoard.transform.localPosition = new Vector3(0, 0, 0.1f);
        signBoard.transform.localScale = new Vector3(2f, 0.5f, 0.1f);
        Material boardMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        boardMat.SetColor("_BaseColor", new Color(0.2f, 0.1f, 0.05f)); 
        signBoard.GetComponent<MeshRenderer>().sharedMaterial = boardMat;

        // 9. Deco additions (Bonus constraint)
        SpawnDecor("wooden_box t:GameObject", new Vector3(-2.2f, 0.5f, -1.0f));
        SpawnDecor("bag_of_gold t:GameObject", new Vector3(0.8f, 1.3f, -0.5f));
        SpawnDecor("wicker_bottle t:GameObject", new Vector3(1.0f, 1.3f, -0.5f));

        // 10. FORCE APPLY COLORS (Fixing the gray bug)
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        Texture2D bakeryTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Graphics/Low-Poly Medieval Market/Textures/Bakery market texture.tif");
        Texture2D envTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Graphics/Low-Poly Medieval Market/Textures/Environment.tif");
        Texture2D vegTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Graphics/Low-Poly Medieval Market/Textures/vegetables market.tif");
        
        Material newBakeryMat = new Material(urpShader);
        if (bakeryTex != null) { newBakeryMat.SetTexture("_BaseMap", bakeryTex); newBakeryMat.SetColor("_BaseColor", Color.white); }
        Material newEnvMat = new Material(urpShader);
        if (envTex != null) { newEnvMat.SetTexture("_BaseMap", envTex); newEnvMat.SetColor("_BaseColor", Color.white); }
        Material newVegMat = new Material(urpShader);
        if (vegTex != null) { newVegMat.SetTexture("_BaseMap", vegTex); newVegMat.SetColor("_BaseColor", Color.white); }

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
            else if (objName.Contains("bottle") || objName.Contains("basket"))
                matToApply = newBakeryMat;
            
            Material[] newMats = new Material[r.sharedMaterials.Length];
            for (int j = 0; j < newMats.Length; j++)
            {
                newMats[j] = matToApply;
            }
            r.sharedMaterials = newMats;
        }

        Debug.Log("Store generation complete! Improved VR Store was built in the current scene.");
    }

    private static void SpawnDecor(string searchStr, Vector3 pos)
    {
        string[] guids = AssetDatabase.FindAssets(searchStr);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                GameObject decor = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                decor.transform.position = pos;
                GameObject storeRoot = GameObject.Find("VR_Store");
                if (storeRoot != null) decor.transform.SetParent(storeRoot.transform);
            }
        }
    }
}
