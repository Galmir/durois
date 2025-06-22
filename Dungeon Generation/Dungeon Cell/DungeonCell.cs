using Godot;

public partial class DungeonCell : Node3D {
    [Export] MeshInstance3D wallN;
    [Export] MeshInstance3D wallW;
    [Export] MeshInstance3D wallS;
    [Export] MeshInstance3D wallE;
    [Export] MeshInstance3D MiniMapTile;

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

    public void KeepTorches(int chanceToKeepTorches) {
        for (int i = 0; i < 4; i++) {
            int chance = GD.RandRange(1, 100);
            if (chance > chanceToKeepTorches) {
                switch (i) {
                    case 0:
                        if (wallN is not null) wallN.GetChild(0).QueueFree();
                        break;

                    case 1:
                        if (wallS is not null) wallS.GetChild(0).QueueFree();
                        break;

                    case 2:
                        if (wallW is not null) wallW.GetChild(0).QueueFree();
                        break;

                    case 3:
                        if (wallE is not null) wallE.GetChild(0).QueueFree();
                        break;
                }
            }
        }
    }
}
