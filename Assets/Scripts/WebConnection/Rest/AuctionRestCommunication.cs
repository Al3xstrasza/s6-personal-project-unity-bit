using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class AuctionRestCommunication : MonoBehaviour
{
    private const string URI_AUCTIONCREATE = "inventory/createNewAuction";
    private const string URI_AUCTIONFILTER = "auction/retrieveAuctions";
    private const string URI_GETUPTODATEAUCTION = "auction/retrieveUpToDateState";
    private const string URI_PLACEBID = "auction/placeBid";
    [SerializeField]
    private RestClient client;

    public static AuctionRestCommunication _instance;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<AuctionRestCommunication>();
        }
    }

    // Create Auction
    public void CreateAuction(AuctionCreationObject auction)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("POST", auction, URI_AUCTIONCREATE, headers, DisplayMessage);
        }
    }
    // Cancel Auction

    // Place Bid
    public void PlaceBid(int amount, string auctionId)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("auctionId", auctionId);
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("POST", new DataContainer() {heldInt = amount}, URI_PLACEBID, headers, DisplayMessage);
        }
    }

    // Retrieve filtered
    public void RetrieveFilteredAuctions(FilterOptions options, Action<string> callback)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("POST", options, URI_AUCTIONFILTER, headers, callback);
        }
    }

    public void GetUpToDateAuctionStateBeforeSubscribing(string auctionId, Action<string> callback)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("POST", new DataContainer() {heldString = auctionId}, URI_GETUPTODATEAUCTION, headers, callback);
        }
    }

    private void DisplayMessage(string message)
    {
        if (message.IsNullOrEmpty()) return;
        MessageDisplayer._instance.DisplayMessage(message);
    }
}
