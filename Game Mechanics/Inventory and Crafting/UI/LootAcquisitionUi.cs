using Godot;

public partial class LootAcquisitionUi : VBoxContainer {
    [Export] private PackedScene itemObtainedUi;
    [Export] private int maxItemsShownAtOnce;
    [Export] private Timer recursiveTimer;
    private Godot.Collections.Array<InventorySlot> itemQueue;

    public override void _Ready() {
        itemQueue = [];
        base._Ready();
        recursiveTimer.Timeout += ShowItemObtained;
        PlayerData.Instance.PlayerInventory.ItemObtained += addItemSlotToQueue;
        recursiveTimer.Start();

    }

    public void ShowItemObtained() {
        if (itemQueue.Count > 0 && GetChildCount() == 1) {
            for (int i = 0; i < maxItemsShownAtOnce; i++) {
                if (itemQueue.Count > 0) {
                    ItemObtainedUi newItem = itemObtainedUi.Instantiate<ItemObtainedUi>();
                    AddChild(newItem);
                    newItem.ObtainedItem = itemQueue[0];
                    itemQueue.RemoveAt(0);
                }
                else break;

            }


        }
        recursiveTimer.Start();
    }

    public void addItemSlotToQueue(InventorySlot item) {
        itemQueue.Add(item);
    }
}
