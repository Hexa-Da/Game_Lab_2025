using UnityEngine;
using TMPro;

// Rattaché au canvas d'interaction
// Script qui gère le text a afficher en fonction de l'objet avec lequel on veux interagir
public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    private TMP_Text interactionText; 
    
    [Header("Messages")]
    [SerializeField] private string pickupMessage = "Ramasser (F)";
    [SerializeField] private string talkMessage = "Parler (F)";
    
    [Header("Position")]
    [SerializeField] private float pickupOffset = 100f;
    [SerializeField] private float npcOffset = 220f; // Offset plus élevé pour les PNJ

    private void Awake()
    {
        instance = this;
        // on va chercher le formatage du texte dans l'UI
        interactionText = interactionUI.GetComponentInChildren<TMP_Text>();
    }

    public void DisableInteraction()
    {
        interactionUI.SetActive(false);
    }

    // on active l'UI d'interaction en vérifiant si c'est un pnj ou un objet ramassable
    public void EnableInteraction(Vector3 worldPosition, bool isNPC)
    {   
        interactionUI.SetActive(true);

        // on change le texte en fonction de l'interaction
        interactionText.text = isNPC ? talkMessage : pickupMessage;

        // Convertir la position monde en position écran
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // Utiliser un offset différent selon le type d'objet
        screenPosition.y += isNPC ? npcOffset : pickupOffset;
        
        interactionUI.transform.position = screenPosition;
    }
}