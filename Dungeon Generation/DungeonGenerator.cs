using Godot;
using Godot.Collections;
using System;
using System.Linq;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class DungeonGenerator : Node3D {

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
    [Export] private int maxMonstersPerRoom = 4; // chance to not remove a room from the minimum spanning tree
    [Export] private int chanceToKeepTorches = 25;
    [Export] private int maxTreasuresPerRoom = 2;

    private GridMap gridMap = new();
    private GridMap enemiesAndLootMap = new();

    private PackedScene playerScene = GD.Load<PackedScene>("uid://cnp46bxhs5wct");
    private PackedScene dungeonCell = GD.Load<PackedScene>("uid://c6cpwbmgqllwl");
    private PackedScene enemyScene = GD.Load<PackedScene>("uid://dnub6h4v0xxq0");
    private PackedScene treasureScene = GD.Load<PackedScene>("uid://qx270dvsirlu");

    private PlayerController playerInstance;

    private Array<Array<Vector3>> roomTiles = [];
    private Array<Vector3> roomPositions;
    private bool isEntrancePlaced = false;
    private bool isExitPlaced = false;
    private int roomsMade;
    private Vector3I entrancePosition;
    private int currentFloor;

    public Action<PlayerController> PlayerPlaced;


    public void GenerateDungeon() {
        GD.Print("Generate dungeon");
        gridMap.Clear();
        enemiesAndLootMap.Clear();
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
        roomTiles.Clear();
        roomPositions = [];
        roomsMade = 0;
        currentFloor = floor * floorHeight;

        isEntrancePlaced = false;
        isExitPlaced = false;

        GD.Print("making room");
        for (int i = 0; i < roomsNumber; i++) {
            MakeRoom(roomRecursionAttempts);
            roomsMade++;
        }

        GD.Print("place entrance");
        while (!isEntrancePlaced) {
            Vector3I randomStartingPosition = (Vector3I)roomTiles.PickRandom().PickRandom();
            if (gridMap.GetCellItem(randomStartingPosition) == Rules.Instance.TileDict[Rules.Tiles.Floor]) {
                enemiesAndLootMap.SetCellItem(randomStartingPosition, Rules.Instance.TileDict[Rules.Tiles.Entrance]);
                isEntrancePlaced = true;
                entrancePosition = randomStartingPosition;
            }

        }

        GD.Print("place exit");
        isExitPlaced = false;
        while (!isExitPlaced) {
            Vector3I randomStartingPosition = (Vector3I)roomTiles.PickRandom().PickRandom();
            if (gridMap.GetCellItem(randomStartingPosition) == Rules.Instance.TileDict[Rules.Tiles.Floor] &&
            entrancePosition.DistanceSquaredTo(randomStartingPosition) / 10 > (int)borderSize / 2) {
                enemiesAndLootMap.SetCellItem(randomStartingPosition, Rules.Instance.TileDict[Rules.Tiles.Exit]);
                isExitPlaced = true;
            }

        }

        GD.Print("graph connection");
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
        GD.Print("making hallways");
        Array<int> visitedPoints = [];
        visitedPoints.Add(GD.RandRange(0, roomPositions.Count() - 1));
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
                    gridMap.SetCellItem((Vector3I)tileFrom, Rules.Instance.TileDict[Rules.Tiles.Door]);
                    gridMap.SetCellItem((Vector3I)tileTo, Rules.Instance.TileDict[Rules.Tiles.Door]);
                }
            }
        }

        AStarGrid2D astar = new();
        astar.Region = new Rect2I(Vector2I.Zero, Vector2I.One * borderSize);
        astar.Update();
        astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        astar.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;

        // block the pathfinding from walking into rooms
        foreach (Vector3I tile in gridMap.GetUsedCellsByItem(Rules.Instance.TileDict[Rules.Tiles.Floor])) {
            if (tile.Y == currentFloor) astar.SetPointSolid(new Vector2I(tile.X, tile.Z));
        }

        foreach (Vector3[] hallway in hallways) {
            Vector2I posFrom = new Vector2I((int)hallway[0].X, (int)hallway[0].Z);
            Vector2I posTo = new Vector2I((int)hallway[1].X, (int)hallway[1].Z);
            Vector2[] hall = astar.GetPointPath(posFrom, posTo);

            foreach (Vector2 i in hall) {
                Vector3I position = new Vector3I((int)i.X, currentFloor, (int)i.Y);
                if (gridMap.GetCellItem(position) < 0) {
                    gridMap.SetCellItem(position, Rules.Instance.TileDict[Rules.Tiles.Hallway]);
                    astar.SetPointWeightScale(new Vector2I((int)i.X, (int)i.Y), 0.5f);
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
                if (gridMap.GetCellItem(pos) == Rules.Instance.TileDict[Rules.Tiles.Floor]) {
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
                gridMap.SetCellItem(pos, Rules.Instance.TileDict[Rules.Tiles.Floor]);
                room.Add(pos);
            }
        }

        // add enemies
        int maxMonstersInThisRoom = maxMonstersPerRoom <= room.Count() ? maxMonstersPerRoom : room.Count();
        int monstersToPlace = GD.RandRange(0, maxMonstersInThisRoom);
        for (int monster = 0; monster < monstersToPlace; monster++) {
            bool placed = false;
            while (placed == false) {
                Vector3I candidateTile = (Vector3I)room.PickRandom();
                if (enemiesAndLootMap.GetCellItem(candidateTile) < 0) {
                    enemiesAndLootMap.SetCellItem(candidateTile, Rules.Instance.TileDict[Rules.Tiles.Enemy]);
                    placed = true;
                }
            }

        }

        // add treasure chests
        int maxtreasuresInThisRoom = maxTreasuresPerRoom <= room.Count() ? maxTreasuresPerRoom : room.Count();
        int treasuresToPlace = GD.RandRange(0, maxMonstersInThisRoom);
        for (int treasure = 0; treasure < treasuresToPlace; treasure++) {
            bool placed = false;
            while (placed == false) {
                Vector3I candidateTile = (Vector3I)room.PickRandom();
                if (enemiesAndLootMap.GetCellItem(candidateTile) < 0) {
                    enemiesAndLootMap.SetCellItem((Vector3I)room.PickRandom(), Rules.Instance.TileDict[Rules.Tiles.Loot]);
                    placed = true;
                }
            }
        }

        roomTiles.Add(room);

        float avgX = startPos.X + (float)(width / 2);
        float avgZ = startPos.Z + (float)(height / 2);
        Vector3 roomCenter = new Vector3(avgX, currentFloor, avgZ);
        roomPositions.Add(roomCenter);
    }


    public void makeRealDungeon() {
        GD.Print("make real dungeon");
        Array<Vector3I> cellsToConvert = gridMap.GetUsedCells();
        Vector3I gridOffset = new Vector3I(6, 12, 6);

        // create the floor and walls, position the palyer
        GD.Print("placing " + cellsToConvert.Count + " cells");
        foreach (Vector3I cellPosition in cellsToConvert) {
            NewDunCell newCell = dungeonCell.Instantiate<NewDunCell>();
            AddChild(newCell);
            newCell.GlobalPosition = cellPosition * gridOffset;
            // position the player in the dungeon

            int neighborN = gridMap.GetCellItem(cellPosition + Vector3I.Forward);
            int neighborS = gridMap.GetCellItem(cellPosition + Vector3I.Back);
            int neighborW = gridMap.GetCellItem(cellPosition + Vector3I.Left);
            int neighborE = gridMap.GetCellItem(cellPosition + Vector3I.Right);

            Rules.CardinalDirections hallwayDir = Rules.CardinalDirections.N;
            if (neighborN == Rules.Instance.TileDict[Rules.Tiles.Hallway]) hallwayDir = Rules.CardinalDirections.N;
            if (neighborS == Rules.Instance.TileDict[Rules.Tiles.Hallway]) hallwayDir = Rules.CardinalDirections.S;
            if (neighborW == Rules.Instance.TileDict[Rules.Tiles.Hallway]) hallwayDir = Rules.CardinalDirections.W;
            if (neighborE == Rules.Instance.TileDict[Rules.Tiles.Hallway]) hallwayDir = Rules.CardinalDirections.E;

            if (gridMap.GetCellItem(cellPosition) < Rules.Instance.TileDict[Rules.Tiles.Hallway])
                newCell.OpenWalls(
                    neighborN >= 0 && neighborN < Rules.Instance.TileDict[Rules.Tiles.Hallway],
                    neighborS >= 0 && neighborS < Rules.Instance.TileDict[Rules.Tiles.Hallway],
                    neighborW >= 0 && neighborW < Rules.Instance.TileDict[Rules.Tiles.Hallway],
                    neighborE >= 0 && neighborE < Rules.Instance.TileDict[Rules.Tiles.Hallway],
                    gridMap.GetCellItem(cellPosition) == Rules.Instance.TileDict[Rules.Tiles.Door],
                    hallwayDir
                );

            else if (gridMap.GetCellItem(cellPosition) == Rules.Instance.TileDict[Rules.Tiles.Hallway])
                newCell.OpenWalls(
                    neighborN > Rules.Instance.TileDict[Rules.Tiles.Floor],
                    neighborS > Rules.Instance.TileDict[Rules.Tiles.Floor],
                    neighborW > Rules.Instance.TileDict[Rules.Tiles.Floor],
                    neighborE > Rules.Instance.TileDict[Rules.Tiles.Floor],
                    gridMap.GetCellItem(cellPosition) == Rules.Instance.TileDict[Rules.Tiles.Door],
                    hallwayDir
                );
        }

        Array<Vector3I> entranceTile = enemiesAndLootMap.GetUsedCellsByItem(Rules.Instance.TileDict[Rules.Tiles.Entrance]);
        GD.Print("found entrance for player");
        playerInstance = playerScene.Instantiate<PlayerController>();
        AddChild(playerInstance);
        playerInstance.GlobalPosition = entranceTile[0] * gridOffset + Vector3.Up;
        PlayerPlaced?.Invoke(playerInstance);
        GD.Print("placing player");

        // instantiate enemies and loot
        cellsToConvert = enemiesAndLootMap.GetUsedCells();
        GD.Print("placing " + cellsToConvert.Count + " monsters and loot");
        foreach (Vector3I cellPosition in cellsToConvert) {
            if (enemiesAndLootMap.GetCellItem(cellPosition) == Rules.Instance.TileDict[Rules.Tiles.Enemy]) {
                EnemyController newEnemy = enemyScene.Instantiate<EnemyController>();
                AddChild(newEnemy);
                newEnemy.GlobalPosition = cellPosition * gridOffset + Vector3.Up;
            }
            if (enemiesAndLootMap.GetCellItem(cellPosition) == Rules.Instance.TileDict[Rules.Tiles.Loot]) {
                StaticBody3D newChest = treasureScene.Instantiate<StaticBody3D>();
                AddChild(newChest);
                newChest.GlobalPosition = cellPosition * gridOffset;
            }
        }
    }
}
