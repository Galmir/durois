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
                    break;
                case Rules.CardinalDirections.W:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(90));
                    break;
                case Rules.CardinalDirections.S:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(180));
                    break;
                case Rules.CardinalDirections.E:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(-90));
                    break;

            }
        }
    }
}
