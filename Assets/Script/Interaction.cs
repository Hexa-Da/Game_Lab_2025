using UnityEngine;
using UnityEngine.Events;
<<<<<<< Updated upstream
using UnityEngine.UI; // Added for Sprite type
=======
using UnityEngine.Rendering.Universal;
using TMPro;

>>>>>>> Stashed changes

// Rattaché à chaque objet avec lequel on peut interagir
// Script qui gère l'interaction avec les objets
public class Interaction : MonoBehaviour
{
    Outline outline;
    public UnityEvent onInteraction;
    private AudioSource m_AudioSource;
<<<<<<< Updated upstream
    
    [Header("Options")]
    public bool isPickup = false; // vrai pour les objets ramassables
=======
    private GameObject m_BlurGameObject;
    private Vignette m_Vignette;
    private ChromaticAberration m_ChromaticAberration;
    private CustomVolumeComponent m_BlurVolume;

    
    [Header("Options")]
    public bool isFragment = false; // vrai pour les fragments, false pour les batons
    public bool isStick = false; // vrai pour les batons, false pour les fragments
>>>>>>> Stashed changes
    public bool isNPC = false;    // vrai pour les pnj

    [Header("Options PNJ")]
    [SerializeField] private Transform transformNPC;
    [SerializeField] private string npcId; 

<<<<<<< Updated upstream
    [Header("Pickup Options")]
    [SerializeField] private Sprite spriteDeLObjet; // Added sprite field for pickup items
    private InventoryManager inventoryManager; // Added inventory manager reference
=======
    [Header("UI Compteurs")]
    private static int stickCount = 0;
    private static int fragmentCount = 0;
>>>>>>> Stashed changes

    void Start()
    {
        outline = GetComponent<Outline>(); // on recupère le script outline
        DisableOutline(); // et on le désactive au début
<<<<<<< Updated upstream
        inventoryManager = FindObjectOfType<InventoryManager>(); // Find the inventory manager
=======
        m_BlurGameObject = GameObject.Find("BlurVolume"); // o recupere le component qui gere les effets de flou
        m_ChromaticAberration = m_BlurGameObject.GetComponent<ChromaticAberration>();
        m_BlurVolume = m_BlurGameObject.GetComponent<CustomVolumeComponent>();
        m_Vignette = m_BlurGameObject.GetComponent<Vignette>();
>>>>>>> Stashed changes
    }

    public void Interact()
    {
<<<<<<< Updated upstream


        if (isPickup)
        {
            MenuInGame.instance.UpdateUI();
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.Play();
            inventoryManager.AddItem(spriteDeLObjet);
        }
        onInteraction.Invoke(); // déclanche l'enssemble des fonction attachées à onInteraction
        // setActive(false) pour les objets ramassables
        // TriggerDialogue() pour les pnj
=======
        if (isFragment)
        {
            fragmentCount++;
        }
        else if (isStick)
        {
            stickCount++;
        }
        MenuInGame.instance.UpdateUI(stickCount, fragmentCount);
        onInteraction.Invoke();
>>>>>>> Stashed changes
    }

    public void TriggerDialogue()
    {
        // fait apparaitre la bulle de dialogue au dessus du pnj
        DialogueManager.instance.StartDialogue(npcId, transformNPC);
    }

    public void DisableOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void EnableOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }
<<<<<<< Updated upstream
=======

    public void UpdateBlur()
    {
        Debug.Log("La il adapte le flou normalement");
    //Pour ajuster le flou il faut:
    // - Relever la valeur du compteur de fragment
    // - Deduire les valeurs à donner à chaque paramètre des effets (On a pas encore choisi les valeurs)
    // - update les valeurs
    
    }
>>>>>>> Stashed changes
}