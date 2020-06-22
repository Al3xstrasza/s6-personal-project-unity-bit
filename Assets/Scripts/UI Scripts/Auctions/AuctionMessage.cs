using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AuctionMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetMessage(string message)
    {
        text.text = message;
    }
}
