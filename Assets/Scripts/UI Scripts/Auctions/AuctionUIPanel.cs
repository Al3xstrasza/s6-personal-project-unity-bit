using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuctionUIPanel : MonoBehaviour
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
    private Button buttonViewAuction;
    
    private Auction auction;

    private bool doLoop = true;

    private void Update()
    {
        if (auction == null) return;
        if (!doLoop) return;
        if (auction.auctionEnded)
        {
            buttonBuyout.interactable = false;
            buttonViewAuction.interactable = false;
            doLoop = false;
        }
        else
        {
            endTimeCounter.text = CalculateTimeString();
        }
    }

    public void SetupUiPanel(Auction auction, Action<string> openAuction, Action<string> buyout)
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

        if (openAuction != null && buyout != null)
        {
            buttonViewAuction.onClick.AddListener(delegate() { openAuction(this.auction.auctionId); });
            buttonBuyout.onClick.AddListener(delegate() { buyout(this.auction.auctionId); });
        }

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
