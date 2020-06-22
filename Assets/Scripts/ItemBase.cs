using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class ItemBase // should be made abstract to be used as a base. maybe even turned into a scriptable object
{
    public int itemBaseId;
    public int stackSize;
    public int usedSlot;

    public ItemBase(int id, int stack, int slot)
    {
        itemBaseId = id;
        stackSize = stack;
        usedSlot = slot;
    }
    public ItemBase()
    {

    }

    public string GetItemName()
    {
        return ItemDB._instance.GetName(itemBaseId);
    }

    public Sprite GetItemSprite()
    {
        return ItemDB._instance.GetItemSprite(itemBaseId);
    }

    //TODO: add baseitem database
    // Proof of concept field, should contain actual game data. base data should come from a baseitem database. this field should only contain modifiers for the base baseitem
    // Possibly should be removed
    //public string itemStatData;

    //TODO: sync baseitem database on login
    //TODO: setup interaction with baseitem database
    public string ToStringRichText()
    {
        ItemData data = ItemDB._instance.GetData(itemBaseId);
        StringBuilder b = new StringBuilder();
        b.AppendFormat("{0}", data.itemName);
        b.Append("\n");
        if (data.level == -1)
        {
            b.AppendFormat("Level {0} <{1}>{2}</color>", data.level, GetColor(data.rarity), Enum.GetName(typeof(ItemRarity), data.rarity));
        }
        else
        {
            b.AppendFormat("<{0}>{1}</color>", GetColor(data.rarity), Enum.GetName(typeof(ItemRarity), data.rarity));
        }
        b.Append(" ");
        b.AppendFormat("{0}", GetTypeName(data.type));
        b.Append("\n");
        //b.AppendFormat("<#DBDBDB>Item level: {0}</color>", itemLevel);
        //b.Append("\n");
        b.Append("\n");
        if (!data.baseStatData.IsNullOrEmpty())
        {
            b.AppendFormat("{0}", data.baseStatData);
            b.Append("\n");
            b.Append("\n");
        }
        b.AppendFormat("<i><#ffaa00>{0}</i></color>", data.itemDescription);
        b.Append("\n");
        b.Append("\n");
        b.AppendFormat("<#ffeca9>Sell Value: {0} Gold</color>", data.sellValue);
        b.Replace(";", "\n");

        return b.ToString();
    }

    private string GetColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return "#ffffff";
            case ItemRarity.Uncommon:
                return "#1eff00";
            case ItemRarity.Rare:
                return "#2f78ff";
            case ItemRarity.Epic:
                return "#a335ee";
            case ItemRarity.Legendary:
                return "#ff9600";
            case ItemRarity.Relic:
                return "#cf4747";
            default:
                return "000000";
        }
    }

    private string GetTypeName(ItemType type)
    {
        switch (type)
        {
            case ItemType.Armor:
                return "Armor";
            case ItemType.Consumable:
                return "Consumable";
            case ItemType.Crafting:
                return "Crafting Material";
            case ItemType.Equipment:
                return "Equipment";
            case ItemType.Jewelry:
                return "Jewelry";
            case ItemType.Trinket:
                return "Trinket";
            case ItemType.Weapon:
                return "Weapon";
            case ItemType.Error:
                return "AN ERROR HAS OCCURED, THIS SHOULD NOT SHOW UP";
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public ItemBase GetCopy(ItemBase copyFrom)
    {
        ItemBase item = new ItemBase
        {
            stackSize = copyFrom.stackSize,
            itemBaseId = copyFrom.itemBaseId,
            //itemStatData = copyFrom.itemStatData
        };

        return item;
    }

    public FilterOptions GetFilterOptions()
    {
        FilterOptions options = new FilterOptions();
        options.name = ItemDB._instance.GetName(itemBaseId);
        options.rarity = Enum.GetName(typeof(ItemRarity), ItemDB._instance.GetRarity(itemBaseId));
        options.type = Enum.GetName(typeof(ItemType), ItemDB._instance.GetType(itemBaseId));
        return options;
    }
}

public enum ItemType
{
    Armor,
    Consumable,
    Crafting,
    Equipment,
    Jewelry,
    Trinket,
    Weapon,
    Error
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Relic,
    Error
}