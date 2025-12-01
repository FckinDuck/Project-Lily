using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10f;

    private Queue<string> paragraph = new Queue<string>();
    private bool convoEnded;
    private bool isTyping;


    private string p;

    private Coroutine typeDialogCoroutine;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.5f;
    public void DisplayNextParagraph(DialogText dialogText)
    {
        if (paragraph.Count == 0)
        {
            if (!convoEnded)
            {
                StartConvo(dialogText);
            }
            else
            {
                EndConvo();
                return;
            }

        }

        if (!isTyping)
        {
            p = paragraph.Dequeue();

            typeDialogCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        //NPCDialogueText.text = p;
        if (paragraph.Count ==0)
        {
            convoEnded = true;
        }
    }
    private void StartConvo(DialogText dialogText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        NPCNameText.text= dialogText.speakerName;

        for (int i = 0; i < dialogText.paragraphs.Length; i++)
        {
            paragraph.Enqueue(dialogText.paragraphs[i]);
        }
    }
    private void EndConvo()
    {
        paragraph.Clear(); 
        convoEnded = false;
        if (gameObject.activeSelf)
        {
            gameObject.SetActive (false);
        }
    }
    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        NPCDialogueText.text= "";

        string originalText =p ;
        string displayText="";
        int alphaIndex = 0;

        foreach (char c in p.ToCharArray())
        {
            alphaIndex++;
            NPCDialogueText.text = originalText;

            displayText = NPCDialogueText.text.Insert(alphaIndex,HTML_ALPHA);
            NPCDialogueText.text = displayText;

            yield return new WaitForSeconds(MAX_TYPE_TIME/typeSpeed);
        }


        isTyping = false;

    }
}
