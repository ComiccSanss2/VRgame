using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRFixer : EditorWindow
{
    [MenuItem("VR/Fix Player Setup")]
    public static void FixVR()
    {
        // 1. Remove existing cameras that might conflict
        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            if (cam.gameObject.name == "Main Camera")
            {
                DestroyImmediate(cam.gameObject);
            }
        }

        // 2. Remove broken XR Origins
        var existingOrigins = Object.FindObjectsByType<Unity.XR.CoreUtils.XROrigin>(FindObjectsSortMode.None);
        foreach (var origin in existingOrigins)
        {
            DestroyImmediate(origin.gameObject);
        }

        // Also remove older XR Rig if it exists
        var oldRigs = GameObject.Find("XR Origin (VR)");
        if (oldRigs != null) DestroyImmediate(oldRigs);
        
        var oldRig2 = GameObject.Find("XR Rig");
        if (oldRig2 != null) DestroyImmediate(oldRig2);

        // 3. Instantiate the working XR Origin from Starter Assets
        string prefabPath = "Assets/Samples/XR Interaction Toolkit/3.4.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";
        GameObject originPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (originPrefab != null)
        {
            GameObject newOrigin = (GameObject)PrefabUtility.InstantiatePrefab(originPrefab);
            newOrigin.name = "XR Origin (XR Rig)";
            newOrigin.transform.position = new Vector3(0, 0, 0); // Center of the store
            
            // STRICT TP RULE: Disable Continuous Movement!
            Component[] allComponents = newOrigin.GetComponentsInChildren<Component>(true);
            foreach (Component comp in allComponents)
            {
                if (comp != null && comp.GetType().Name.Contains("ContinuousMove"))
                {
                    DestroyImmediate(comp);
                }
            }
        }
        else
        {
            Debug.LogError("XR Origin prefab not found at " + prefabPath);
        }

        // 4. Ensure XR Interaction Manager and INPUT ACTION MANAGER (Crucial for Teleportation!)
        GameObject interactionManagerObj = GameObject.Find("XR Interaction Manager");
        if (interactionManagerObj == null)
        {
            interactionManagerObj = new GameObject("XR Interaction Manager");
            interactionManagerObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
        }
        
        if (interactionManagerObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager>() == null)
        {
            var inputManager = interactionManagerObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager>();
            var actionAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/Samples/XR Interaction Toolkit/3.4.1/Starter Assets/XRI Default Input Actions.inputactions");
            if (actionAsset != null)
            {
                inputManager.actionAssets = new System.Collections.Generic.List<UnityEngine.InputSystem.InputActionAsset> { actionAsset };
            }
            else
            {
                Debug.LogError("Could not find XRI Default Input Actions!");
            }
        }

        // 5. Ensure Event System for UI
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>();
        }

        // 6. Nuke Simulator Hack (Because XR Device Simulator overrides physical headset)
        GameObject nukeSimulator = new GameObject("Nuke_Simulator_Hack");
        nukeSimulator.AddComponent<SimulatorNuker>();

        Debug.Log("VR Setup Fixed! Replaced broken camera/rig with the official working XR Origin.");
    }
}

public class SimulatorNuker : MonoBehaviour
{
    void Update()
    {
        // 1. Continuously search for XR Device Simulator and destroy it if Unity spawns it
        var simulator = Object.FindAnyObjectByType<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator>();
        if (simulator != null)
        {
            Destroy(simulator.gameObject);
            Debug.Log("Destroyed XR Device Simulator to allow physical headset tracking!");
        }

        // 2. FORCE DISABLE CONTINUOUS MOVEMENT (TP RULE)
        // Find all LocomotionProviders in the scene
        var allMovers = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var mover in allMovers)
        {
            string typeName = mover.GetType().Name;
            // Catch any continuous move provider (DynamicMoveProvider, ActionBasedContinuousMoveProvider, etc)
            if (typeName.Contains("MoveProvider") && !typeName.Contains("Teleport"))
            {
                if (mover.enabled)
                {
                    mover.enabled = false;
                    Debug.Log("Disabled illegal continuous movement: " + typeName);
                }
            }
        }
    }
}
