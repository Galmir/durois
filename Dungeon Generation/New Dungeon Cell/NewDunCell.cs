using Godot;


public partial class NewDunCell : Node3D {
    [Export] StaticBody3D wallN;
    [Export] StaticBody3D wallS;
    [Export] StaticBody3D wallW;
    [Export] StaticBody3D wallE;

    [Export] Node3D doorWall;
    [Export] private int chanceToKeepTorches;

    // if there are cells in the mentioned cardinal directions, remove the wall. Keep the necessary columns
    public void OpenWalls(bool north, bool south, bool west, bool east, bool isDoor, Rules.CardinalDirections hallwayDirection) {
        if (north) wallN.QueueFree();
        if (south) wallS.QueueFree();
        if (west) wallW.QueueFree();
        if (east) wallE.QueueFree();

        if (!isDoor) doorWall.QueueFree();
        else {
            switch (hallwayDirection) {
                case Rules.CardinalDirections.N:
                    doorWall.Rotate(Vector3.Up, 0);
                    wallN.QueueFree();
                    break;
                case Rules.CardinalDirections.W:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(90));
                    wallW.QueueFree();
                    break;
                case Rules.CardinalDirections.S:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(180));
                    wallS.QueueFree();
                    break;
                case Rules.CardinalDirections.E:
                    doorWall.Rotate(Vector3.Up, Mathf.DegToRad(-90));
                    wallE.QueueFree();
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
