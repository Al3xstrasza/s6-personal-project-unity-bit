using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class AuctionWebsocketCommunication : MonoBehaviour
{
    [SerializeField]
    private WebSocketClient client;
    public static AuctionWebsocketCommunication _instance;
    // Start is called before the first frame update
    private void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<AuctionWebsocketCommunication>();
        }
    }

    private string currentSubbedAuction = "";
    public bool SubscribeToAuction(string auctionId)
    {
        var returnValue = client.SubscribeTopic("/topic/" + auctionId, auctionId);
        if (returnValue)
        {
            currentSubbedAuction = auctionId;
        }

        return returnValue;
    }

    public bool UnsubscribeFromAuction()
    {
        if (currentSubbedAuction.IsNullOrEmpty()) return false;
        var returnValue = client.UnsubscribeTopic(currentSubbedAuction);
        if (returnValue)
        {
            currentSubbedAuction = "";
        }

        return returnValue;
    }
}
