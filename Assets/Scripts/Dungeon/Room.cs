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
}
