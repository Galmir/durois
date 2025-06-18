using Godot;
using Godot.Collections;

public partial class Rules : Node {


    public enum Tiles {
        Floor,
        Hallway,
        Door,
        Border,
        Wall,
        Entrance,
        Exit
    }

    public Dictionary<Tiles, int> TileDict = new Dictionary<Tiles, int>() {
        {Tiles.Floor, 0},
        { Tiles.Hallway, 1},
        { Tiles.Door, 2},
        { Tiles.Border, 3},
        { Tiles.Wall, 4},
        { Tiles.Entrance, 5},
        { Tiles.Exit, 6}
    };

    private static Rules _instance;
    public static Rules Instance => _instance;

    public override void _EnterTree() {
        if (_instance != null) Instance.QueueFree();
        _instance = this;
    }
}
