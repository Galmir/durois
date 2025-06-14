using Godot;
using Godot.Collections;
using System.Linq;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

[Tool]
public partial class DungeonGenerator : Node3D {

    public bool start;
    [Export]
    private bool _start {
        get {
            return start;
        }
        set {
            if (Engine.IsEditorHint()) GenerateDungeon();
        }
    }

    [ExportCategory("Assets Dependency")]
    [Export] private GridMap gridMap;
    [Export] private Resource dungeonMesh;
    [Export] private PackedScene playerController;

    [ExportCategory("Room properties")]
    [Export] private string customSeed;
    [Export] private uint debugSeed = 0;
    [Export] private int borderSize = 50;
    [Export] private int floors = 1;
    [Export] private int roomsNumber = 15;
    [Export] private int roomMargin = 1;
    [Export] private int roomRecursionAttempts = 15;
    [Export] private int minRoomSize = 5;
    [Export] private int maxRoomSize = 10;
    [Export] private float roomSurvivalChance = 25f; // chance to not remove a room from the minimum spanning tree

    private Array<Array<Vector3>> roomTiles = [];
    private Array<Vector3> roomPositions;
    private bool isEntrancePlaced;
    private bool isExitPlaced;
    private int roomsMade;
    private Vector3I entrancePosition;
    private int currentFloor;

    public enum Tiles {
        Room,
        Hallway,
        Door,
        Border,
        Wall,
        Entrance,
        Exit
    }

    public Dictionary<Tiles, int> TileDict = new Dictionary<Tiles, int>() {
        {Tiles.Room, 0},
        { Tiles.Hallway, 1},
        { Tiles.Door, 2},
        { Tiles.Border, 3},
        { Tiles.Wall, 4},
        { Tiles.Entrance, 5},
        { Tiles.Exit, 6}
    };


    public override void _Ready() {
    }


    public void VisualizeBorder() {
        gridMap.Clear();
        for (int i = -1; i < borderSize + 1; i++) {
            gridMap.SetCellItem(new Vector3I(i, 0, -1), TileDict[Tiles.Border]);
            gridMap.SetCellItem(new Vector3I(i, 0, borderSize), TileDict[Tiles.Border]);
            gridMap.SetCellItem(new Vector3I(borderSize, 0, i), TileDict[Tiles.Border]);
            gridMap.SetCellItem(new Vector3I(-1, 0, i), TileDict[Tiles.Border]);
        }

    }


    private void GenerateDungeon() {
        VisualizeBorder();

        uint seed;
        if (customSeed is not null && customSeed.Length > 0) seed = customSeed.Hash();
        else seed = GD.Randi();

        if (debugSeed > 0) seed = debugSeed;
        GD.Seed(seed);
        GD.Print("generating dungeon from seed: " + seed);

        for (int i = 0; i < floors; i++) {
            GenerateFloor(i);
        }



        GD.Print("Dungeon Meshi... Oh, Dungeon Meshi!");
    }

    private void GenerateFloor(int floor) {
        roomTiles.Clear();
        roomPositions = [];
        roomsMade = 0;
        currentFloor = floor;

        isEntrancePlaced = false;
        isExitPlaced = false;
        for (int i = 0; i < roomsNumber; i++) {
            MakeRoom(roomRecursionAttempts);
            roomsMade++;
        }

        //Place Entrance

        while (!isEntrancePlaced) {
            Vector3I randomStartingPosition = (Vector3I)roomTiles.PickRandom().PickRandom();
            if (gridMap.GetCellItem(randomStartingPosition) == TileDict[Tiles.Room]) {
                gridMap.SetCellItem(randomStartingPosition, TileDict[Tiles.Entrance]);
                isEntrancePlaced = true;
                entrancePosition = randomStartingPosition;

            }

        }

        while (!isExitPlaced) {
            Vector3I randomStartingPosition = (Vector3I)roomTiles.PickRandom().PickRandom();
            if (gridMap.GetCellItem(randomStartingPosition) == TileDict[Tiles.Room] &&
            entrancePosition.DistanceSquaredTo(randomStartingPosition) / 10 > (int)borderSize / 2) {
                gridMap.SetCellItem(randomStartingPosition, TileDict[Tiles.Exit]);
                isExitPlaced = true;
            }

        }


        // Connect them together
        Array<Vector2> map2D = [];   // converted gridmap in 2D
        AStar2D delGraph = new(); // Delaunay triangulation graph
        AStar2D mstGraph = new(); // minimum spanning tree graph
        foreach (Vector3 room in roomPositions) {
            Vector2 newRoom = new Vector2(room.X, room.Z);
            map2D.Add(newRoom);
            delGraph.AddPoint(delGraph.GetAvailablePointId(), newRoom);
            mstGraph.AddPoint(mstGraph.GetAvailablePointId(), newRoom);
        }
        // triangulate the rooms
        Vector2[] arrayConversion = map2D.ToArray();
        int[] delaunay = Geometry2D.TriangulateDelaunay(arrayConversion);
        for (int i = 0; i < delaunay.Count(); i += 3) {
            int p1 = delaunay[i];
            int p2 = delaunay[i + 1];
            int p3 = delaunay[i + 2];
            delGraph.ConnectPoints(p1, p2);
            delGraph.ConnectPoints(p2, p3);
            delGraph.ConnectPoints(p3, p1);
        }

        // connect the rooms logically
        Array<int> visitedPoints = [];
        visitedPoints.Add(GD.RandRange(0, roomPositions.Count()));
        while (visitedPoints.Count() != mstGraph.GetPointCount()) {
            Array<int[]> possibleConnections = [];
            foreach (int vp in visitedPoints) {
                foreach (int c in delGraph.GetPointConnections(vp)) {
                    if (!visitedPoints.Contains(c)) {
                        int[] con = [vp, c];
                        possibleConnections.Add(con);
                    }
                }
            }

            int[] connection = possibleConnections.PickRandom();
            foreach (int[] pc in possibleConnections) {
                if (map2D[pc[0]].DistanceSquaredTo(map2D[pc[1]]) < map2D[connection[0]].DistanceSquaredTo(map2D[connection[1]])) {
                    connection = pc;
                }
            }
            visitedPoints.Add(connection[1]);
            mstGraph.ConnectPoints(connection[0], connection[1]);
            delGraph.ConnectPoints(connection[0], connection[1]);
        }

        AStar2D hallwayGraph = mstGraph;

        foreach (long point in delGraph.GetPointIds()) {
            foreach (long connection in delGraph.GetPointConnections(point)) {
                if (connection > point) {
                    float kill = GD.Randf();
                    if (roomSurvivalChance > kill) hallwayGraph.ConnectPoints(point, connection);
                }
            }
        }
        CreateHallways(hallwayGraph);
    }
    private void CreateHallways(AStar2D hallwayGraph) {
        Array<Vector3[]> hallways = [];
        foreach (int point in hallwayGraph.GetPointIds()) {
            foreach (int connection in hallwayGraph.GetPointConnections(point)) {
                if (connection > point) {
                    Array<Vector3> roomFrom = roomTiles[point];
                    Array<Vector3> roomTo = roomTiles[connection];
                    Vector3 tileFrom = roomFrom[0];
                    Vector3 tileTo = roomTo[0];
                    // find the closet tile to the connected toom
                    foreach (Vector3 tile in roomFrom) {
                        if (tile.DistanceSquaredTo(roomPositions[connection]) <
                        tileFrom.DistanceSquaredTo(roomPositions[connection])) tileFrom = tile;
                    }
                    foreach (Vector3 tile in roomTo) {
                        if (tile.DistanceSquaredTo(roomPositions[point]) <
                        tileTo.DistanceSquaredTo(roomPositions[point])) tileTo = tile;
                    }

                    Vector3[] hallway = [tileFrom, tileTo];
                    hallways.Add(hallway);
                    gridMap.SetCellItem((Vector3I)tileFrom, TileDict[Tiles.Door]);
                    gridMap.SetCellItem((Vector3I)tileTo, TileDict[Tiles.Door]);
                }
            }
        }

        AStarGrid2D astar = new();
        astar.Region = new Rect2I(Vector2I.Zero, Vector2I.One * borderSize);
        astar.Update();
        astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        astar.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;

        // block the pathfinding from walking into rooms
        foreach (Vector3I tile in gridMap.GetUsedCellsByItem(TileDict[Tiles.Room]) +
        gridMap.GetUsedCellsByItem(TileDict[Tiles.Wall])) astar.SetPointSolid(new Vector2I(tile.X, tile.Z));

        foreach (Vector3[] hallway in hallways) {
            Vector2I posFrom = new Vector2I((int)hallway[0].X, (int)hallway[0].Z);
            Vector2I posTo = new Vector2I((int)hallway[1].X, (int)hallway[1].Z);
            Vector2[] hall = astar.GetPointPath(posFrom, posTo);

            foreach (Vector2 i in hall) {
                Vector3I position = new Vector3I((int)i.X, 0, (int)i.Y);
                if (gridMap.GetCellItem(position) < 0) {
                    gridMap.SetCellItem(position, TileDict[Tiles.Hallway]);
                }
            }
        }


    }

    private void MakeRoom(int attemptsLeft) {
        if (attemptsLeft < 1) return;

        int width = GD.RandRange(minRoomSize, maxRoomSize);
        int height = GD.RandRange(minRoomSize, maxRoomSize);

        Vector3I startPos = new Vector3I();
        startPos.X = GD.RandRange(0, borderSize - width);
        startPos.Z = GD.RandRange(0, borderSize - height);

        // check for space availability
        for (int row = -roomMargin; row < height + roomMargin; row++) {
            for (int col = -roomMargin; col < width + roomMargin; col++) {
                Vector3I pos = startPos + new Vector3I(col, 0, row);
                if (gridMap.GetCellItem(pos) == TileDict[Tiles.Room] || gridMap.GetCellItem(pos) == TileDict[Tiles.Wall]) {
                    MakeRoom(attemptsLeft - 1);
                    return;
                }
            }
        }

        // make the actual room
        Array<Vector3> room = [];
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                Vector3I pos = startPos + new Vector3I(col, 0, row);
                if (col == 0 || col == width - 1 || row == 0 || row == height - 1) gridMap.SetCellItem(pos, TileDict[Tiles.Wall]);
                else gridMap.SetCellItem(pos, TileDict[Tiles.Room]);
                room.Add(pos);
            }
        }
        roomTiles.Add(room);

        float avgX = startPos.X + (float)(width / 2);
        float avgZ = startPos.Z + (float)(height / 2);
        Vector3 roomCenter = new Vector3(avgX, 0, avgZ);
        roomPositions.Add(roomCenter);
    }
}
