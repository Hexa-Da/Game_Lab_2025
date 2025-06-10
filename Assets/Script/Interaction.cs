using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


// Rattaché à chaque objet avec lequel on peut interagir
// Script qui gère l'interaction avec les objets
public class Interaction : MonoBehaviour
{
    Outline outline;

    private SpriteRenderer m_SpriteRender;
    private AudioSource m_AudioSource;
    
    private Vignette m_Vignette;
    private ChromaticAberration m_ChromaticAberration;
    private CustomVolumeComponent m_BlurVolume;

    [Header("Objets")]
    public Volume m_BlurVolumeProfile;
    public AudioClip StickSound;
    public AudioClip FragmentSound;

    [Header("Options")]
    public bool isFragment = false; // vrai pour les fragments, false pour les batons
    public bool isStick = false; // vrai pour les batons, false pour les fragments
    public bool isGlasses = false; //vrai pour les lunettes dans le tuto
    public bool isTeleport = false; // vrai pour la porte du tuto qui permet d'avancer
    public float initialBlurValue = 0.126f;
    public float BlurStep = 0.021f;
    public float initialCAValue = 0.96f;
    public float CAStep = 0.07f;
    public float intialVignetteValue = 0.45f;
    public float VignetteStep = 0.16f;


    [Header("UI Compteurs")]
    private static int stickCount = 0;
    private static int fragmentCount = 0;

    // Static getters and setters for the counters
    public static int GetStickCount() => stickCount;
    public static int GetFragmentCount() => fragmentCount;
    public static void SetStickCount(int count) => stickCount = count;
    public static void SetFragmentCount(int count) => fragmentCount = count;

    void Start()
    {
        outline = GetComponent<Outline>(); // on recupère le script outline
        DisableOutline(); // et on le désactive au début
        m_BlurVolumeProfile.profile.TryGet<CustomVolumeComponent>(out m_BlurVolume);
        m_BlurVolumeProfile.profile.TryGet<ChromaticAberration>(out m_ChromaticAberration); //cherche le volume component ChromaticAberration et l'assigne a m_ChromaticAberration       m_BlurVolumeProfile.profile.TryGet<CustomVolumeComponent>(out m_BlurVolume);
        m_BlurVolumeProfile.profile.TryGet<Vignette>(out m_Vignette);
        m_SpriteRender = this.GetComponent<SpriteRenderer>();
        m_AudioSource = this.GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (isFragment || isStick || isGlasses)
        {
            
            if (isFragment)
            {
                fragmentCount++;
                m_AudioSource.clip = FragmentSound;
            }
            if (isStick)
            {
                stickCount++;
                m_AudioSource.clip = StickSound;
            }
            MenuInGame.instance.UpdateUI(stickCount, fragmentCount);
            tag = "PickedUp";
            UpdateBlur();
            if (m_AudioSource.clip != null)
            {
                m_AudioSource.Play();
            }
            m_SpriteRender.enabled = false;
            Destroy(this, 5f);
        }

        else if (isTeleport) 
        {
            SceneManager.LoadScene(2);
        }
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
        m_BlurVolume.horizontalBlur.Override(initialBlurValue - BlurStep * fragmentCount);
        m_BlurVolume.verticalBlur.Override(initialBlurValue - BlurStep * fragmentCount);
        m_ChromaticAberration.intensity.Override(initialCAValue - CAStep * fragmentCount);
        m_Vignette.intensity.Override(intialVignetteValue - VignetteStep * fragmentCount);
    }
}