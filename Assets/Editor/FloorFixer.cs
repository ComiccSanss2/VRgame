using UnityEngine;
using UnityEditor;


public class FloorFixer : EditorWindow
{
    [MenuItem("VR/Enable Teleportation On Floor")]
    public static void FixFloor()
    {
        // On cherche tous les objets de la scène
        MeshRenderer[] allRenderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        
        int count = 0;
        foreach (MeshRenderer r in allRenderers)
        {
            // Si c'est un objet qui ressemble à un sol (très grand, ou appelé "Plane" / "Floor")
            if (r.gameObject.name.ToLower().Contains("plane") || r.gameObject.name.ToLower().Contains("terrain") || r.gameObject.name.ToLower().Contains("floor") || r.transform.localScale.x > 5f)
            {
                // S'il n'a pas de collider, on lui en met un
                if (r.gameObject.GetComponent<Collider>() == null)
                {
                    r.gameObject.AddComponent<MeshCollider>();
                }
                
                // Si l'objet n'a pas déjà de TeleportationArea, on l'ajoute
                if (r.gameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>() == null)
                {
                    var area = r.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();
                    
                    // Optionnel : s'assurer que le Layer d'interaction est compatible avec la téléportation
                    // area.interactionLayers = 1; // Removed to keep XRI default // Default
                    count++;
                }
            }
        }
        
        Debug.Log($"Added TeleportationArea to {count} ground objects! You should now be able to teleport on them.");
    }
}
