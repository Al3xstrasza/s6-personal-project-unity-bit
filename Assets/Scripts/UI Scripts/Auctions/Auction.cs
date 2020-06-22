using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Auction
{
    public bool auctionEnded = false;
    public int id;
    public string auctionId;
    public long auctionStartTime;
    public long auctionEndTime;
    public ItemBase item;
    //public int amount;
    public int priceperunit;
    public bool allowBuyout = false;
    public int buyoutPrice;
    public List<Bid> bidHistory = new List<Bid>();
    public string creator;

    public FilterOptions filterOptions;

    public Auction()
    {

    }

    public int GetHighestBid()
    {
        return bidHistory.Count > 0 ? bidHistory.Max(t => t.bid) : 0;
    }

    public override string ToString()
    {
        return auctionId;
    }
}

[Serializable]
public class FilterOptions
{
    public string name;
    public string type;
    public string rarity;
    public bool buyout;

    public override string ToString()
    {
        return $"{name} - {type} - {rarity} - {buyout}";
    }
}

[Serializable]
public class AuctionCollection
{
    public List<Auction> auctions;
}