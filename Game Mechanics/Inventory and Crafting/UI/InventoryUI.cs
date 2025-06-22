using Godot;

public partial class InventoryUI : Control {
    [Export] public PackedScene InvSlotUIScene;
    private GridContainer inventoryGrid;
    private Label itemName;
    private TextureRect itemIcon;
    private RichTextLabel itemDescription;
    private Label goldAmountLabel;
    private Button weaponFilterButton;
    private Button equipButton;
    private InventoryItem selectedItem;


    public override void _Ready() {
        //get all the components
        inventoryGrid = GetNode<GridContainer>("%InventoryGrid");
        itemName = GetNode<Label>("%ItemNameLabel");
        itemIcon = GetNode<TextureRect>("%ItemIconTexture");
        itemDescription = GetNode<RichTextLabel>("%ItemDescriptiontLabel");
        goldAmountLabel = GetNode<Label>("%GoldAmountLabel");
        weaponFilterButton = GetNode<Button>("%WeaponFilterButton");
        equipButton = GetNode<Button>("%EquipButton");
        equipButton.Pressed += EquipItem;
        FillInventory();
        this.VisibilityChanged += _OnVisibilityChange;
    }

    private void CleanInventory() {
        foreach (Node child in inventoryGrid.GetChildren()) {
            child.Free();
        }
    }

    private void ClearDescription() {
        selectedItem = null;
        itemName.Text = "";
        itemIcon.Texture = null;
        itemDescription.Text = "";
        equipButton.Visible = false;
    }


    private void FillInventory() {
        CleanInventory();
        ClearDescription();
        foreach (InventorySlot slot in PlayerData.Instance.PlayerInventory.InventorySlots) {

            InventorySlotUIButton newSlot = InvSlotUIScene.Instantiate<InventorySlotUIButton>();
            inventoryGrid.AddChild(newSlot);
            newSlot.SlotToShow = slot;
            newSlot.ItemSelected += UpdateItemDescription;
        }
        // focus on the first item for controller use
        if (inventoryGrid.GetChildCount() > 0) inventoryGrid.GetChild<InventorySlotUIButton>(0).GrabFocus();
    }


    private void FilterInventoryByTag(Rules.ItemTags filterTag = Rules.ItemTags.ALL) {
        foreach (InventorySlotUIButton slot in inventoryGrid.GetChildren()) {
            if (slot.SlotToShow.Item.FilterTags.Contains(filterTag) || filterTag == Rules.ItemTags.ALL) slot.Visible = true;
            else slot.Visible = false;
        }
        goldAmountLabel.Text = PlayerData.Instance.PlayerInventory.Gold.ToString();
    }


    private void EquipItem() {
        // TODO
    }


    private void UpdateItemDescription(InventoryItem item) {
        selectedItem = item;
        itemName.Text = item.Name;
        itemIcon.Texture = item.Icon;
        itemDescription.Text = item.GetFullDescription();
        if (item is EquippableItem) {
            equipButton.Visible = true;
        }
    }

    private void _OnVisibilityChange() {
        FillInventory();
    }
}
