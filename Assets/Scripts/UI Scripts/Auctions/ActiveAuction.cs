using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAuction : MonoBehaviour
{
    [SerializeField]
    private Image itemSprite;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemAmount;
    [SerializeField]
    private TextMeshProUGUI highestBid;
    [SerializeField]
    private TextMeshProUGUI bidCount;
    [SerializeField]
    private TextMeshProUGUI buyoutPrice;
    [SerializeField]
    private TextMeshProUGUI endTimeCounter;
    [SerializeField]
    private TextMeshProUGUI posterName;

    [SerializeField]
    private Button buttonBuyout;
    [SerializeField]
    private Button buttonExitAuction;
    [SerializeField]
    private Button buttonPostBid;

    [SerializeField]
    private TMP_InputField bidInput;

    [SerializeField]
    private GameObject bidUiPrefab;
    [SerializeField]
    private Transform uiBidParent;

    private Auction auction;

    private bool doLoop = true;

    private void Update()
    {
        if (auction == null) return;
        if (!doLoop) return;
        if (auction.auctionEnded)
        {
            buttonBuyout.interactable = false;
            doLoop = false;
        }
        else
        {
            endTimeCounter.text = CalculateTimeString();
        }
    }

    public void PushBid()
    {
        AuctionRestCommunication._instance.PlaceBid(int.Parse(bidInput.text), auction.auctionId);
    }

    public void AddBid(Bid bid)
    {
        auction.bidHistory.Add(bid);
        SetupBids();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        highestBid.text = auction.GetHighestBid().ToString();
        bidCount.text = auction.bidHistory.Count.ToString();
    }

    public void AddBuyout(string user)
    {
        var messageDisplayer = Instantiate(bidUiPrefab, uiBidParent).GetComponent<AuctionMessage>();

        messageDisplayer.SetMessage($"Buyout by: {user}");
        messageDisplayer.transform.SetAsFirstSibling();
    }

    private void SetupBids()
    {
        uiBidParent.Clear();
        //auction.bidHistory.Sort(); 
        List<Bid> bids = auction.bidHistory
            .OrderBy(x => x.bid == -10 ? 1 : 0)
            .ThenBy(x => x.bid == -20 ? 1 : 0)
            .ThenBy(x => x.bid)
            .ToList();
        bids.Reverse();
        //auction.bidHistory.Reverse();
        foreach (var bid in bids)
        {
            var bidDisplayer = Instantiate(bidUiPrefab, uiBidParent).GetComponent<AuctionMessage>();
            bidDisplayer.SetMessage(bid.ToRichString());
        }
    }

    public void SetupActiveAuction(Auction auction)
    {
        doLoop = true;
        this.auction = auction;
        if (auction?.item == null) return;
        itemSprite.sprite = auction.item.GetItemSprite();
        itemName.text = auction.item.GetItemName();
        itemAmount.text = auction.item.stackSize + "x";
        bidCount.text = auction.bidHistory.Count.ToString();
        highestBid.text = auction.GetHighestBid().ToString();
        buyoutPrice.text = auction.allowBuyout ? auction.buyoutPrice.ToString() : "No Buyout";
        posterName.text = auction.creator;
        endTimeCounter.text = "";
        WebSocketDataHandler.bidEvent.AddListener(AddBid);
        SetupBids();
    } 

    public void BreakAuction()
    {
        WebSocketDataHandler.bidEvent.RemoveListener(AddBid);
        bidInput.text = "";
    }

    private string CalculateTimeString()
    {
        DateTime end = FromUnixTime(auction.auctionEndTime);
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc);

        TimeSpan ts = end.Subtract(now);
        if (ts.Seconds < 0)
        {
            return $"Ending Soon";
        }
        return $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
    }

    public static DateTime FromUnixTime(long unixTime)
    {
        return epoch.AddMilliseconds(unixTime);
    }

    private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
