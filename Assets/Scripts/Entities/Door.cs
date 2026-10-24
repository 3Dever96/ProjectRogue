using Animancer;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private AnimancerComponent anim;
    [SerializeField] private AnimationClip closedAnim;
    [SerializeField] private AnimationClip openAnim;

    private bool isOpen;

    private Room myRoom;
    private Room neighbor;

    public void SetRooms(Room currentRoom, Room neighborRoom)
    {
        myRoom = currentRoom;
        neighbor = neighborRoom;
    }

    public void OnInteract()
    {
        if (!isOpen)
        {
            // Only allow interaction if this door belongs to the room the player is actually in
            if (DungeonManager.Instance.CurrentRoom == myRoom)
            {
                neighbor.gameObject.SetActive(true);
                DungeonManager.Instance.SetNextRoom(neighbor);
            }

            isOpen = true;
            anim.Play(openAnim, 0.5f);
        }
    }

    private void OnEnable()
    {
        if (anim == null)
        {
            anim = GetComponent<AnimancerComponent>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        InteractionHub player = other.GetComponent<InteractionHub>();
        if (player != null)
        {
            player.SetInteraction(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionHub player = other.GetComponent<InteractionHub>();

        if (player != null)
        {
            player.ClearInteraction();

            isOpen = false;
            anim.Play(closedAnim, 0.5f);

            // Ask the stable DungeonManager to evaluate if the neighbor should be closed down
            if (neighbor != null)
            {
                DungeonManager.Instance.RequestDespawn(neighbor);
            }
        }
    }
}
