using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRestCommunication : MonoBehaviour
{
    private const string URI_GETCURRENCY = "currency/retrieveBalance";
    private const string URI_GETFLOATING = "currency/retrieveFloating";
    private const string URI_GETINVENTORY = "inventory/retrieveInventory";
    [SerializeField]
    private RestClient client;

    public static InventoryRestCommunication _instance;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<InventoryRestCommunication>();
        }
    }

    // Get Inventory
    public void GetInventory(string user, Action<string> callback)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("GET", URI_GETINVENTORY, headers, callback);
        }
    }
    // Get Currency
    public void GetCurrency(string user, Action<string> callback)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("GET", URI_GETCURRENCY, headers, callback);
        }
        
    }

    // Get Floating
    public void GetFloating(string user, Action<string> callback)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("GET", new DataContainer() { heldString = user }, URI_GETFLOATING, headers, callback);
        }
    }
}
