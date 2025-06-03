using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker;
    public string nextDialogueId;
    [TextArea] public string text;
}