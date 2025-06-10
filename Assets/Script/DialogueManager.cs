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
        public string dialogueState; // État actuel du dialogue (ex: "first_meet", "quest_started", "quest_completed")
    }

    [System.Serializable]
    public class DialogueCondition
    {
        public enum ConditionType
        {
            FragmentCount,
            // Ajoutez d'autres types de conditions ici si nécessaire
        }

        public ConditionType type;
        public int requiredValue;
        public string conditionName; // Pour identifier la condition (ex: "fragments")
    }

    [System.Serializable]
    public class DialogueLine // espace de defintion des lignes de dialogue
    {
        [TextArea(2, 5)] // defini la taille de la zone de texte pour la ligne de dialogue
        public string dialogueId;   // ID unique de cette ligne de dialogue
        public string text;
        public List<DialogueChoice> choices; // Liste des choix possibles
        public string requiredState; // État requis pour afficher cette ligne de dialogue
        public string setState; // État à définir après avoir affiché cette ligne
        public DialogueCondition condition; // Condition pour afficher cette ligne

        // vérifie si la ligne de dialogue a des choix 
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

    // Dictionnaire pour stocker l'état des dialogues de chaque NPC
    private Dictionary<string, string> npcDialogueStates = new Dictionary<string, string>();

    // Variable statique pour stocker le nombre de fragments
    private static int fragmentCount = 0;

    // Méthode pour mettre à jour le nombre de fragments
    public static void UpdateFragmentCount(int count)
    {
        fragmentCount = count;
        Debug.Log($"Fragment count updated to: {count}");
    }

    private bool CheckCondition(DialogueCondition condition)
    {
        if (condition == null) return true;

        switch (condition.type)
        {
            case DialogueCondition.ConditionType.FragmentCount:
                return Interaction.GetFragmentCount() >= condition.requiredValue;
            default:
                return true;
        }
    }

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

    public void StartDialogue(string npcId, Transform npcTransform, string startDialogueId = null)
    {
        // Trouver le NPC correspondant
        currentDialogue = dialogues.Find(d => d.npcId == npcId);

        // Nettoyer tout dialogue existant
        if (activeBubble != null)
        {
            EndDialogue();
        }

        // Initialiser l'état du dialogue si nécessaire
        if (!npcDialogueStates.ContainsKey(npcId))
        {
            npcDialogueStates[npcId] = "first";
        }

        // Si un ID de dialogue de départ est spécifié, l'utiliser
        if (!string.IsNullOrEmpty(startDialogueId))
        {
            if (dialogueDatabase.TryGetValue(startDialogueId, out DialogueLine startLine))
            {
                currentLine = startLine;
            }
            else
            {
                Debug.LogWarning($"Start dialogue ID {startDialogueId} not found for NPC {npcId}");
                return;
            }
        }
        else
        {
            // Sinon, trouver la première ligne de dialogue valide
            currentLine = null;
            foreach (var line in currentDialogue.dialogueLines)
            {
                // Vérifier l'état et la condition
                if ((string.IsNullOrEmpty(line.requiredState) || line.requiredState == npcDialogueStates[npcId]) 
                    && CheckCondition(line.condition))
                {
                    currentLine = line;
                    Debug.Log($"Found valid dialogue line. State: {npcDialogueStates[npcId]}, Fragment count: {Interaction.GetFragmentCount()}");
                    break;
                }
            }
        }

        if (currentLine == null)
        {
            Debug.LogWarning($"No valid dialogue found for NPC {npcId} in state {npcDialogueStates[npcId]}. Current fragment count: {Interaction.GetFragmentCount()}");
            return;
        }

        bubbleTarget = npcTransform;

        // Créer la bulle de dialogue
        CreateDialogueBubble();
        DisplayCurrentLine();    
    }

    private void CreateDialogueBubble()
    {
        // Détruire la bulle précédente si elle existe
        if (activeBubble != null)
            Destroy(activeBubble);

        // Créer la bulle dans le Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Créer un nouveau GameObject parent pour la bulle
        GameObject bubbleParent = new GameObject("DialogueBubbleParent");
        bubbleParent.transform.SetParent(canvas.transform, false);
        
        // Ajouter un CanvasGroup pour contrôler l'ordre de rendu
        bubbleCanvasGroup = bubbleParent.AddComponent<CanvasGroup>();
        bubbleCanvasGroup.blocksRaycasts = true;
        bubbleCanvasGroup.interactable = true;

        // Déplacer le parent au début de la hiérarchie pour qu'il soit rendu en premier
        bubbleParent.transform.SetAsFirstSibling();

        activeBubble = Instantiate(dialogueBubblePrefab, bubbleParent.transform);
        currentBubbleText = activeBubble.GetComponentInChildren<TMP_Text>();

        // Trouver le ChoicesContainer dans la bulle créée
        choicesContainer = activeBubble.transform.Find("ChoicesContainer").GetComponent<RectTransform>();

        if (currentBubbleText == null || choicesContainer == null)
        {
            EndDialogue();
            return;
        }

        // Mettre à jour l'interactivité en fonction de l'état du menu
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

        // Convertir la position monde du PNJ en position écran
        Vector3 screenPos = Camera.main.WorldToScreenPoint(bubbleTarget.position);
        
        // Vérifier si le point est devant la caméra
        if (screenPos.z < 0)
        {
            activeBubble.SetActive(false);
            return;
        }
        
        activeBubble.SetActive(true);
        
        // Calculer l'offset en fonction de la hauteur de l'écran
        float offset = Screen.height * bubbleOffsetPercent; // Convertir le pourcentage en décimal
        
        // Appliquer la position à la bulle UI
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

        // Mettre à jour le texte
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
            // Créer un bouton pour chaque choix
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            RectTransform rectTransform = choiceButton.GetComponent<RectTransform>();
            
            // Configurer le texte et le callback
            TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.choiceText;

            Button button = choiceButton.GetComponent<Button>();

            // Utiliser une variable locale pour éviter les problèmes de closure
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
            
            // Mettre à jour l'état du dialogue si nécessaire
            if (!string.IsNullOrEmpty(currentLine.setState))
            {
                npcDialogueStates[currentDialogue.npcId] = currentLine.setState;
                Debug.Log($"Setting dialogue state to: {currentLine.setState} for NPC: {currentDialogue.npcId}");
            }
            else
            {
                // Si setState est vide, on réinitialise l'état à "first"
                npcDialogueStates[currentDialogue.npcId] = "first";
                Debug.Log($"Resetting dialogue state to 'first' for NPC: {currentDialogue.npcId}");
            }
            
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
            // Détruire le parent de la bulle
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

    // Méthode pour définir manuellement l'état d'un dialogue
    public void SetDialogueState(string npcId, string state)
    {
        npcDialogueStates[npcId] = state;
    }

    // Méthode pour obtenir l'état actuel d'un dialogue
    public string GetDialogueState(string npcId)
    {
        return npcDialogueStates.ContainsKey(npcId) ? npcDialogueStates[npcId] : "first";
    }

    // Méthode utilitaire pour obtenir l'ID d'une ligne de dialogue par son index
    public string GetDialogueIdByIndex(string npcId, int index)
    {
        NPCDialogue dialogue = dialogues.Find(d => d.npcId == npcId);
        if (dialogue != null && index >= 0 && index < dialogue.dialogueLines.Count)
        {
            return dialogue.dialogueLines[index].dialogueId;
        }
        return null;
    }
}