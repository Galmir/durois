using Godot;
using System;

[GlobalClass]
public partial class ShopSlotUIButton : Button {

    public float PriceModifier = 1.0f;
    public int Price = 1;

    private InventorySlot _slotToShow;
    public InventorySlot SlotToShow {
        get { return _slotToShow; }
        set {
            _slotToShow = value;
            Amount.Visible = false;
            itemIcon.Texture = _slotToShow.Item.Icon;
            Amount.Text = _slotToShow.Amount.ToString();
            if (_slotToShow.Item.IsStackable) {
                Amount.Visible = true;
            }
            Price = (int)Math.Clamp(Math.Round(_slotToShow.Item.GoldValue * PriceModifier * _slotToShow.Amount), 1, 9999);
            priceLabel.Text = Price.ToString();
        }
    }

    [Export] public Label Amount;
    [Export] private PanelContainer rarityBG;
    [Export] private TextureRect itemIcon;
    [Export] private Label priceLabel;

    public Action<ShopSlotUIButton> ItemSelected;

    public GridContainer StartingGrid;
    public GridContainer CurrentGrid;


    public override void _Ready() {
        Pressed += _Pressed;
    }


    public override void _Pressed() {
        ItemSelected.Invoke(this);
    }
}
