
using Godot;

public partial class MerchantShopUi : PanelContainer {
    [Export]
    public PackedScene shopSlotUIscene;
    [Export]
    public Inventory merchantInventory;
    [Export]
    public Inventory PlayerInventory;
    [Export]
    public float sellRate = 1.0f;
    [Export]
    public float buyRate = 0.1f;

    private GridContainer merchantInventoryGrid;
    private GridContainer tradingInventoryGrid;
    private GridContainer playerInventoryGrid;
    private Label merchantGoldLabel;
    private Label tradingGoldLabel;
    private Label playerGoldLabel;
    private Button acceptButton;
    private Button exitButton;

    private int _merchantGold;
    private int _tradingGold;
    private int _playerGold;


    public int MerchantGold {
        get { return _merchantGold; }
        set {
            _merchantGold = value;
            merchantGoldLabel.Text = _merchantGold.ToString();
        }
    }


    public int TradingGold {
        get { return _tradingGold; }
        set {
            _tradingGold = value;
            tradingGoldLabel.Text = _tradingGold.ToString();
        }
    }


    public int PlayerGold {
        get { return _playerGold; }
        set {
            _playerGold = value;
            merchantGoldLabel.Text = _playerGold.ToString();
        }
    }


    public override void _Ready() {
        merchantInventoryGrid = GetNode<GridContainer>("%MerchantInventoryGrid");
        tradingInventoryGrid = GetNode<GridContainer>("%TradingInventoryGrid");
        playerInventoryGrid = GetNode<GridContainer>("%PlayerInventoryGrid");
        merchantGoldLabel = GetNode<Label>("%MerchantGoldLabel");
        tradingGoldLabel = GetNode<Label>("%TradingGoldLabel");
        playerGoldLabel = GetNode<Label>("%PlayerGoldLabel");
        acceptButton = GetNode<Button>("%AcceptButton");
        acceptButton.Pressed += AcceptTransaction;
        exitButton = GetNode<Button>("%ExitButton");
        exitButton.Pressed += QueueFree;

        FillInventory(merchantInventoryGrid, merchantInventory, sellRate);
        FillInventory(playerInventoryGrid, PlayerInventory, buyRate);

        MerchantGold = merchantInventory.Gold;
        TradingGold = 0;
        PlayerGold = PlayerInventory.Gold;

    }

    private void FillInventory(GridContainer grid, Inventory inventoryToLoad, float priceModifier) {
        foreach (InventorySlot slot in inventoryToLoad.InventorySlots) {
            ShopItemSlotUi newSlot = shopSlotUIscene.Instantiate<ShopItemSlotUi>();
            newSlot.PriceModifier = priceModifier;
            newSlot.SlotToShow = slot;
            grid.AddChild(newSlot);
            newSlot.StartingGrid = grid;
            newSlot.ItemSelected += AddOrRemoveFromTrading;
        }
    }

    private void AddOrRemoveFromTrading(ShopItemSlotUi slot) {
        if (slot.GetParent<GridContainer>() != tradingInventoryGrid) {
            if (slot.StartingGrid == merchantInventoryGrid) {
                slot.Reparent(tradingInventoryGrid);
                TradingGold -= slot.Price;
            }
            else {

                if (slot.StartingGrid == playerInventoryGrid) {
                    slot.Reparent(tradingInventoryGrid);
                    TradingGold += slot.Price;
                }
            }
        }
        else {

            if (slot.StartingGrid == merchantInventoryGrid) {
                slot.Reparent(merchantInventoryGrid);
                TradingGold += slot.Price;
            }
            else {

                if (slot.StartingGrid == playerInventoryGrid) {
                    slot.Reparent(playerInventoryGrid);
                    TradingGold -= slot.Price;
                }
            }
        }
    }

    private void AcceptTransaction() {
        if (TradingGold < 0 && PlayerGold >= TradingGold) { // confirm purchase
            PlayerGold -= TradingGold;
            MerchantGold += TradingGold;
            TradingGold = 0;
            SendItemsToInventories();
        }
        else if (TradingGold >= 0 && MerchantGold >= TradingGold) { // confirm sale
            PlayerGold += TradingGold;
            MerchantGold -= TradingGold;
            TradingGold = 0;
            SendItemsToInventories();
        }
    }


    private void SendItemsToInventories() {
        foreach (ShopItemSlotUi slot in tradingInventoryGrid.GetChildren()) {
            if (slot.StartingGrid == merchantInventoryGrid) {
                slot.Reparent(playerInventoryGrid);
                slot.StartingGrid = playerInventoryGrid;
            }
            else {
                slot.Reparent(merchantInventoryGrid);
                slot.StartingGrid = merchantInventoryGrid;
            }
        }
        UpdateInventoryFromGrid(merchantInventory, MerchantGold, merchantInventoryGrid);
        UpdateInventoryFromGrid(PlayerInventory, PlayerGold, playerInventoryGrid);
    }


    private void UpdateInventoryFromGrid(Inventory inventory, int goldFrom, GridContainer from) {
        inventory.ClearInventory();
        inventory.Gold = goldFrom;
        foreach (ShopItemSlotUi slot in from.GetChildren()) {
            inventory.AddItem(slot.SlotToShow.Item, slot.SlotToShow.Amount);
        }
    }
}
