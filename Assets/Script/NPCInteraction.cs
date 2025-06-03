using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using TMPro;




// RattachÃ© Ã  chaque objet avec lequel on peut interagir
// Script qui gÃ¨re l'interaction avec les objets
public class NPCInteraction : MonoBehaviour
{
    Outline outline;
    public UnityEvent onInteraction;
    public bool isNPC = true;


    [Header("Options PNJ")]
    [SerializeField] private Transform transformNPC;
    [SerializeField] private string npcId; 

    void Start()
    {
        outline = GetComponent<Outline>(); // on recupÃ¨re le script outline
        DisableOutline(); // et on le dÃ©sactive au dÃ©but

    }

    public void Interact()
    {
        onInteraction.Invoke(); // dÃ©clanche l'enssemble des fonction attachÃ©es Ã  onInteraction
        //NPCInteraction.TriggerDialogue()
        
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