using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class ForceTeleportSetup : EditorWindow
{
    [MenuItem("VR/Force Setup Teleport Script")]
    public static void SetupTeleport()
    {
        // 1. Trouver le contrôleur gauche
        GameObject leftController = GameObject.Find("Left Controller");
        if (leftController == null)
        {
            Debug.LogError("Left Controller introuvable !");
            return;
        }

        // 2. Trouver ou ajouter le composant StudentTeleport
        StudentTeleport studentTp = leftController.GetComponent<StudentTeleport>();
        if (studentTp == null)
        {
            studentTp = leftController.AddComponent<StudentTeleport>();
        }

        // 3. Assigner le Ray Interactor
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor = leftController.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        if (rayInteractor == null)
        {
            rayInteractor = leftController.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        }
        studentTp.rayInteractor = rayInteractor;

        // On s'assure que le rayon est bien configuré pour la téléportation
        // Layer 31 est souvent utilisé pour le Teleport dans XRI, ou sinon on garde le masque
        rayInteractor.interactionLayers = 1; // Default
        
        // 4. Assigner le TeleportationProvider
        TeleportationProvider provider = Object.FindAnyObjectByType<TeleportationProvider>();
        if (provider == null)
        {
            provider = leftController.AddComponent<TeleportationProvider>();
        }
        studentTp.provider = provider;

        // Optionnel : Désactiver le rayon au démarrage pour qu'il ne s'affiche que quand on pousse le joystick
        rayInteractor.enabled = false;

        Debug.Log("StudentTeleport a ete configure de force sur le Left Controller !");
    }
}
