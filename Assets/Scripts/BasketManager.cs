using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Script à expliquer au professeur :
/// Gère le panier de courses. Il crée dynamiquement plusieurs emplacements (Sockets) 
/// à l'intérieur du panier pour y déposer plusieurs objets en même temps.
/// </summary>
public class BasketManager : MonoBehaviour
{
    [Header("Configuration des Sockets")]
    public float espacement = 0.15f; 
    public float hauteur = 0.1f;    
    public int interactionLayer = 2; // Réserve ces emplacements aux produits uniquement

    void Start()
    {
        Vector3[] positions = {
            new Vector3(0, hauteur, 0),                       // Centre
            new Vector3(espacement, hauteur, 0),              // Droite
            new Vector3(-espacement, hauteur, 0),             // Gauche
            new Vector3(0, hauteur, espacement),              // Devant
            new Vector3(0, hauteur, -espacement)              // Derrière
        };

        for (int i = 0; i < positions.Length; i++)
        {
            CreerEmplacement("Emplacement_" + i, positions[i]);
        }
    }

    private void CreerEmplacement(string nom, Vector3 positionLocale)
    {
        GameObject socketObj = new GameObject(nom);
        socketObj.transform.SetParent(this.transform);
        socketObj.transform.localPosition = positionLocale;
        
        SphereCollider collider = socketObj.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.1f; 
        
        XRSocketInteractor socket = socketObj.AddComponent<XRSocketInteractor>();
        socket.interactionLayers = interactionLayer; 
    }
}
