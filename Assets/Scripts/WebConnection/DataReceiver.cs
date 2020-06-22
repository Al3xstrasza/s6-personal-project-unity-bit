using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataReceiver : MonoBehaviour
{
    public Text text;

    public void HandleText(string text)
    {
        DisplayMessage(text);
    }

    public void HandleBinary(byte[] bytes)
    {

    }


    public void DisplayMessage(string data)
    {
        text.text += data;
    }
}
