using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabaseAsset", menuName = "ItemDatabaseAsset")]
public class ItemDatabase : SerializedScriptableObject
{
    [SerializeField]
    private Dictionary<int, ItemData> dataSet = new Dictionary<int, ItemData>();

    public ItemData GetAllData(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId] : null;
    }

    public string GetName(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].itemName : null;
    }
    //public bool GetStackable(int baseId)
    //{
    //    return dataSet.ContainsKey(baseId) && dataSet[baseId].stackable;
    //}
    public int GetLevel(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].level : -1;
    }
    public string GetBaseStatData(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].baseStatData : null;
    }
    public string GetItemDescription(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].itemDescription : null;
    }
    public int GetSellValue(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].sellValue : -1;
    }
    public Sprite GetItemSprite(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].itemSprite : null;
    }
    public ItemType GetType(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].type : ItemType.Error;
    }
    public ItemRarity GetRarity(int baseId)
    {
        return dataSet.ContainsKey(baseId) ? dataSet[baseId].rarity : ItemRarity.Error;
    }
}

[Serializable]
public class ItemData
{
    public string itemName;
    //public bool stackable;
    public int level;
    public string baseStatData;
    public string itemDescription;
    public int sellValue;
    public Sprite itemSprite;
    public ItemType type;
    public ItemRarity rarity;
}