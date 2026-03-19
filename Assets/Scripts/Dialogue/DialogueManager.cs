using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;
    public GameObject backgroundPanel;
    public GameObject continueButton;

    private Queue<DialogueLine> dialogueLines;

    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;

    // public event Action OnDialogueFinished;
    // // to trigger an event saying the current dialogue is finished
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        dialogueLines = new Queue<DialogueLine>();

        dialogueText.text = "";
        characterNameText.text = "";
        backgroundPanel.SetActive(false);
        continueButton.SetActive(false);
        // to deactivate the dialogue UI until it is needed
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("StartDialogue called");

        isDialogueActive = true;
        dialogueLines.Clear();

        backgroundPanel.SetActive(true);
        continueButton.SetActive(true);
        dialogueText.text = "";
        characterNameText.text = "";
        // reactivate

        foreach (DialogueLine line in dialogue.lines)
        {
            dialogueLines.Enqueue(line);
        }

        Debug.Log("Queued lines: " + dialogueLines.Count);

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            continueButton.SetActive(false);
            EndDialogue();
            return;
        }
        // check if there are any lines left, if not end dialogue

        continueButton.SetActive(false);
        DialogueLine currentLine = dialogueLines.Dequeue();

        characterNameText.text = currentLine.character.name;
        characterNameText.color = currentLine.character.nameColor;
        StopAllCoroutines();
        StartCoroutine(TypeLine(currentLine.line));
        // reset the newest line
    }
    
    public void EndDialogue()
    {
        dialogueText.text = "";
        characterNameText.text = "";
        backgroundPanel.SetActive(false);
        continueButton.SetActive(false);
        Debug.Log("Dialogue Ended");
        isDialogueActive = false;
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
        continueButton.SetActive(true);
    }

}
