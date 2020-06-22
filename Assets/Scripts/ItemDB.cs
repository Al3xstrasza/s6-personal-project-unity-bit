using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static ItemDB _instance;

    public ItemDatabase databaseAsset;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<ItemDB>();
        }
    }

    public ItemData GetData(int baseId)
    {
        return databaseAsset.GetAllData(baseId);
    }
    public string GetName(int baseId)
    {
        return databaseAsset.GetName(baseId);
    }
    //public bool GetStackable(int baseId)
    //{
    //    return databaseAsset.GetStackable(baseId);
    //}
    public int GetLevel(int baseId)
    {
        return databaseAsset.GetLevel(baseId);
    }
    public string GetBaseStatData(int baseId)
    {
        return databaseAsset.GetBaseStatData(baseId);
    }
    public string GetItemDescription(int baseId)
    {
        return databaseAsset.GetItemDescription(baseId);
    }
    public int GetSellValue(int baseId)
    {
        return databaseAsset.GetSellValue(baseId);
    }
    public Sprite GetItemSprite(int baseId)
    {
        return databaseAsset.GetItemSprite(baseId);
    }
    public ItemType GetType(int baseId)
    {
        return databaseAsset.GetType(baseId);
    }
    public ItemRarity GetRarity(int baseId)
    {
        return databaseAsset.GetRarity(baseId);
    }

    public FilterOptions GetFilterOptions(int baseId)
    {
        FilterOptions options = new FilterOptions();
        options.name = ItemDB._instance.GetName(baseId);
        options.rarity = Enum.GetName(typeof(ItemRarity), ItemDB._instance.GetRarity(baseId));
        options.type = Enum.GetName(typeof(ItemType), ItemDB._instance.GetType(baseId));
        return options;
    }
}
