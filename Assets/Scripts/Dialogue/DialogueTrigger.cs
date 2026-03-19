using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Color nameColor;
}
// character details of who is speaking


[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}
// a line of dialogue, showing the character and their line


[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines = new List<DialogueLine>();
}
// a full scene of dialogue, a list of dialogue lines


public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    
    public void Begin(){
        Debug.Log("Dialogue Triggered");
        DialogueManager.Instance.StartDialogue(dialogue);
    }

}
