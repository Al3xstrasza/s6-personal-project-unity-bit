using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UpdateWebsocketCommunication : MonoBehaviour
{
    [SerializeField]
    private WebSocketClient client;
    public static UpdateWebsocketCommunication _instance;
    // Start is called before the first frame update
    private void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<UpdateWebsocketCommunication>();
        }
    }

    private string subbedUser;

    public void EstablishWebsocketConnection()
    {
        client.EstablishWebsocketConnection();
    }

    public void DropWebsocketConnection()
    {
        client.DropWebsocketConnection();
    }

    public bool SubscribeUpdateWebsocket(string user)
    {
        Debug.Log("Subscribed on " + "/topic/" + user);
        var returnValue = client.SubscribeTopic("/topic/" + user, user);
        subbedUser = user;
        return returnValue;
    }

    public bool UnsubscribeUpdateWebsocket()
    {
        if (subbedUser.IsNullOrEmpty()) return false;
        var returnValue = client.UnsubscribeTopic(subbedUser);
        subbedUser = "";
        return returnValue;
    }
}
