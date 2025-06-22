using Godot;

[GlobalClass]
public partial class InventorySlot : Resource {
    [Export] public InventoryItem Item;
    [Export]
    public int Amount {
        get { return _amount; }
        set {
            Mathf.Clamp(value, 1, 999);
        }
    }
    private int _amount = 1;
}
