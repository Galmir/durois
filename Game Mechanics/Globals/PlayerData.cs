using Godot;

public partial class PlayerData : Node {
    private static PlayerData _instance;
    public static PlayerData Instance => _instance;

    public Inventory PlayerInventory { get; set; } = (Inventory)GD.Load("uid://1qd00mmnbmaj"); // temporary ID of the inventory
    public PlayerData CombatData { get; private set; }// = GD.Load<PlayerData>("uid://yryau8tiehx6");


    public override void _Ready() {
        if (_instance != null) {
            _instance.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        _instance = this;
    }
}
