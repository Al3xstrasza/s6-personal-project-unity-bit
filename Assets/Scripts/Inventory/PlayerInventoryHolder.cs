    using System;
    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;
    using WebSocketSharp;

    public class PlayerInventoryHolder : SerializedMonoBehaviour
{
    // invetory setup as int, itembase
    // int for slot id, itembase holds all item data=
    // look for existing stacks >
    // top these off >
    // when topped off and items are still to be added use first open item slot (look for not used int in dictionary)
    // when removing items look for last stack and start removing from here and work backwards
    //TODO: add logic for dragging items around
    // this has to communicate to the backend that items have been moved. playerInventoryHolder backend will be added later thus not needed yet

    [SerializeField]
    private TextMeshProUGUI currencyText;
    [SerializeField]
    private TextMeshProUGUI floatingText;

    [SerializeField]
    private int currency = 0;
    [SerializeField]
    private int currencyFloating = 0;
    [SerializeField]
    //private Dictionary<int, ItemBase> inventory = new Dictionary<int, ItemBase>();
    private List<ItemBase> inventory = new List<ItemBase>();

    public static PlayerInventoryHolder _instance;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<PlayerInventoryHolder>();
        }

        WebSocketDataHandler.inventoryChangeEvent.AddListener(ParseInventoryChange);
        WebSocketDataHandler.currencyChangeEvent.AddListener(ParseCurrencyChange);
    }

    private void ParseInventoryChange(InventoryChange change)
    {
        if (change.add)
        {
            AddItem(change.item);
            DisplayMessage($"{change.item.stackSize}x {change.item.GetItemName()} added to inventory");
        }
        else
        {
            RemoveItem(change.item);
            DisplayMessage($"{change.item.stackSize}x {change.item.GetItemName()} removed from inventory");
        }
    }

    private void ParseCurrencyChange(CurrencyChange change)
    {
        if (change.floating)
        {
            if (change.add)
            {
                UpdateFloating(change.amount);
            }
            else
            {
                UpdateFloating(change.amount * -1);
            }
        }
        else
        {
            if (change.add)
            {
                UpdateCurrency(change.amount);
                DisplayMessage($"{change.amount} currency added");
            }
            else
            {
                UpdateCurrency(change.amount * -1);
                DisplayMessage($"{change.amount} currency removed");
            }
        }
    }

    public void SetInventory(List<ItemBase> inventory)
    {
        this.inventory = inventory;
    }

    public void SetCurrency(int currency)
    {
        this.currency = currency;
        currencyText.text = currency.ToString();
    }
    public void SetFloating(int floating)
    {
        currencyFloating = floating;
        floatingText.text = currencyFloating.ToString();
    }

    public List<ItemBase> GetInventory()
    {
        return inventory;
    }

    public void UpdateCurrency(int change)
    {
        currency += change;
        currencyText.text = currency.ToString();
    }

    public void UpdateFloating(int change)
    {
        currencyFloating += change;
        floatingText.text = currencyFloating.ToString();
    }

    public void AddItem(ItemBase item)
    {
        //if (ItemDB._instance.GetStackable(item.itemBaseId))
        //{
            var addedToAStack = false;

            foreach (var itemBase in inventory.Where(itemBase => itemBase.itemBaseId == item.itemBaseId/* && ItemDB._instance.GetStackable(itemBase.itemBaseId)*/))
            {
                itemBase.stackSize += item.stackSize;
                addedToAStack = true;
                break;
            }

            if (addedToAStack) return;
            
        //}

        item.usedSlot = GetNextOpenSlot();
        inventory.Add(item);
    }

    private int GetNextOpenSlot()
    {
        int i = 0;
        bool foundSlot = false;
        while (!foundSlot)
        {
            if (inventory.Count(itemBase => itemBase.usedSlot == i) > 0)
            {
                i++;
            }
            else
            {
                foundSlot = true;
            }
        }

        return i;
    }

    // Returns -1 if item doesnt exist in playerInventoryHolder to remove
    // Returns 0 if playerInventoryHolder doesnt hold enough items to remove
    // Returns 1 if item was successfully removed
    public int RemoveItem(ItemBase item)
    {
        if (inventory.Count(itemBase => itemBase.itemBaseId == item.itemBaseId) > 0)
        {
            foreach (var itemBase in inventory.Where(itemBase => itemBase.itemBaseId == item.itemBaseId))
            {
                if (itemBase.stackSize < item.stackSize)
                {
                    return 0;
                }
                itemBase.stackSize -= item.stackSize;
                if (itemBase.stackSize == 0)
                {
                    inventory.Remove(itemBase);
                }
                return 1;
            }
        }
        return -1;
    }

    public void ClearData()
    {
        currency = 0;
        currencyFloating = 0;
        inventory = new List<ItemBase>();
    }

    private void DisplayMessage(string message)
    {
        if (message.IsNullOrEmpty()) return;
        MessageDisplayer._instance.DisplayMessage(message);
    }
}

[Serializable]
public class PlayerInventory
{
    public List<ItemBase> inventory = new List<ItemBase>();
    public string user;
}
