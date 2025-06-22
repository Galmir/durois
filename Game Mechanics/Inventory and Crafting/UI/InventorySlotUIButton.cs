using Godot;

[GlobalClass]
public partial class InventorySlotUIButton : Button {

    private InventorySlot _slotToShow;

    [Export]
    public InventorySlot SlotToShow {
        get { return _slotToShow; }
        set {
            _slotToShow = value;
            Amount.Visible = false;
            this.Icon = _slotToShow.Item.Icon;
            Amount.Text = _slotToShow.Amount.ToString();
            if (_slotToShow.Item.IsStackable) {
                Amount.Visible = true;
            }
        }
    }

    [Export] public Label Amount;

    [Signal]
    public delegate void ItemSelectedEventHandler(InventoryItem item);


    public override void _Ready() {
        Amount = GetNode<Label>("ItemAmountLabel");
        this.Pressed += _Pressed;
    }


    public override void _Pressed() {
        EmitSignal(SignalName.ItemSelected, SlotToShow.Item);
    }
}
