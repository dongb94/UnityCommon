using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogParser
{
    public static Queue<DialogData> Parse(string file)
    {
        Queue<DialogData> DialogDataQueue = new Queue<DialogData>();
        
        var dialogGroup = file.Split('}');
        foreach (var dialogData in dialogGroup)
        {
            if(dialogData.IndexOf("\"") == -1) continue;
            var dialog = dialogData;
            dialog = dialog.Substring(dialog.IndexOf("\"")+1);

            var data = new DialogData(new Queue<string>());
            data.EventName = dialog.Substring(0, dialog.IndexOf("\""));

            dialog = dialog.Substring(dialog.IndexOf("{") + 1);

            var isString = false;
            var isValue = false;
            var stringStartIndex = 0;
            var variable = "";
            var word = "";
            var reservedWord = "";
            
            for (var pointer = 0; pointer<dialog.Length; pointer++)
            {
                var ch = dialog[pointer];
                if (isString && isValue && ch == '\\')
                {
                    word += dialog.Substring(stringStartIndex, pointer - stringStartIndex);
                    pointer++;
                    switch (dialog[pointer])
                    {
                        case 'n' :
                            word += "\n";
                            stringStartIndex = pointer + 1;
                            continue;
                    }
                }
                if(isString && ch!='"') continue;
                switch (ch)
                {
                    case '"' :
                        isString = !isString;
                      if (!isString)
                      {
                          if (isValue)
                          {
                              word += dialog.Substring(stringStartIndex, pointer - stringStartIndex);
                              InputValue(data, variable, word);
                          }
                          else
                          {
                              variable = dialog.Substring(stringStartIndex, pointer - stringStartIndex);
                          }
                      }
                      else
                      {
                          word = "";
                          stringStartIndex = pointer + 1;                          
                      }
                      continue;
                    case '[' :
                      isValue = true;
                      continue;
                    case ']' :
                      isValue = false;
                      continue;
                    case ' ' :
                    case ',' :
                      continue;
                }
                
                //not string && value
                if(!isValue) continue;
                if (ch == 't') reservedWord = dialog.Substring(pointer, 4);
                else if (ch == 'f') reservedWord = dialog.Substring(pointer, 5);
                switch (reservedWord)
                {
                    case "false" :
                        pointer += 4;
                        InputValue(data, variable, false);
                        break;
                    case "true" :
                        pointer += 3;
                        InputValue(data, variable, true);
                        break;
                }
                reservedWord = "";
            }
            
            DialogDataQueue.Enqueue(data);
        }
    
        return DialogDataQueue;
    }

    private static void InputValue(DialogData data, string key, string value)
    {
        switch (key)
        {
                case "RightCharacter" : data.RightCharacter = (DialogManager.Character) Enum.Parse(typeof(DialogManager.Character), value);
                    break;
                case "LeftCharacter" : data.LeftCharacter = (DialogManager.Character) Enum.Parse(typeof(DialogManager.Character), value);
                    break;
                case "Name" : data.Name = value;
                    break;
                case "Text" : data.TextGroup.Enqueue(value);
                    break;
        }
    }
    
    private static void InputValue(DialogData data, string key, bool value)
    {
        switch (key)
        {
            case "RightCharacter" : data.RightActive = value;
                break;
            case "LeftCharacter" : data.LeftActive = value;
                break;
        }
    }
}