using UnityEngine;

public class Skeleton : NPC,ITalkable
{
    [SerializeField] private DialogText dialogText;
    [SerializeField] private DialogueController dialogueController;
    public override void Interact()
    {
        Talk(dialogText);
    }

    public void Talk(DialogText dialogText)
    {
        dialogueController.DisplayNextParagraph(dialogText);
    }
}
