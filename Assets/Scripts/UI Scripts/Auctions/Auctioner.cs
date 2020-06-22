using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using WebSocketSharp;
using Button = UnityEngine.UI.Button;

public class Auctioner : SerializedMonoBehaviour
{
    public TMP_InputField amount;
    public TMP_InputField perUnit;
    public TMP_InputField total;
    public TextMeshProUGUI buttonText;
    public TMP_InputField buyout;
    private AuctionCreationObject auction = new AuctionCreationObject();
    [SerializeField]
    private ItemBase selectedItem;
    public TMP_Dropdown itemSelection;

    public TextMeshProUGUI itemStatDisplayText;

    public Button buttonConfirm;

    [SerializeField]
    private Dictionary<int, int> dropdownItemTracker = new Dictionary<int, int>();

    private bool canAuction = true;

    public void Start()
    {
        itemSelection.onValueChanged.AddListener(delegate {
            DropdownValueChanged(itemSelection);
        });
    }

    public void ToggleBuyout()
    {
        auction.allowBuyout = !auction.allowBuyout;
        buyout.interactable = auction.allowBuyout;
        buttonText.text = auction.allowBuyout ? "Yes" : "No";
    }

    public void StartNewAuctionCreation()
    {
        canAuction = true;
        auction = new AuctionCreationObject();
        selectedItem = null;
        dropdownItemTracker.Clear();
        itemSelection.interactable = true;
        amount.text = "1";
        perUnit.text = "1";
        total.text = "1";
        buttonText.text = "No";
        buyout.text = "0";
        buyout.interactable = false;
        itemStatDisplayText.text = "";

        PopulateDropdown();
    }

    private void PopulateDropdown()
    {
        dropdownItemTracker.Clear();
        itemSelection.ClearOptions();
        if (PlayerInventoryHolder._instance.GetInventory().Count > 0)
        {
            foreach (var itemBase in PlayerInventoryHolder._instance.GetInventory())
            {
                itemSelection.options.Add(new TMP_Dropdown.OptionData()
                {
                    text = itemBase.GetItemName(),
                    image = itemBase.GetItemSprite()
                });
                dropdownItemTracker.Add(itemSelection.options.Count -1, itemBase.usedSlot);
            }
        }
        else
        {
            canAuction = false;
            itemSelection.interactable = false;
            buttonConfirm.interactable = false;
        }

        if (itemSelection.options.Count > 0)
        {
            itemSelection.value = -1;
            buttonConfirm.interactable = true;
            SelectItem(0);
        }
    }

    public void DropdownValueChanged(TMP_Dropdown change)
    {
        SelectItem(change.value);
    }

    private void SelectItem(int index)
    {
        foreach (var item in PlayerInventoryHolder._instance.GetInventory())
        {
            if (item.usedSlot == dropdownItemTracker[index])
            {
                selectedItem = item;
            }
        }

        itemStatDisplayText.text = selectedItem.ToStringRichText();
        RecalculateTotalField();
    }

    public void RecalculateTotalField()
    {
        if (!canAuction) return;
        if (amount.text.IsNullOrEmpty() || perUnit.text.IsNullOrEmpty() || amount.text.Equals("-") || perUnit.text.Equals("-"))
        {
            total.text = "0";
            buttonConfirm.interactable = false;
            return;
        }
        if (selectedItem != null)
        {
            if (int.Parse(amount.text) > selectedItem.stackSize)
            {
                amount.text = selectedItem.stackSize.ToString();
            }

            if (int.Parse(amount.text) <= 0)
            {
                amount.text = "1";
            }

            if (int.Parse(perUnit.text) <= 0)
            {
                perUnit.text = "1";
            }

            total.text = (int.Parse(amount.text) * int.Parse(perUnit.text)).ToString();
            buttonConfirm.interactable = true;
            return;
        }
        total.text = "0";
        buttonConfirm.interactable = false;
    }

    public bool PublishAuction()
    {
        if (selectedItem != null)
        {
            auction.amount = int.Parse(amount.text);
            auction.pricePerUnit = int.Parse(perUnit.text);
            auction.itemId = selectedItem.itemBaseId;
            auction.filterOptions = ItemDB._instance.GetFilterOptions(auction.itemId);
            if (auction.allowBuyout)
            {
                auction.filterOptions.buyout = true;
                auction.allowBuyout = true;
                auction.buyoutPrice = int.Parse(buyout.text);
            }
            AuctionRestCommunication._instance.CreateAuction(auction);
            return true;
        }

        MessageDisplayer._instance.DisplayMessage("Error: No selected item");
        return false;
    }
}

public class AuctionCreationObject
{
    public int amount;
    public int pricePerUnit;
    public bool allowBuyout;
    public int buyoutPrice;
    public int itemId;
    public FilterOptions filterOptions;
}
