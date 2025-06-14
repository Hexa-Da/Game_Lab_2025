using UnityEngine;
using TMPro;

// RattachÃ© au canvas d'interaction
// Script qui gÃ¨re le text a afficher en fonction de l'objet avec lequel on veux interagir
public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    private TMP_Text interactionText; 
    
    [Header("Messages")]
    [SerializeField] private string pickupMessage = "Ramasser (F)";
    [SerializeField] private string talkMessage = "Parler (F)";
    [SerializeField] private string teleportMessage = "Ouvrir (F)";
    
    [Header("Position")]
    [SerializeField] private float pickupOffset = 1.0f; // 1 unitÃ© monde au-dessus de l'objet
    [SerializeField] private float npcOffset = 2.0f; // 2 unitÃ©s monde au-dessus du NPC

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

    // on active l'UI d'interaction en vÃ©rifiant si c'est un pnj ou un objet ramassable
    public void EnableInteraction(Vector3 worldPosition, bool isNPC, bool isTeleport)
    {   
        interactionUI.SetActive(true);
    
        // on change le texte en fonction de l'interaction
        interactionText.text = isNPC ? talkMessage : isTeleport ? teleportMessage : pickupMessage;
        
        // Configurer le RectTransform pour un positionnement correct
        RectTransform rectTransform = interactionUI.GetComponent<RectTransform>();
        
        if (rectTransform != null)
        {
            // Positionner directement en coordonnÃ©es monde avec l'offset appropriÃ©
            rectTransform.position = new Vector3(
                worldPosition.x,
                worldPosition.y + (isNPC ? npcOffset : pickupOffset), // Utiliser l'offset appropriÃ©
                worldPosition.z
            );

        }
    }
}