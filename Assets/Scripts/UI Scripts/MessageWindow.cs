using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5)
        {
            RemoveMessage();
        }
    }

    public void SetText(string message)
    {
        text.text = message;
    }

    public void RemoveMessage()
    {
        Destroy(gameObject);
    }
}
