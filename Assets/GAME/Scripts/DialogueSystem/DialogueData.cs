using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueManager.DialogueLine[] lines;
    public DialogueData nextDialogue;
}