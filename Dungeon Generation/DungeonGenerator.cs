using Godot;
using Godot.Collections;
using System;
using System.Linq;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

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
    [Export] private GridMap monstersAndLootMap;
    [Export] private CharacterBody3D playerScene;
    private PackedScene dungeonCell = GD.Load<PackedScene>("uid://dffqonp6jrc1s");

    [ExportCategory("Dungeon")]
    [Export] private string customSeed;
    [Export] private uint debugSeed = 0;
    [Export] private int borderSize = 50;
    [Export] private int floors = 1;
    [Export] private int floorHeight = 2;
    [Export] private int roomsNumber = 15;

    [ExportCategory("Room")]
    [Export] private int roomMargin = 1;
    [Export] private int roomRecursionAttempts = 15;
    [Export] private int minRoomSize = 5;
    [Export] private int maxRoomSize = 10;
    [Export] private float roomSurvivalChance = 25f;
    [Export] private int maxMonstersPerRoom = 1; // chance to not remove a room from the minimum spanning tree
    [Export] private int chanceToKeepTorches = 25;

    private Array<Array<Vector3>> roomTiles = [];
    private Array<Vector3> roomPositions;
    private bool isEntrancePlaced = false;
    private bool isExitPlaced = false;
    private bool playerPlaced = false;
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
        GenerateDungeon();
    }


    private void GenerateDungeon() {
        GD.Print("generate dungeon");
        gridMap.Clear();
        monstersAndLootMap.Clear();
        uint seed;
        if (customSeed is not null && customSeed.Length > 0) seed = customSeed.Hash();
        else seed = GD.Randi();

        if (debugSeed > 0) seed = debugSeed;
        GD.Seed(seed);
        GD.Print("generating dungeon from seed: " + seed);

        for (int floor = 0; floor < floors; floor++) {
            GenerateFloor(floor);
        }

        makeRealDungeon();

        GD.Print("Dungeon Meshi... Oh, Dungeon Meshi!");
    }

    private void GenerateFloor(int floor) {
        GD.Print("generate floor");
        roomTiles.Clear();
        roomPositions = [];
        roomsMade = 0;
        currentFloor = floor * floorHeight;

        isEntrancePlaced = false;
        isExitPlaced = false;
        for (int i = 0; i < roomsNumber; i++) {
            GD.Print("Floor " + floor + ": making room " + i + "/" + roomsNumber);
            MakeRoom(roomRecursionAttempts);
            roomsMade++;
        }

        playerPlaced = false;
        while (!isEntrancePlaced) {
            Vector3I randomStartingPosition = (Vector3I)roomTiles.PickRandom().PickRandom();
            if (gridMap.GetCellItem(randomStartingPosition) == TileDict[Tiles.Room]) {
                gridMap.SetCellItem(randomStartingPosition, TileDict[Tiles.Entrance]);
                isEntrancePlaced = true;
                entrancePosition = randomStartingPosition;
                if (!playerPlaced) {
                    playerScene.Position = roomPositions[0];
                    ;
                    playerPlaced = true;
                }
            }

        }

        isExitPlaced = false;
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

            int[] connection = possibleConnections.PickRandom(); // how can this be empty?
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
        GD.Print("create hallways");
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
        foreach (Vector3I tile in gridMap.GetUsedCellsByItem(TileDict[Tiles.Room])) {
            if (tile.Y == currentFloor) astar.SetPointSolid(new Vector2I(tile.X, tile.Z));
        }

        foreach (Vector3[] hallway in hallways) {
            Vector2I posFrom = new Vector2I((int)hallway[0].X, (int)hallway[0].Z);
            Vector2I posTo = new Vector2I((int)hallway[1].X, (int)hallway[1].Z);
            Vector2[] hall = astar.GetPointPath(posFrom, posTo);

            foreach (Vector2 i in hall) {
                Vector3I position = new Vector3I((int)i.X, currentFloor, (int)i.Y);
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
        startPos.Y = currentFloor;

        // check for space availability
        for (int row = -roomMargin; row < height + roomMargin; row++) {
            for (int col = -roomMargin; col < width + roomMargin; col++) {
                Vector3I pos = startPos + new Vector3I(col, 0, row);
                if (gridMap.GetCellItem(pos) == TileDict[Tiles.Room]) {
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
                gridMap.SetCellItem(pos, TileDict[Tiles.Room]);
                room.Add(pos);
            }
        }
        int maxMonstersInThisRoom = maxMonstersPerRoom <= room.Count() ? maxMonstersPerRoom : room.Count();
        int monstersToPlace = GD.RandRange(0, maxMonstersInThisRoom);
        for (int monster = 0; monster < monstersToPlace; monster++) {
            monstersAndLootMap.SetCellItem((Vector3I)room.PickRandom(), 4);
        }

        roomTiles.Add(room);

        float avgX = startPos.X + (float)(width / 2);
        float avgZ = startPos.Z + (float)(height / 2);
        Vector3 roomCenter = new Vector3(avgX, currentFloor, avgZ);
        roomPositions.Add(roomCenter);
    }


    public void VisualizeBorder() {
        GD.Print("visualize border");
        gridMap.Clear();
        for (int i = -1; i < borderSize + 1; i++) {
            gridMap.SetCellItem(new Vector3I(i, 0, -1), TileDict[Tiles.Border]);
            gridMap.SetCellItem(new Vector3I(i, 0, borderSize), TileDict[Tiles.Border]);
            gridMap.SetCellItem(new Vector3I(borderSize, 0, i), TileDict[Tiles.Border]);
            gridMap.SetCellItem(new Vector3I(-1, 0, i), TileDict[Tiles.Border]);
        }

    }


    public void PutWalls(int floor) {
        GD.Print("put walls");
        for (int i = -1; i < borderSize + 1; i++) {
            for (int j = -1; j < borderSize + 1; j++) {

                bool hasNeighbor =
                (gridMap.GetCellItem(new Vector3I(i, floor, j - 1)) >= 0 && gridMap.GetCellItem(new Vector3I(i, floor, j - 1)) != TileDict[Tiles.Wall]) ||
                (gridMap.GetCellItem(new Vector3I(i - 1, floor, j)) >= 0 && gridMap.GetCellItem(new Vector3I(i - 1, floor, j)) != TileDict[Tiles.Wall]) ||
                (gridMap.GetCellItem(new Vector3I(i + 1, floor, j)) >= 0 && gridMap.GetCellItem(new Vector3I(i + 1, floor, j)) != TileDict[Tiles.Wall]) ||
                (gridMap.GetCellItem(new Vector3I(i, floor, j + 1)) >= 0 && gridMap.GetCellItem(new Vector3I(i, floor, j + 1)) != TileDict[Tiles.Wall]);


                if (gridMap.GetCellItem(new Vector3I(i, floor, j)) < 0 && hasNeighbor) {
                    for (int w = 0; w < floorHeight; w++) gridMap.SetCellItem(new Vector3I(i, floor + w, j), TileDict[Tiles.Wall]);
                }
            }
        }
    }


    public void makeRealDungeon() {
        Array<Vector3I> cellsToConvert = gridMap.GetUsedCells();
        Vector3I gridOffset = new Vector3I(6, 12, 6);
        foreach (Vector3I cellPosition in cellsToConvert) {
            DungeonCell newCell = dungeonCell.Instantiate<DungeonCell>();
            AddChild(newCell);
            newCell.GlobalPosition = cellPosition * gridOffset;
            if (gridMap.GetCellItem(cellPosition) == TileDict[Tiles.Entrance]) playerScene.Position = cellPosition * gridOffset;
            if (gridMap.GetCellItem(cellPosition + Vector3I.Left) >= 0) newCell.RemoveWall(DungeonCell.Directions.West);
            if (gridMap.GetCellItem(cellPosition + Vector3I.Right) >= 0) newCell.RemoveWall(DungeonCell.Directions.East);
            if (gridMap.GetCellItem(cellPosition + Vector3I.Forward) >= 0) newCell.RemoveWall(DungeonCell.Directions.North);
            if (gridMap.GetCellItem(cellPosition + Vector3I.Back) >= 0) newCell.RemoveWall(DungeonCell.Directions.South);
            newCell.KeepTorches(chanceToKeepTorches);
        }
    }
}
