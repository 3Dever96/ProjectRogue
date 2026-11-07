using UnityEngine;

/**
 * Contract for objects that can be interacted with by the player.
 * The interaction hub uses this interface to trigger actions and highlight the current target.
 */
public interface IInteractable
{
    // Called when the player activates the interaction input while this object is the current target.
    public void OnInteract();

    // Marks whether this object is the closest or active interactable and can be visually highlighted.
    public void SetCurrent(bool isCurrent);

    public bool CanInteract(Transform hub); 
}
