using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BranchingDungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int mainPathLength = 10;
    public int maxBranches = 3;
    public int maxBranchLength = 4;

    [Header("Visuals")]
    public Room roomPrefab;
    public float roomSpacing = 10f;

    // Grid coordinates mapped to a list of coordinates they are ALLOWED to connect to
    private Dictionary<Vector2Int, List<Vector2Int>> roomConnections = new Dictionary<Vector2Int, List<Vector2Int>>();
    private List<Vector2Int> mainPath = new List<Vector2Int>();

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
        mainPath.Clear();

        // 1. Generate Main Spine
        Vector2Int currentPos = Vector2Int.zero;
        AddRoomToGrid(currentPos);
        mainPath.Add(currentPos);

        for (int i = 0; i < mainPathLength - 1; i++)
        {
            Vector2Int nextPos = GetRandomValidNeighbor(currentPos);

            // Connect previous room to next room, and next room back to previous room
            ConnectRooms(currentPos, nextPos);

            currentPos = nextPos;
            mainPath.Add(currentPos);
        }

        // 2. Generate Side Branches
        int branchesCreated = 0;
        int attempts = 0;

        while (branchesCreated < maxBranches && attempts < 100)
        {
            attempts++;
            Vector2Int branchStart = mainPath[Random.Range(1, mainPath.Count - 1)];

            if (TryGenerateBranch(branchStart))
            {
                branchesCreated++;
            }
        }

        // 3. Spawn the layout
        SpawnDungeon();
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

    bool TryGenerateBranch(Vector2Int startPoint)
    {
        Vector2Int currentPos = startPoint;
        int branchLength = Random.Range(1, maxBranchLength + 1);
        bool builtAnything = false;

        for (int i = 0; i < branchLength; i++)
        {
            List<Vector2Int> validMoves = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                Vector2Int nextTile = currentPos + dir;
                // Strict rule: It CANNOT merge into ANY existing room
                if (!roomConnections.ContainsKey(nextTile))
                {
                    validMoves.Add(nextTile);
                }
            }

            if (validMoves.Count == 0) break;

            Vector2Int nextPos = validMoves[Random.Range(0, validMoves.Count)];

            // Link them together!
            ConnectRooms(currentPos, nextPos);

            currentPos = nextPos;
            builtAnything = true;
        }

        return builtAnything;
    }

    void SpawnDungeon()
    {
        foreach (KeyValuePair<Vector2Int, List<Vector2Int>> roomData in roomConnections)
        {
            Vector2Int cell = roomData.Key;
            List<Vector2Int> connections = roomData.Value;

            Vector3 worldPos = new Vector3(cell.x * roomSpacing, 0, cell.y * roomSpacing);

            Room roomInstance = Instantiate(roomPrefab, worldPos, Quaternion.identity, transform);
            roomInstance.name = $"Room_{cell.x}_{cell.y}";

            // Check doors ONLY if they are explicitly registered in our connections list
            bool openNorth = connections.Contains(cell + Vector2Int.up);
            bool openSouth = connections.Contains(cell + Vector2Int.down);
            bool openEast = connections.Contains(cell + Vector2Int.right);
            bool openWest = connections.Contains(cell + Vector2Int.left);

            roomInstance.ConfigurePassages(openNorth, openEast, openSouth, openWest);
            roomInstance.SetDungeonBiome(currentBiome);
        }
    }
}
