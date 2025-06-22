using System;
using Godot;

public partial class ShopItemSlotUi : Button {
    public float PriceModifier = 1.0f;
    public int Price = 1;
    public delegate void ItemSelectedEventHandler(ShopItemSlotUi itemSelected);
    public event ItemSelectedEventHandler ItemSelected;

    private InventorySlot _slotToShow;
    [Export]
    public InventorySlot SlotToShow {
        get { return _slotToShow; }
        set {
            _slotToShow = value;
            Icon = _slotToShow.Item.Icon;
            amountLabel = GetNode<Label>("%ItemAmountLabel");
            priceLabel = GetNode<Label>("%ItemPriceLabel");
            amountLabel.Visible = false;
            amountLabel.Text = _slotToShow.Amount.ToString();
            if (_slotToShow.Item.IsStackable) {
                amountLabel.Visible = true;
            }
            Price = (int)Math.Clamp(Math.Round(_slotToShow.Item.GoldValue * PriceModifier * _slotToShow.Amount), 1, 9999);
            priceLabel.Text = Price.ToString();
        }
    }

    private Label amountLabel;
    private Label priceLabel;
    public GridContainer StartingGrid;
    public GridContainer CurrentGrid;


    public override void _Ready() {
    }


    public override void _Pressed() {
        ItemSelected.Invoke(this);
    }

}

