using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject messagePrefab;

    [SerializeField] private Transform parent;

    public static MessageDisplayer _instance;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<MessageDisplayer>();
        }
    }

    public void DisplayMessage(string message)
    {
        var obj = Instantiate(messagePrefab, parent);
        obj.GetComponent<MessageWindow>().SetText(message);
    }
}
