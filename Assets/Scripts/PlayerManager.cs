using System;
using UnityEngine;
using WebSocketSharp;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loginWindow;
    [SerializeField]
    private GameObject mainWindow;

    public static PlayerManager _instance;

    private string currentPlayerId;
    private string username;

    public string GetPlayerId()
    {
        return currentPlayerId;
    }

    public string GetUsername()
    {
        return username;
    }

    // Temporary debug code
    public PlayerInventoryHolder playerInventoryHolder;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<PlayerManager>();
        }
        loginWindow.SetActive(true);
        mainWindow.SetActive(false);
    }

    public void Logout()
    {
        UpdateWebsocketCommunication._instance.UnsubscribeUpdateWebsocket();
        //UpdateWebsocketCommunication._instance.DropWebsocketConnection();
        //playerInventoryHolder.ClearData();
        loginWindow.SetActive(true);
        mainWindow.SetActive(false);
    }


    public void PerformSignup(string username, string password, Action<string> callback)
    {
        LoginRestCommunication._instance.Signup(username, password, callback);
    }

    public void PerformLogin(string username, string password)
    {
        LoginRestCommunication._instance.Login(username, password, ProcessLogin);
    }

    private void ProcessLogin(string tokenJson)
    {
        if (tokenJson.IsNullOrEmpty())
        {
            return;
        }

        // should perform actual login attempt with rest call to security server


        TokenResponse response = JsonUtility.FromJson<TokenResponse>(tokenJson);

        currentPlayerId = response.access_token;

        UpdateWebsocketCommunication._instance.EstablishWebsocketConnection();


        // Get player inventory
        InventoryRestCommunication._instance.GetInventory(currentPlayerId, SetInventory);
        //// Get player currency
        InventoryRestCommunication._instance.GetCurrency(currentPlayerId, SetCurrency);
        //// Get floating currency
        InventoryRestCommunication._instance.GetFloating(currentPlayerId, SetFloating);

        LoginRestCommunication._instance.GetUserInfo(SetUsername);
        // Subscribe with playerid to receive updates from websocket

        loginWindow.SetActive(false);
        mainWindow.SetActive(true);
    }

    public void SetUsername(string username)
    {
        this.username = username;

        UpdateWebsocketCommunication._instance.SubscribeUpdateWebsocket(username);
    }

    private void SetInventory(string inventoryJson)
    {
        if (inventoryJson.IsNullOrEmpty()) return;
        var inventory = JsonUtility.FromJson<PlayerInventory>(inventoryJson);
        playerInventoryHolder.SetInventory(inventory.inventory);
    }

    private void SetCurrency(string currency)
    {
        if (currency.IsNullOrEmpty()) return;
        //var ownedCurrency = JsonUtility.FromJson<DataContainer>(currency);
        playerInventoryHolder.SetCurrency(int.Parse(currency));
    }

    private void SetFloating(string floating)
    {
        if (floating.IsNullOrEmpty()) return;
        //var currentlyFloating = JsonUtility.FromJson<DataContainer>(floating);
        playerInventoryHolder.SetFloating(int.Parse(floating));
    }
}

public class TokenResponse
{
    public string access_token;
}