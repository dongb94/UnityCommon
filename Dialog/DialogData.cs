using System.Collections.Generic;

public class DialogData
{
    public string EventName;
    public DialogManager.Character RightCharacter;
    public DialogManager.Character LeftCharacter;
    public bool RightActive;
    public bool LeftActive;
    public string Name;
    public Queue<string> TextGroup;

    public DialogData(Queue<string> p_TextGroup)
    {
        TextGroup = p_TextGroup;
    }
}