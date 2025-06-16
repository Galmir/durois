using Godot;
using System;

public partial class DungeonCell : Node3D {
    [Export] MeshInstance3D wallN;
    [Export] MeshInstance3D wallW;
    [Export] MeshInstance3D wallS;
    [Export] MeshInstance3D wallE;

    public enum Directions {
        North,
        South,
        West,
        East
    }

    public void RemoveWall(Directions dir) {
        switch (dir) {
            case Directions.North:
                wallN.QueueFree();
                break;
            case Directions.South:
                wallS.QueueFree();
                break;
            case Directions.West:
                wallW.QueueFree();
                break;
            case Directions.East:
                wallE.QueueFree();
                break;
        }
    }
}
