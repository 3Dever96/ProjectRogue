using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks nearby interactable objects and triggers the closest one when the player presses the interact input.
/// </summary>
public class InteractionHub : MonoBehaviour
{
    // All interactables currently within range of this player.
    private readonly List<IInteractable> interactables = new List<IInteractable>();

    // The nearest interactable currently being considered for interaction.
    private IInteractable currentInteractable;

    // Prevents repeated activation while the interact button is held down.
    private bool canInteract;

    private void Update()
    {
        // Reevaluate which object is closest each frame.
        currentInteractable = GetClosestInteractable();

        // Clear the "current" flag from all interactables before marking the active one.
        foreach (IInteractable interactable in interactables)
        {
            interactable.SetCurrent(false);
        }

        if (currentInteractable != null && currentInteractable.CanInteract(transform))
        {
            // Mark the nearest object as the active one.
            currentInteractable.SetCurrent(true);

            // Trigger interaction only on a fresh press, not while the key remains held.
            if (InputHub.Instance.Interact && canInteract)
            {
                currentInteractable.OnInteract();
                currentInteractable.SetCurrent(false);
                RemoveInteraction(currentInteractable);
            }
        }

        // Update the press-state so the same button press is not repeated every frame.
        canInteract = !InputHub.Instance.Interact;
    }

    // Registers a new interactable so it can be considered during proximity checks.
    public void AddInteraction(IInteractable interactable)
    {
        if (interactable == null || interactables.Contains(interactable))
        {
            return;
        }

        interactables.Add(interactable);
    }

    // Removes an interactable from the tracking list.
    public void RemoveInteraction(IInteractable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        interactables.Remove(interactable);

        if (currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    // Finds the nearest object in the current list of interactables.
    private IInteractable GetClosestInteractable()
    {
        if (interactables.Count == 0)
        {
            return null;
        }

        IInteractable closest = interactables[0];
        float closestDistance = GetDistanceToInteractable(closest);

        for (int i = 1; i < interactables.Count; i++)
        {
            IInteractable interactable = interactables[i];
            float distance = GetDistanceToInteractable(interactable);

            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        return closest;
    }

    // Computes the distance between this object and the interactable's transform.
    private float GetDistanceToInteractable(IInteractable interactable)
    {
        if (interactable is not MonoBehaviour monoBehaviour)
        {
            return float.MaxValue;
        }

        return Vector3.Distance(transform.position, monoBehaviour.transform.position);
    }
}
