using Godot;

[GlobalClass]
public partial class StatBonus : Resource {
    [Export] public Rules.Attributes Attribute;
    [Export] public int Amount;
}
