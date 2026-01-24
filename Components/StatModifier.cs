using Godot;
using System;

[GlobalClass]
public partial class StatModifier : Resource {
    [Export] public float SpeedMultiplier = 1.0f;
    [Export] public float AttackMultiplier = 1.0f;
    [Export] public float DefenseMultiplier = 1.0f;
    [Export] public float CritChance = 0.0f;
    [Export] public float CritMultiplier = 1.5f;
}
