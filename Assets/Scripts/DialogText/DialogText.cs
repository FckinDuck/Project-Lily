using UnityEngine;

[CreateAssetMenu(menuName ="Dialogue/New Dialogue Container")]
public class DialogText : ScriptableObject
{
    public string speakerName;
    [TextArea(5,15)]
    public string[] paragraphs;
}
