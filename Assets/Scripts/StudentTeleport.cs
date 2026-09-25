using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Script à expliquer au professeur :
/// Ce script personnalisé gère la logique de téléportation. 
/// Il surveille l'axe Y du joystick gauche et déclenche une requête de téléportation 
/// auprès du TeleportationProvider lorsque le joueur relâche le joystick.
/// </summary>
public class StudentTeleport : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Le composant qui gère le déplacement du joueur")]
    public TeleportationProvider provider;
    
    [Tooltip("Le rayon laser qui pointe vers la destination")]
    public XRRayInteractor rayInteractor;
    
    private InputAction leftThumbstickAction;
    private bool isAiming = false;

    void Awake()
    {
        // Création dynamique de l'action pour lire le joystick gauche
        leftThumbstickAction = new InputAction(
            name: "LeftThumbstickTeleport",
            type: InputActionType.Value,
            binding: "<XRController>{LeftHand}/thumbstick"
        );
        leftThumbstickAction.Enable();

        // Recherche automatique des composants s'ils ne sont pas assignés
        if (provider == null) provider = Object.FindAnyObjectByType<TeleportationProvider>();
        if (rayInteractor == null) rayInteractor = GetComponent<XRRayInteractor>();
    }

    void OnDestroy()
    {
        if (leftThumbstickAction != null)
        {
            leftThumbstickAction.Disable();
            leftThumbstickAction.Dispose();
        }
    }

    void Update()
    {
        if (provider == null || rayInteractor == null) return;

        // Lecture du joystick (Y = haut/bas)
        float y = leftThumbstickAction.ReadValue<Vector2>().y;

        // 1. Le joueur pousse le joystick vers le haut
        if (y > 0.5f && !isAiming)
        {
            isAiming = true;
            rayInteractor.enabled = true; // On active le composant Ray Interactor
            
            // On s'assure que le LineRenderer est visible
            var lineVisual = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>();
            if (lineVisual != null) lineVisual.enabled = true;
        }
        // 2. Le joueur relâche le joystick (déclenchement de la téléportation)
        else if (y < 0.2f && isAiming)
        {
            isAiming = false;
            
            // On vérifie si le rayon pointe vers un sol valide (Teleportation Area)
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                TeleportRequest request = new TeleportRequest()
                {
                    destinationPosition = hit.point,
                };
                
                // On envoie la requête de téléportation au système central
                provider.QueueTeleportRequest(request);
            }
            
            // On cache le rayon
            rayInteractor.enabled = false;
            var lineVisual = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>();
            if (lineVisual != null) lineVisual.enabled = false;
        }
    }
}
