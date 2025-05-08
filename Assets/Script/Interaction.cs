using UnityEngine;
using UnityEngine.Events;


// Rattaché à chaque objet avec lequel on peut interagir
// Script qui gère l'interaction avec les objets
public class Interaction : MonoBehaviour
{
    Outline outline;
    public UnityEvent onInteraction;

    private AudioSource m_AudioSource;
    
    [Header("Options")]
    public bool isPickup = false; // vrai pour les objets ramassables
    public bool isNPC = false;    // vrai pour les pnj

    [Header("Options PNJ")]
    [SerializeField] private Transform transformNPC;
    [SerializeField] private string npcId; 

    void Start()
    {
        outline = GetComponent<Outline>(); // on recupère le script outline
        DisableOutline(); // et on le désactive au début
    }

    public void Interact()
    {


        if (isPickup)
        {
            MenuInGame.instance.UpdateUI();
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.Play();
        }
        onInteraction.Invoke(); // déclanche l'enssemble des fonction attachées à onInteraction
        // setActive(false) pour les objets ramassables
        // TriggerDialogue() pour les pnj
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
}