using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;

    private Queue<DialogueLine> dialogueLines;

    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        dialogueLines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        dialogueLines.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            dialogueLines.Enqueue(line);
        }

        DisplayNextLine();
        // clear any leftover lines, queue all lines
    }

    public void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }
        // check if there are any lines left, if not end dialogue

        DialogueLine currentLine = dialogueLines.Dequeue();
        characterNameText.text = currentLine.character.name;
        StopAllCoroutines();
        StartCoroutine(TypeLine(currentLine.line));
        // reset the newest line
    }
    
    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueText.text = "";
        characterNameText.text = "";
        // kill it all
    }

    private System.Collections.IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        // to stall the displays in a typewriter effect
    }

}
