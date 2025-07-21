using Godot;

public partial class SaveData : Resource {
    public Inventory PlayerInventory { get; set; }
    public PlayerData CombatData { get; set; }
}
