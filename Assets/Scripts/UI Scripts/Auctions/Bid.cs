using System;
using WebSocketSharp;

[Serializable]
public class Bid : IComparable<Bid>
{
    public int bid;
    public DateTime bidDate;
    public string bidder;

    public Bid(int bid, string bidder)
    {
        this.bid = bid;
        this.bidder = bidder;
    }

    public int CompareTo(Bid other)
    {
        
        if (bid == -20 || bid == -10)
        {
            return int.MaxValue;
        }
        return other.CompareTo(this);
    }

    public override string ToString()
    {
        return $"{bidder} offers: {bid}";
    }

    public string ToRichString()
    {
        if (bid == -20)
        {
            return $"The auction has ended without bids, returning item";
        }
        if (bid == -10)
        {
            return $"{bidder} has won the auction";
        }
        if (bidder.IsNullOrEmpty())
        {
            return $"Minimum bids start at <#8900FF>{bid}</color>";
        }

        return $"{bidder} offers: <#0083FF>{bid}</color>";
    }
}
