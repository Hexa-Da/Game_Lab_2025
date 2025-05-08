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
    public GameObject menuInGame; 
    public GameObject option; 
    public GameObject inventory;
    public static bool isPaused; 
    public static bool inOption; 
    public static bool inInventory; 

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.5f; 
    private CanvasGroup pauseButtonCanvasGroup; // crée un CanvasGroup pour le bouton de pause
    private Coroutine currentFadeCoroutine; // Ajout d'une coroutine pour l'animation

    [Header("UI Elements")]
    public static MenuInGame instance; // Référence au singleton
    private bool wasMoving = false; // Vérifie si le joueur est en mouvement

    [Header("Option")]
    [SerializeField] private GameObject selectedOption; // Référence au bouton sélectionné dès l'ouverture des options
    [SerializeField] private Button optionButton;
    [SerializeField] private Button optionCloseButton;

    [Header("Inventory")]
    [SerializeField] private GameObject selectedInventory; // Référence au bouton sélectionné dès l'ouverture de l'inventaire
    [SerializeField] private GameObject inventoryPanel; // Panel contenant la grille d'inventaire
    [SerializeField] private GameObject inventoryGrid; // Grille pour afficher les objets
    [SerializeField] private Button inventoryButton; // Bouton pour ouvrir l'inventaire depuis le menu principal
    [SerializeField] private Button inventoryCloseButton; // Bouton pour fermer l'inventaire
    [SerializeField] private TMP_Text inventoryTitle; // Titre de l'inventaire
    [SerializeField] private TMP_Text itemCounterText; // Référence au texte pour afficher le nombre d'objets
    private int itemCount = 0;
    private List<GameObject> collectedItems = new List<GameObject>(); // Liste des objets collectés

    private void Awake()
    {
        instance = this;
        // Récupère le CanvasGroup du bouton pause
        pauseButtonCanvasGroup = pauseButton.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        menuInGame.SetActive(false);
        option.SetActive(false);
        inventory.SetActive(false);
        inInventory = false;
        inOption = false;
        isPaused = false;
        
        // Initialise le nombre d'objets collectés
        itemCounterText.text = "Nombre d'objects\n" + itemCount;

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

        // Vérification et configuration du bouton d'inventaire
        if (inventoryButton != null)
        {
            inventoryButton.onClick.AddListener(ToggleInventory);
        }
        if (inventoryCloseButton != null)
        {
            inventoryCloseButton.onClick.AddListener(CloseInventory);
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
            else if (inInventory)
            {
                CloseInventory();
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
        float startAlpha = pauseButtonCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f; // Si show est true, alors targetAlpha est 1, sinon 0
        float elapsedTime = 0f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            // Interpolation linéaire entre startAlpha et targetAlpha
            pauseButtonCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t); 
            yield return null;
        }

        pauseButtonCanvasGroup.alpha = targetAlpha;
        pauseButton.interactable = show;
        currentFadeCoroutine = null; // Réinitialise la coroutine
    }

    // Fonction pour mettre à jour l'inventaire lorsque le joueur ramasse un objet
    public void UpdateUI()
    {
        itemCount++;
        itemCounterText.text = "Nombre d'objects\n" + itemCount;
        // Mettre à jour la grille d'inventaire
        UpdateInventoryGrid();
    }

    private void UpdateInventoryGrid()
    {
        // TODO: Ajouter la logique pour afficher les objets dans la grille
    }

    public void ResumeGame()
    {
        menuInGame.SetActive(false);
        option.SetActive(false);
        inventory.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        inOption = false;
        inInventory = false;
        
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
        inventory.SetActive(false);
        Time.timeScale = 0;
        isPaused = true;
        inOption = false;
        inInventory = false;
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

    public void ToggleInventory()
    {
        inventory.SetActive(true);
        eventController.SetSelectedGameObject(selectedInventory);
        inInventory = true;
    }

    public void CloseInventory()
    {
        inventory.SetActive(false);
        eventController.SetSelectedGameObject(selectedGameObject);
        inInventory = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        // on désacrive le menu car
        // si le joueur relance le jeu, il arrive dans le jeu et non dans le menu 
        inOption = false;
        inInventory = false;
        SceneManager.LoadScene("Menu");
    }
}
