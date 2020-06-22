using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class AuctionWindow : GameWindow
{
    [SerializeField]
    private GameObject panelCreation;
    [SerializeField]
    private GameObject panelListing;
    [SerializeField]
    private GameObject panelBidding;

    //Filter input
    [SerializeField]
    private TMP_InputField searchInput;
    [SerializeField]
    private TMP_Dropdown dropItemType;
    [SerializeField]
    private TMP_Dropdown dropItemRarity;
    [SerializeField]
    private TMP_Dropdown dropBuyout;
    [SerializeField]
    private Button filterButton;

    [SerializeField]
    private Button createAuctionButton;

    [SerializeField]
    private Auctioner auctioner;

    [SerializeField]
    private GameObject auctionUiPanelPrefab;
    [SerializeField]
    private Transform auctionListParent;

    [SerializeField]
    private ActiveAuction bidPanel;

    private bool auctionCreation = false;

    private bool auctionOpened = false;
    // Start is called before the first frame update
    private void Start()
    {
        panelCreation.SetActive(false);
        panelBidding.SetActive(false);
        panelListing.SetActive(true);
    }

    // Update is called once per frame
    private void OnEnable()
    {
        //FetchResults();
    }

    public void FetchResults()
    {
        //something with filters
        var options = new FilterOptions
        {
            name = searchInput.text,
            type = dropItemType.options[dropItemType.value].text,
            rarity = dropItemRarity.options[dropItemRarity.value].text
        };
        switch (dropBuyout.options[dropBuyout.value].text)
        {
            case "Either":
                options.buyout = false;
                break;
            case "Yes":
                options.buyout = true;
                break;
        }
        AuctionRestCommunication._instance.RetrieveFilteredAuctions(options, DisplayResults);
    }

    private void DisplayResults(string rawJson)
    {
        auctionListParent.Clear();
        if (rawJson == null || auctionCreation) return;
        var auctionCollection = JsonUtility.FromJson<AuctionCollection>(rawJson);
        foreach (var auction in auctionCollection.auctions)
        {
            var obj = Instantiate(auctionUiPanelPrefab, auctionListParent);
            AuctionUIPanel uiPanel = obj.gameObject.GetComponent<AuctionUIPanel>();
            uiPanel.SetupUiPanel(auction, OpenAuction, Buyout);
        }
    }

    public void Buyout(string auction)
    {
        //Debug.Log(auction.buyoutPrice);
    }

    public void OpenAuction(string auction)
    {
        // Maybe display message when still busy with opening active auction
        if (auctionOpened) return;
        Debug.Log("Auction ID: " + auction);
        AuctionRestCommunication._instance.GetUpToDateAuctionStateBeforeSubscribing(auction, StartActiveAuction);
    }

    private void StartActiveAuction(string auctionJson)
    {
        if (auctionJson.IsNullOrEmpty()) return;
        var auction = JsonUtility.FromJson<Auction>(auctionJson);
        if (!AuctionWebsocketCommunication._instance.SubscribeToAuction(auction.auctionId)) return;
        bidPanel.SetupActiveAuction(auction);
        auctionOpened = true;
        panelCreation.SetActive(false);
        panelBidding.SetActive(true);
        panelListing.SetActive(false);
    }

    public void CloseActiveAuction()
    {
        bidPanel.BreakAuction();
        panelCreation.SetActive(false);
        panelBidding.SetActive(false);
        panelListing.SetActive(true);
        auctionOpened = false;
        AuctionWebsocketCommunication._instance.UnsubscribeFromAuction();
    }

    public override void DoBeforeDisable()
    {
        if (auctionOpened)
        {
            CloseActiveAuction();
        }

        if (auctionCreation)
        {
            StopAuctionCreation();
        }
        EnableFilterInput();
        auctionListParent.Clear();
    }

    public void DisableFilterInput()
    {
        ToggleFilterInput(0);
    }

    public void EnableFilterInput()
    {
        ToggleFilterInput(1);
    }

    public void StartAuctionCreation()
    {
        if (auctionOpened)
        {
            CloseActiveAuction();
        }

        auctionCreation = true;
        ToggleFilterInput(0);
        panelCreation.SetActive(true);
        panelListing.SetActive(false);
        if (auctioner != null)
        {
            auctioner.StartNewAuctionCreation();
        }
    }

    public void PublishAuction()
    {
        if (!auctioner.PublishAuction()) return;
        auctionCreation = false;
        ToggleFilterInput(1);
        panelCreation.SetActive(false);
        panelListing.SetActive(true);
    }

    
    public void StopAuctionCreation()
    {
        auctionCreation = false;
        ToggleFilterInput(1);
        panelCreation.SetActive(false);
        panelListing.SetActive(true);
    }

    public void ToggleFilterInput(int force = -1)
    {
        // No force specified, default toggle behaviour.
        if (force == -1)
        {
            searchInput.interactable = !searchInput.IsInteractable();
            dropItemType.interactable = !dropItemType.IsInteractable();
            dropItemRarity.interactable = !dropItemRarity.IsInteractable();
            dropBuyout.interactable = !dropBuyout.IsInteractable();
            filterButton.interactable = !filterButton.IsInteractable();
            Image backgroundImage = searchInput.gameObject.GetComponent<Image>();
            if (filterButton.IsInteractable())
            {
                backgroundImage.color = Color.white;
            }
            else
            {
                backgroundImage.color = new Color(212f/255f, 212f/255f, 212f/255f);
            }
        }
        else
        {
            Image backgroundImage = searchInput.gameObject.GetComponent<Image>();
            // Force specified. disable on 0, enable on 1.
            if (force == 0)
            {
                searchInput.interactable = false;
                dropItemType.interactable = false;
                dropItemRarity.interactable = false;
                dropBuyout.interactable = false;
                filterButton.interactable = false;
                
            }
            else if (force == 1)
            {
                searchInput.interactable = true;
                dropItemType.interactable = true;
                dropItemRarity.interactable = true;
                dropBuyout.interactable = true;
                filterButton.interactable = true;
            }

            backgroundImage.color = filterButton.IsInteractable() ? Color.white : new Color(212f / 255f, 212f / 255f, 212f / 255f);
        }
    }
}
