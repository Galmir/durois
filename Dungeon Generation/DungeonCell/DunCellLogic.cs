using Godot;

public partial class DunCellLogic : Node3D {
    [Export] MeshInstance3D wallN;
    [Export] MeshInstance3D wallS;
    [Export] MeshInstance3D wallW;
    [Export] MeshInstance3D wallE;

    [Export] MeshInstance3D columnNE;
    [Export] MeshInstance3D columnNW;
    [Export] MeshInstance3D columnSE;
    [Export] MeshInstance3D columnSW;

    [Export] Node3D doorWall;
    [Export] private int chanceToKeepTorches;

    // if there are cells in the mentioned cardinal directions, remove the wall. Keep the necessary columns
    public void OpenWalls(bool north, bool south, bool west, bool east, bool isDoor, Rules.CardinalDirections hallwayDirection) {
        if (north) wallN.QueueFree();
        if (south) wallS.QueueFree();
        if (west) wallW.QueueFree();
        if (east) wallE.QueueFree();

        // remove columns on empty spaces
        if (north && east) columnNE.QueueFree();
        if (north && west) columnNW.QueueFree();
        if (south && east) columnSE.QueueFree();
        if (south && west) columnSW.QueueFree();

        // remove overlapping columns
        if (north) columnNW?.QueueFree();
        if (west) columnSW?.QueueFree();
        if (south) columnSE?.QueueFree();
        if (east) columnNE?.QueueFree();

        if (!isDoor) doorWall.QueueFree();
        else {
            switch (hallwayDirection) {
                case Rules.CardinalDirections.N:
                    doorWall.Rotate(Vector3.Up, 0);
                    wallN.QueueFree();
                    columnNW.QueueFree();
                    columnNE.QueueFree();
                    break;
                case Rules.CardinalDirections.W:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(90));
                    wallW.QueueFree();
                    columnNW.QueueFree();
                    columnSW.QueueFree();
                    break;
                case Rules.CardinalDirections.S:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(180));
                    wallS.QueueFree();
                    columnSW.QueueFree();
                    columnSE.QueueFree();
                    break;
                case Rules.CardinalDirections.E:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(-90));
                    wallE.QueueFree();
                    columnNE.QueueFree();
                    columnSE.QueueFree();
                    break;

            }
        }
        KeepTorches();
    }

    public void KeepTorches() {
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
