using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using TMPro;


// Rattaché à chaque objet avec lequel on peut interagir
// Script qui gère l'interaction avec les objets
public class Interaction : MonoBehaviour
{
    Outline outline;
    public UnityEvent onInteraction;

    private AudioSource m_AudioSource;
    private GameObject m_BlurGameObject;
    private Vignette m_Vignette;
    private ChromaticAberration m_ChromaticAberration;
    private CustomVolumeComponent m_BlurVolume;

    
    [Header("Options")]
    public bool isFragment = false; // vrai pour les fragments, false pour les batons
    public bool isStick = false; // vrai pour les batons, false pour les fragments
    public bool isNPC = false;    // vrai pour les pnj

    [Header("Options PNJ")]
    [SerializeField] private Transform transformNPC;
    [SerializeField] private string npcId; 

    [Header("UI Compteurs")]
    private static int stickCount = 0;
    private static int fragmentCount = 0;

    void Start()
    {
        outline = GetComponent<Outline>(); // on recupère le script outline
        DisableOutline(); // et on le désactive au début
        m_BlurGameObject = GameObject.Find("BlurVolume"); // o recupere le component qui gere les effets de flou
        m_ChromaticAberration = m_BlurGameObject.GetComponent<ChromaticAberration>();
        m_BlurVolume = m_BlurGameObject.GetComponent<CustomVolumeComponent>();
        m_Vignette = m_BlurGameObject.GetComponent<Vignette>();
    }

    public void Interact()
    {
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

    public void UpdateBlur()
    {
        Debug.Log("La il adapte le flou normalement");
    //Pour ajuster le flou il faut:
    // - Relever la valeur du compteur de fragment
    // - Deduire les valeurs à donner à chaque paramètre des effets (On a pas encore choisi les valeurs)
    // - update les valeurs
    
    }
}