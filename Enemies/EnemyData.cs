using Godot;

[GlobalClass]
public partial class EnemyData : Resource {
    [Export] public GameConstants.EnemyIDs EnemyID;
    [Export] public string Name;
    [Export] public Inventory Rewards;
}
