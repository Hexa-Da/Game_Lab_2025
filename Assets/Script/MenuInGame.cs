using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Rattaché au GameManager
// Script pour gérer le menu en jeu
public class MenuInGame : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject selectedGameObject; // Référence au bouton sélectionné dès l'ouverture du menu
    [SerializeField] private EventSystem eventController; 
    [SerializeField] private Button pauseButton; 
    [SerializeField] private GameObject stickCounter;
    [SerializeField] private GameObject fragmentCounter;
    public GameObject menuInGame; 
    public GameObject option; 
    public static bool isPaused; 
    public static bool inOption; 

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.5f; 
    [SerializeField] private GameObject hudGroup;
    private CanvasGroup hudCanvasGroup;
    private Coroutine currentFadeCoroutine; // Ajout d'une coroutine pour l'animation

    [Header("UI Elements")]
    public static MenuInGame instance; // Référence au singleton
    private bool wasMoving = false; // Vérifie si le joueur est en mouvement

    [Header("Option")]
    [SerializeField] private GameObject selectedOption; // Référence au bouton sélectionné dès l'ouverture des options
    [SerializeField] private Button optionButton;
    [SerializeField] private Button optionCloseButton;

    [Header("UI Compteurs")]
    [SerializeField] public TMP_Text stickCounterText;
    [SerializeField] public TMP_Text fragmentCounterText;

    private void Awake()
    {
        instance = this;
        // Récupère le CanvasGroup de l'hud
        hudCanvasGroup = hudGroup.GetComponent<CanvasGroup>();

        // Initialisation des compteurs à l'écran
        UpdateUI(0, 0); // Affiche les compteurs à 0 au démarrage
    }

    void Start()
    {
        menuInGame.SetActive(false);
        option.SetActive(false);
        inOption = false;
        isPaused = false;

        // Vérification et configuration du bouton pause    
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        
        // Vérification et configuration du bouton d'option
        if (optionButton != null)
        {
            optionButton.onClick.AddListener(ToggleOption);
        }
        if (optionCloseButton != null)
        {
            optionCloseButton.onClick.AddListener(CloseOption);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inOption)
            {
                CloseOption();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Gestion de l'affichage du bouton de pause lorsque le joueur est immobile 
        if (!isPaused)
        {
            // Vérifie si le joueur est en mouvement avec les inputs
            bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;
            
            if (isMoving != wasMoving)
            {
                if (currentFadeCoroutine != null)
                {
                    // Arrête la coroutine en cours pour la remplacer par une nouvelle
                    StopCoroutine(currentFadeCoroutine);
                }
                currentFadeCoroutine = StartCoroutine(FadeButton(!isMoving));
                // Met à jour l'état précédent du mouvement
                wasMoving = isMoving;
            }
        }
    }

    // Fonction pour l'animation du bouton de pause
    private IEnumerator FadeButton(bool show)
    {
        float startAlpha = hudCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;
        float elapsedTime = 0f;

        float hudStartAlpha = hudCanvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            hudCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        hudCanvasGroup.alpha = targetAlpha;
        hudCanvasGroup.interactable = show;
        hudCanvasGroup.blocksRaycasts = show;
        currentFadeCoroutine = null;
    }

    public void ResumeGame()
    {
        menuInGame.SetActive(false);
        option.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        inOption = false;
        
        // Notifier le DialogueManager que le menu est fermé
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.SetMenuState(false);
            // le dialogue peut alors reprendre
        }
        
        // Réafficher le bouton de pause si le joueur est immobile
        if (!wasMoving)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }
            currentFadeCoroutine = StartCoroutine(FadeButton(true));
        }
    }

    public void PauseGame()
    {
        menuInGame.SetActive(true);
        option.SetActive(false);
        Time.timeScale = 0;
        isPaused = true;
        inOption = false;
        eventController.SetSelectedGameObject(selectedGameObject);
        
        // Notifier le DialogueManager que le menu est ouvert
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.SetMenuState(true);
            // le dialogue reste alors ouvert mais pu interactable
        }
    }

    public void ToggleOption()
    {
        option.SetActive(true);
        eventController.SetSelectedGameObject(selectedOption);
        inOption = true;
    }

    public void CloseOption()
    {
        option.SetActive(false);
        eventController.SetSelectedGameObject(selectedGameObject);
        inOption = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        // on désacrive le menu car
        // si le joueur relance le jeu, il arrive dans le jeu et non dans le menu 
        inOption = false;
        SceneManager.LoadScene("Menu");
    }

    public void UpdateUI(int stickCount, int fragmentCount)
    {
        if (stickCounterText != null)
            stickCounterText.text = stickCount.ToString();
        if (fragmentCounterText != null)
            fragmentCounterText.text = fragmentCount.ToString();
    }
}
