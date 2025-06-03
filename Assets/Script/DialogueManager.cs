using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// RattachÃ© au dialogueManager
// Script qui gÃ¨re la bulle de dialogue et les dialogues
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Configuration bulle de dialogue")]
    [SerializeField] private GameObject dialogueBubblePrefab;
    private float bubbleOffsetPercent = 0.2f; // Distance au-dessus du PNJ en unitÃ©s monde
    private Transform bubbleTarget; // defini la position de la bulle de dialogue
    private GameObject activeBubble;


    [Header("Configuration des choix")]
    [SerializeField] private GameObject choiceButtonPrefab; 
    private RectTransform choicesContainer; // defini le conteneur pour les boutons de choix
    private List<GameObject> activeChoiceButtons = new List<GameObject>();

    [System.Serializable] 
    public class NPCDialogue // espace de defintion des dialogues en fonction de l'id du NPC
    {
        public string npcId; // Identifiant unique du NPC
        public string npcName;
        public List<DialogueLine> dialogueLines; // Lignes de dialogue pour ce NPC
    }

    [System.Serializable]
    public class DialogueLine // espace de defintion des lignes de dialogue
    {
        [TextArea(2, 5)] // defini la taille de la zone de texte pour la ligne de dialogue
        public string dialogueId;   // ID unique de cette ligne de dialogue
        public string text;
        public List<DialogueChoice> choices; // Liste des choix possibles

        // vÃ©rifie si la ligne de dialogue a des choix 
        public bool HasChoices => choices != null && choices.Count > 0; 
    }

    [System.Serializable]
    public class DialogueChoice // espace de defintion des choix de rÃ©ponse
    {
        [TextArea(1, 2)] // defini la taille de la zone de texte pour le choix
        public string choiceText;  // Texte du choix
        public string nextDialogueId; // ID du dialogue suivant
    }
    
    // Liste des dialogues en fonction de l'id du NPC
    [SerializeField] private List<NPCDialogue> dialogues = new List<NPCDialogue>();
    // Dictionnaire des dialogues en fonction de l'id puis de la ligne de dialogue
    private Dictionary<string, DialogueLine> dialogueDatabase = new Dictionary<string, DialogueLine>();
    
    // Ã‰tat actuel du dialogue
    private TMP_Text currentBubbleText;
    private NPCDialogue currentDialogue;
    private DialogueLine currentLine;

    private CanvasGroup bubbleCanvasGroup;
    private bool isMenuOpen = false;

    private void Awake()
    {
        instance = this;
        InitializeDialogueDatabase();
    }

    // Initialise le dictionnaire des dialogues
    private void InitializeDialogueDatabase()
    {
        dialogueDatabase.Clear();
        foreach (NPCDialogue dialogue in dialogues)
        {
            foreach (DialogueLine line in dialogue.dialogueLines)
            {
                if (!string.IsNullOrEmpty(line.dialogueId))
                {
                    dialogueDatabase[line.dialogueId] = line;
                }
            }
        }
    }

    public void StartDialogue(string npcId, Transform npcTransform)
    {
        // Trouver le NPC correspondant
        currentDialogue = dialogues.Find(d => d.npcId == npcId);

        // Nettoyer tout dialogue existant
        if (activeBubble != null)
        {
            EndDialogue();
        }

        // Configurer le dialogue courant
        currentLine = currentDialogue.dialogueLines[0];
        bubbleTarget = npcTransform;

        // CrÃ©er la bulle de dialogue
        CreateDialogueBubble();
        DisplayCurrentLine();    
    }

    private void CreateDialogueBubble()
    {
        // DÃ©truire la bulle prÃ©cÃ©dente si elle existe
        if (activeBubble != null)
            Destroy(activeBubble);

        // CrÃ©er la bulle dans le Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // CrÃ©er un nouveau GameObject parent pour la bulle
        GameObject bubbleParent = new GameObject("DialogueBubbleParent");
        bubbleParent.transform.SetParent(canvas.transform, false);
        
        // Ajouter un CanvasGroup pour contrÃ´ler l'ordre de rendu
        bubbleCanvasGroup = bubbleParent.AddComponent<CanvasGroup>();
        bubbleCanvasGroup.blocksRaycasts = true;
        bubbleCanvasGroup.interactable = true;

        // DÃ©placer le parent au dÃ©but de la hiÃ©rarchie pour qu'il soit rendu en premier
        bubbleParent.transform.SetAsFirstSibling();

        activeBubble = Instantiate(dialogueBubblePrefab, bubbleParent.transform);
        currentBubbleText = activeBubble.GetComponentInChildren<TMP_Text>();

        // Trouver le ChoicesContainer dans la bulle crÃ©Ã©e
        choicesContainer = activeBubble.transform.Find("ChoicesContainer").GetComponent<RectTransform>();

        if (currentBubbleText == null || choicesContainer == null)
        {
            EndDialogue();
            return;
        }

        // Mettre Ã  jour l'interactivitÃ© en fonction de l'Ã©tat du menu
        UpdateBubbleInteractivity();

        PositionBubble();
    }

    private void Update()
    {
        if (activeBubble != null)
        {
            // Maintenir la position et l'orientation
            PositionBubble();
        }
    }

    public void PositionBubble()
    {
        if (bubbleTarget == null || activeBubble == null) return;

        // Convertir la position monde du PNJ en position Ã©cran
        Vector3 screenPos = Camera.main.WorldToScreenPoint(bubbleTarget.position);
        
        // VÃ©rifier si le point est devant la camÃ©ra
        if (screenPos.z < 0)
        {
            activeBubble.SetActive(false);
            return;
        }
        
        activeBubble.SetActive(true);
        
        // Calculer l'offset en fonction de la hauteur de l'Ã©cran
        float offset = Screen.height * bubbleOffsetPercent; // Convertir le pourcentage en dÃ©cimal
        
        // Appliquer la position Ã  la bulle UI
        RectTransform rectTransform = activeBubble.GetComponent<RectTransform>();
        Vector3 newPosition = new Vector3(screenPos.x, screenPos.y + offset, 0);
        rectTransform.position = newPosition;
    }

    private void DisplayCurrentLine()
    {
        if (currentLine == null)
        {
            EndDialogue();
            return;
        }

        // Mettre Ã  jour le texte
        currentBubbleText.text = currentLine.text;

        // Afficher les choix s'il y en a
        if (currentLine.choices != null && currentLine.choices.Count > 0)
        {
            DisplayChoices(currentLine.choices);
        }
    }

    private void DisplayChoices(List<DialogueChoice> choices)
    {
        // Nettoyer les anciens boutons
        ClearChoices();

        // Ajouter un HorizontalLayoutGroup au ChoicesContainer s'il n'en a pas
        HorizontalLayoutGroup layout = choicesContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
        {
            layout = choicesContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10f; // Espace entre les boutons
        }

        foreach (var choice in choices)
        {
            // CrÃ©er un bouton pour chaque choix
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            RectTransform rectTransform = choiceButton.GetComponent<RectTransform>();
            
            // Configurer le texte et le callback
            TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.choiceText;

            Button button = choiceButton.GetComponent<Button>();

            // Utiliser une variable locale pour Ã©viter les problÃ¨mes de closure
            var currentChoice = choice;
            button.onClick.RemoveAllListeners(); // Nettoyer les listeners existants
            button.onClick.AddListener(() => {
                OnChoiceSelected(currentChoice);
            });

            activeChoiceButtons.Add(choiceButton);
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (string.IsNullOrEmpty(choice.nextDialogueId))
        {
            EndDialogue();
            return;
        }

        // Trouver la prochaine ligne de dialogue
        if (dialogueDatabase.TryGetValue(choice.nextDialogueId, out DialogueLine nextLine))
        {
            currentLine = nextLine;
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ClearChoices()
    {
        foreach (var button in activeChoiceButtons)
        {
            Destroy(button);
        }
        activeChoiceButtons.Clear();
    }

    public void SetMenuState(bool isOpen)
    {
        isMenuOpen = isOpen;
        UpdateBubbleInteractivity();
    }

    private void UpdateBubbleInteractivity()
    {
        if (bubbleCanvasGroup != null)
        {
            bubbleCanvasGroup.interactable = !isMenuOpen;
            bubbleCanvasGroup.blocksRaycasts = !isMenuOpen;
        }
    }

    private void EndDialogue()
    {
        ClearChoices();
        
        if (activeBubble != null)
        {
            // DÃ©truire le parent de la bulle
            if (activeBubble.transform.parent != null)
            {
                Destroy(activeBubble.transform.parent.gameObject);
            }
            else
            {
                Destroy(activeBubble);
            }
        }

        activeBubble = null;
        currentBubbleText = null;
        bubbleTarget = null;
        currentDialogue = null;
        bubbleCanvasGroup = null;
    }
}