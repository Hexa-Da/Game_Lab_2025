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
    [SerializeField] private float pickupOffset = 1.0f; // 1 unité monde au-dessus de l'objet
    [SerializeField] private float npcOffset = 2.0f; // 2 unités monde au-dessus du NPC

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
        
        // Configurer le RectTransform pour un positionnement correct
        RectTransform rectTransform = interactionUI.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Positionner directement en coordonnées monde avec l'offset approprié
            rectTransform.position = new Vector3(
                worldPosition.x,
                worldPosition.y + (isNPC ? npcOffset : pickupOffset), // Utiliser l'offset approprié
                worldPosition.z
            );
        }
    }
}