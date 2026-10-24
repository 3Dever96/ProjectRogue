using System.Collections.Generic;
using UnityEngine;

public class BranchingDungeonGenerator : MonoBehaviour
{
    // Define an enum to keep track of what kind of room each coordinate belongs to
    public enum RoomType
    {
        Basic,
        Start,
        Boss,
        TreasureSecret,
        Shop,
        LockedEncounter,
        HardLockedEncounter
    }

    [Header("Dungeon Settings")]
    public int mainPathLength = 10;
    public int maxBranches = 3;
    public int maxBranchLength = 4;

    [Header("Visuals")]
    public Room roomPrefab; // Back to a single modular prefab
    public float roomSpacing = 10f;
    public MapCell cell;

    // Maps a coordinate to its specific room role
    private Dictionary<Vector2Int, RoomType> roomTypes = new Dictionary<Vector2Int, RoomType>();

    // Grid coordinates mapped to a list of coordinates they are ALLOWED to connect to
    private Dictionary<Vector2Int, List<Vector2Int>> roomConnections = new Dictionary<Vector2Int, List<Vector2Int>>();
    private List<Vector2Int> mainPath = new List<Vector2Int>();
    private Dictionary<Vector2Int, Room> layout = new Dictionary<Vector2Int, Room>();

    [SerializeField, Range(0, 7)] private int currentBiome;

    private readonly Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        roomConnections.Clear();
        roomTypes.Clear();
        mainPath.Clear();
        layout.Clear();

        // 1. Generate Main Spine
        Vector2Int currentPos = Vector2Int.zero;
        AddRoomToGrid(currentPos);
        mainPath.Add(currentPos);
        roomTypes[currentPos] = RoomType.Start; // Assign Start Room explicitly

        for (int i = 0; i < mainPathLength - 1; i++)
        {
            Vector2Int nextPos = GetRandomValidNeighbor(currentPos);
            ConnectRooms(currentPos, nextPos);
            currentPos = nextPos;
            mainPath.Add(currentPos);

            // Default fill the spine path with basic rooms for now
            roomTypes[currentPos] = RoomType.Basic;
        }

        // Set the Boss Room at the very end of the main spine path
        Vector2Int bossCoordinate = mainPath[mainPath.Count - 1];
        roomTypes[bossCoordinate] = RoomType.Boss;

        // 2. Generate Side Branches
        int branchesCreated = 0;
        int attempts = 0;
        List<Vector2Int> branchEndPoints = new List<Vector2Int>();

        while (branchesCreated < maxBranches && attempts < 100)
        {
            attempts++;
            Vector2Int branchStart = mainPath[Random.Range(1, mainPath.Count - 2)]; // Don't branch right off the start or boss

            if (TryGenerateBranch(branchStart, out Vector2Int deadEnd))
            {
                branchesCreated++;
                branchEndPoints.Add(deadEnd);
            }
        }

        // 3. Assign types to remaining empty rooms (Encounter distributions)
        AssignSpecialAndEncounterRooms(branchEndPoints);

        // 4. Spawn the layout
        SpawnDungeon();
        SetRoomNeighbors();
    }

    void AddRoomToGrid(Vector2Int coord)
    {
        if (!roomConnections.ContainsKey(coord))
        {
            roomConnections[coord] = new List<Vector2Int>();
        }
    }

    void ConnectRooms(Vector2Int roomA, Vector2Int roomB)
    {
        AddRoomToGrid(roomA);
        AddRoomToGrid(roomB);

        if (!roomConnections[roomA].Contains(roomB)) roomConnections[roomA].Add(roomB);
        if (!roomConnections[roomB].Contains(roomA)) roomConnections[roomB].Add(roomA);
    }

    Vector2Int GetRandomValidNeighbor(Vector2Int center)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int neighbor = center + dir;
            if (!roomConnections.ContainsKey(neighbor))
            {
                validNeighbors.Add(neighbor);
            }
        }

        if (validNeighbors.Count == 0)
        {
            return center + directions[Random.Range(0, directions.Length)];
        }
        return validNeighbors[Random.Range(0, validNeighbors.Count)];
    }

    bool TryGenerateBranch(Vector2Int startPoint, out Vector2Int deadEnd)
    {
        Vector2Int currentPos = startPoint;
        int branchLength = Random.Range(1, maxBranchLength + 1);
        bool builtAnything = false;
        deadEnd = startPoint;

        for (int i = 0; i < branchLength; i++)
        {
            List<Vector2Int> validMoves = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                Vector2Int nextTile = currentPos + dir;
                if (!roomConnections.ContainsKey(nextTile))
                {
                    validMoves.Add(nextTile);
                }
            }

            if (validMoves.Count == 0) break;

            Vector2Int nextPos = validMoves[Random.Range(0, validMoves.Count)];
            ConnectRooms(currentPos, nextPos);

            currentPos = nextPos;
            deadEnd = currentPos;
            roomTypes[currentPos] = RoomType.Basic; // Fill branches as basic rooms initially
            builtAnything = true;
        }

        return builtAnything;
    }

    void AssignSpecialAndEncounterRooms(List<Vector2Int> branchEnds)
    {
        // 1. Place Shop and Treasure rooms at dead ends of side branches first
        if (branchEnds.Count > 0)
        {
            int treasureIndex = Random.Range(0, branchEnds.Count);
            roomTypes[branchEnds[treasureIndex]] = RoomType.TreasureSecret;
            branchEnds.RemoveAt(treasureIndex);
        }
        if (branchEnds.Count > 0)
        {
            int shopIndex = Random.Range(0, branchEnds.Count);
            roomTypes[branchEnds[shopIndex]] = RoomType.Shop;
            branchEnds.RemoveAt(shopIndex);
        }

        // 2. Identify the immediate neighbors of the Start Room to create a Safety Buffer
        List<Vector2Int> startNeighbors = roomConnections[Vector2Int.zero];

        // Track a counter to make sure we don't spawn locked rooms back-to-back
        int roomsSinceLastEncounter = 2;

        // Iterate through the main path coordinates sequentially to control the pacing flow
        for (int i = 0; i < mainPath.Count; i++)
        {
            Vector2Int coord = mainPath[i];

            // Skip modifying the Start and Boss rooms entirely
            if (roomTypes[coord] == RoomType.Start || roomTypes[coord] == RoomType.Boss)
                continue;

            // RULE: Safety Buffer. If this room touches the start room, force it to stay Basic
            if (startNeighbors.Contains(coord))
            {
                roomTypes[coord] = RoomType.Basic;
                continue;
            }

            roomsSinceLastEncounter++;

            // Only allow an encounter to roll if the player has had at least 1 basic room to breathe
            if (roomsSinceLastEncounter >= 2)
            {
                float roll = Random.value;

                // Pacing rule: Calculate depth. Rooms deeper in the path have a higher chance of being Hard
                float progressThroughDungeon = (float)i / mainPath.Count;

                if (roll < 0.35f) // Total 35% chance to trigger an encounter selection
                {
                    // If we roll an encounter, determine if it's Hard or Normal based on path depth
                    if (Random.value < progressThroughDungeon && progressThroughDungeon > 0.5f)
                    {
                        roomTypes[coord] = RoomType.HardLockedEncounter;
                    }
                    else
                    {
                        roomTypes[coord] = RoomType.LockedEncounter;
                    }

                    // Reset our cooldown tracker so the next room cannot be locked
                    roomsSinceLastEncounter = 0;
                }
            }
        }

        // 3. Fallback: Scatter light encounters into any side branch tiles that are still standard basic rooms
        foreach (var coord in roomConnections.Keys)
        {
            // Don't modify rooms we already assigned along the main spine path or special dead ends
            if (mainPath.Contains(coord) || roomTypes[coord] != RoomType.Basic)
                continue;

            float branchRoll = Random.value;
            if (branchRoll < 0.20f) // 20% chance for locked rooms in side passages
            {
                roomTypes[coord] = RoomType.LockedEncounter;
            }
        }
    }

    void SpawnDungeon()
    {
        GameObject mapContainer = GameObject.Find("Map Container");

        foreach (KeyValuePair<Vector2Int, List<Vector2Int>> roomData in roomConnections)
        {
            Vector2Int cellCoord = roomData.Key;
            List<Vector2Int> connections = roomData.Value;

            Vector3 worldPos = new Vector3(cellCoord.x * roomSpacing, 0, cellCoord.y * roomSpacing);

            Room roomInstance = Instantiate(roomPrefab, worldPos, Quaternion.identity, transform);
            roomInstance.name = $"{roomTypes[cellCoord]}_{cellCoord.x}_{cellCoord.y}";

            bool openNorth = connections.Contains(cellCoord + Vector2Int.up);
            bool openSouth = connections.Contains(cellCoord + Vector2Int.down);
            bool openEast = connections.Contains(cellCoord + Vector2Int.right);
            bool openWest = connections.Contains(cellCoord + Vector2Int.left);

            roomInstance.ConfigurePassages(openNorth, openEast, openSouth, openWest);
            roomInstance.SetDungeonBiome(currentBiome);

            // Spawn minimap data if a map cell reference and a map layout container are configured
            if (cell != null && mapContainer != null)
            {
                MapCell newCell = Instantiate(cell, new Vector3(cellCoord.x * 1.5f, cellCoord.y * 1.5f, 0f), Quaternion.identity, mapContainer.transform);
                roomInstance.SetMapCell(newCell);
            }

            layout.Add(cellCoord, roomInstance);
        }
    }

    private void SetRoomNeighbors()
    {
        foreach (KeyValuePair<Vector2Int, Room> room in layout)
        {
            Vector2Int pos = room.Key;
            Room currentRoom = room.Value;

            // Pass the assigned structural enum index into your existing SetNeighborRooms loop
            int numericRoomType = (int)roomTypes[pos];

            Vector2Int northPos = pos + Vector2Int.up;
            Vector2Int eastPos = pos + Vector2Int.right;
            Vector2Int southPos = pos - Vector2Int.up;
            Vector2Int westPos = pos - Vector2Int.right;

            Room northRoom = layout.ContainsKey(northPos) ? layout[northPos] : null;
            Room eastRoom = layout.ContainsKey(eastPos) ? layout[eastPos] : null;
            Room southRoom = layout.ContainsKey(southPos) ? layout[southPos] : null;
            Room westRoom = layout.ContainsKey(westPos) ? layout[westPos] : null;

            currentRoom.SetNeighborRooms(northRoom, eastRoom, southRoom, westRoom, numericRoomType);

            // Put chunk to sleep to prepare state loop
            currentRoom.gameObject.SetActive(false);
        }

        // CORRECTED BLOCK
        if (layout.ContainsKey(Vector2Int.zero))
        {
            layout[Vector2Int.zero].gameObject.SetActive(true);

            // Set the initial room data cleanly using your dedicated manager sequence
            DungeonManager.Instance.SetNextRoom(layout[Vector2Int.zero]);
            DungeonManager.Instance.UpdateRooms(layout[Vector2Int.zero]);
        }

    }
}
