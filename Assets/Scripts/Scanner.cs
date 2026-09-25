using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;


public class Scanner : MonoBehaviour
{
    [Header("Composants VR")]
    public XRGrabInteractable grabInteractable;
    public Transform raycastOrigin;
    
    [Header("Interface Utilisateur (UI)")]
    public TextMeshProUGUI itemDisplay; 
    public TextMeshProUGUI listDisplay; 
    public TextMeshProUGUI totalDisplay; 
    public Button resetButton;

    // La liste de tous les produits scannés
    private List<MarketProduct> scannedItems = new List<MarketProduct>();

    void Start()
    {
        // On récupère le composant qui permet d'attraper l'objet en VR
        if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Quand on appuie sur la gâchette (Activate), on appelle la fonction ScanItem
            grabInteractable.activated.AddListener(ScanItem);
        }
        
        // On écoute le bouton "Réinitialiser" de l'écran
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetList);
        }
    }

    void ScanItem(ActivateEventArgs args)
    {
        if (raycastOrigin == null) raycastOrigin = transform;

        // On lance un rayon (Raycast) devant le pistolet sur 5 mètres
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, 5f))
        {
            ShowLaser(hit.point); // On dessine le laser rouge

            // On vérifie si l'objet touché possède le script MarketProduct
            MarketProduct item = hit.collider.GetComponentInParent<MarketProduct>();
            if (item != null)
            {
                scannedItems.Add(item); // On l'ajoute à notre liste
                UpdateUI(item);         // On met à jour l'écran
            }
        }
        else
        {
            ShowLaser(raycastOrigin.position + raycastOrigin.forward * 5f);
        }
    }

    void ShowLaser(Vector3 endPoint)
    {
        // On crée ou récupère un LineRenderer pour afficher le laser visuellement
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;
        }
        
        lr.enabled = true;
        lr.SetPosition(0, raycastOrigin.position);
        lr.SetPosition(1, endPoint);
        
        // Le laser disparaît après 0.5 secondes
        StopAllCoroutines();
        StartCoroutine(HideLaser(lr));
    }

    IEnumerator HideLaser(LineRenderer lr)
    {
        yield return new WaitForSeconds(0.5f);
        lr.enabled = false;
    }

    public void UpdateUI(MarketProduct lastItem)
    {
        if (itemDisplay != null)
        {
            if (lastItem != null)
                itemDisplay.text = $"Produit scanné : {lastItem.nomDuProduit}\nPrix : {lastItem.prix:0.00} €";
            else
                itemDisplay.text = "Aucun produit scanné";
        }

        string listStr = "Liste de courses:\n";
        float total = 0f;
        foreach(var it in scannedItems)
        {
            listStr += $"- {it.nomDuProduit} : {it.prix:0.00} €\n";
            total += it.prix; // On calcule le sous-total
        }

        if (listDisplay != null) listDisplay.text = listStr;
        if (totalDisplay != null) totalDisplay.text = $"Sous-total: {total:0.00} €";
    }

    public void ResetList()
    {
        scannedItems.Clear(); // On vide le panier
        UpdateUI(null);
    }
}
