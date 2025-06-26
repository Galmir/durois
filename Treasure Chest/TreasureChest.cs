using Godot;

public partial class TreasureChest : Interactable {
    [Export] private LootTable loot;
    public override void _Ready() {
        Interacted += _OnInteract;
    }

    public override void _OnInteract(Node3D interacter) {
        GD.Print("loot");
    }
}
