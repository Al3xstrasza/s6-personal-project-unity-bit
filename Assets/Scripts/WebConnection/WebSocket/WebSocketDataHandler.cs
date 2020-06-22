using System;
using System.Collections;
using System.Collections.Generic;
using StompHelper;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

public class WebSocketDataHandler : MonoBehaviour
{
    public static BidEvent bidEvent;
    public static InventoryChangeEvent inventoryChangeEvent;
    public static CurrencyChangeEvent currencyChangeEvent;

    private void Start()
    {
        if (bidEvent == null)
            bidEvent = new BidEvent();
        if (inventoryChangeEvent == null)
            inventoryChangeEvent = new InventoryChangeEvent();
        if (currencyChangeEvent == null)
            currencyChangeEvent = new CurrencyChangeEvent();
    }

    private void Update()
    {
        if (WebSocketClient.responses.Count <= 0) return;
        HandleMessage(WebSocketClient.responses[0]);
        WebSocketClient.responses.RemoveAt(0);

    }

    public void HandleMessage(StompMessage data)
    {
        if (data.Body.IsNullOrEmpty())
        {
            return;
        }

        if (data.Headers.ContainsKey("MessageType"))
        {
            var messageType = "";
            var changeType = "";
            var floating = "";
            data.Headers.TryGetValue("MessageType", out messageType);
            if (messageType.IsNullOrEmpty()) return;

            var inventoryChange = new InventoryChange();
            var currencyChange = new CurrencyChange();

            switch (messageType)
            {
                case "bidPlacement":
                    bidEvent.Invoke(JsonUtility.FromJson<Bid>(data.Body));
                    break;
                case "inventoryChange":
                    data.Headers.TryGetValue("changeType", out changeType);
                    inventoryChange.item = JsonUtility.FromJson<ItemBase>(data.Body);
                    inventoryChange.add = changeType.Equals("add");
                    inventoryChangeEvent.Invoke(inventoryChange);
                    break;
                case "currencyChange":
                    data.Headers.TryGetValue("changeType", out changeType);
                    data.Headers.TryGetValue("floating", out floating);
                    currencyChange.amount = int.Parse(data.Body);
                    currencyChange.add = changeType.Equals("add");
                    currencyChange.floating = floating.Equals("true");
                    currencyChangeEvent.Invoke(currencyChange);
                    break;
            }
        }
        // got body, process data


    }
}


[Serializable]
public class BidEvent : UnityEvent<Bid>
{
}

[Serializable]
public class InventoryChangeEvent : UnityEvent<InventoryChange>
{
}

[Serializable]
public class CurrencyChangeEvent : UnityEvent<CurrencyChange>
{
}

[Serializable]
public class InventoryChange
{
    public ItemBase item;
    public bool add;
}

[Serializable]
public class CurrencyChange
{
    public int amount;
    public bool add;
    public bool floating;
}