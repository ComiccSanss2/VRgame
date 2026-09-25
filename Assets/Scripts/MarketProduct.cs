using UnityEngine;

/// <summary>
/// Script à expliquer au professeur :
/// Ce script est attaché à chaque produit du magasin (Pomme, Fromage, etc.).
/// Il stocke les données du produit pour que le scanner puisse les lire.
/// </summary>
public class MarketProduct : MonoBehaviour
{
    [Header("Informations du Produit")]
    [Tooltip("Le nom affiché sur l'écran de la caisse")]
    public string nomDuProduit = "Produit Inconnu";
    
    [Tooltip("Le prix en euros")]
    public float prix = 0.0f;

    void Start()
    {
        // On s'assure que le produit a bien un composant de physique pour pouvoir tomber dans le panier
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Le produit est soumis à la gravité
        rb.useGravity = true;
    }
}
