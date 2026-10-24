using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Wall Objects")]
    public GameObject wallNorth;
    public GameObject wallEast;
    public GameObject wallSouth;
    public GameObject wallWest;

    [Header("Doorway Objects")]
    public GameObject doorwayNorth;
    public GameObject doorwayEast;
    public GameObject doorwaySouth;
    public GameObject doorwayWest;

    private Renderer[] renderers;

    public bool isCurrentRoom;

    private MapCell cell;

    private int roomType;

    public void ConfigurePassages(bool openNorth, bool openEast, bool openSouth, bool openWest)
    {
        wallNorth.SetActive(!openNorth);
        doorwayNorth.SetActive(openNorth);

        wallEast.SetActive(!openEast);
        doorwayEast.SetActive(openEast);

        wallSouth.SetActive(!openSouth);
        doorwaySouth.SetActive(openSouth);

        wallWest.SetActive(!openWest);
        doorwayWest.SetActive(openWest);
    }

    public void SetNeighborRooms(Room north, Room east, Room south, Room west, int newType)
    {
        bool n = false, e = false, s = false, w = false;

        roomType = newType;

        if (doorwayNorth.activeInHierarchy)
        {
            Door d = doorwayNorth.GetComponentInChildren<Door>();
            d.SetRooms(this, north);

            n = true;
        }

        if (doorwayEast.activeInHierarchy)
        {
            Door d = doorwayEast.GetComponentInChildren<Door>();
            d.SetRooms(this, east);

            e = true;
        }

        if (doorwaySouth.activeInHierarchy)
        {
            Door d = doorwaySouth.GetComponentInChildren<Door>();
            d.SetRooms(this, south);

            s = true;
        }

        if (doorwayWest.activeInHierarchy)
        {
            Door d = doorwayWest.GetComponentInChildren<Door>();
            d.SetRooms(this, west);

            w = true;
        }

        cell.InitializeCell(n,e, s, w, roomType);

        gameObject.SetActive(false);
    }

    public void SetDungeonBiome(int index)
    {
        renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.material.mainTextureOffset = new Vector2(0.125f * index, 0f);
        }
    }

    public void SetMapCell(MapCell newCell)
    {
        cell = newCell;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            // Pass 'this' room reference into the updated manager method
            DungeonManager.Instance.UpdateRooms(this);

            cell.SetCurrentRoom();
        }
    }
}
