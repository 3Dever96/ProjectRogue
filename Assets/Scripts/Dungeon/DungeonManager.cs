using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [field: SerializeField] public Room CurrentRoom { get; private set; }
    [field: SerializeField] public Room PreviousRoom { get; private set; }
    [field: SerializeField] public Room NextRoom { get; private set; }

    public static DungeonManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject); // Fixed minor safety bug here
    }

    public void UpdateRooms(Room enteredRoom)
    {
        // If we step into the room we intended to go to
        if (enteredRoom == NextRoom)
        {
            PreviousRoom = CurrentRoom;
            CurrentRoom = NextRoom;
            NextRoom = null;
        }
        // If the player backtracks into the previous room instead
        else if (enteredRoom == PreviousRoom)
        {
            NextRoom = null;
            CurrentRoom = PreviousRoom;
            PreviousRoom = null; // Clear or adjust based on your dungeon depth rules
        }
    }

    public void SetNextRoom(Room nextRoom)
    {
        NextRoom = nextRoom;
    }

    // Safely host the despawn routine here so turning off a room doesn't kill the code
    public void RequestDespawn(Room roomToDespawn)
    {
        StartCoroutine(DespawnRoutine(roomToDespawn));
    }

    private IEnumerator DespawnRoutine(Room roomToDespawn)
    {
        yield return new WaitForSecondsRealtime(1f);

        // Double check the player didn't turn back around into this room during the 1-second delay
        if (CurrentRoom != roomToDespawn && NextRoom != roomToDespawn)
        {
            roomToDespawn.gameObject.SetActive(false);
        }
    }
}
