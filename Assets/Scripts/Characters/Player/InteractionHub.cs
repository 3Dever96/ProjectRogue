using UnityEngine;

public class InteractionHub : MonoBehaviour
{
    private IInteractable currentInteraction;

    private bool canInteract;

    public bool SetInteraction(IInteractable newInteraction)
    {
        if (currentInteraction == null)
        {
            currentInteraction = newInteraction;
            return true;
        }

        return false;
    }

    public void ClearInteraction()
    {
        currentInteraction = null;
    }

    private void Update()
    {
        if (currentInteraction != null)
        {
            if (InputHub.Instance.Interact && canInteract)
            {
                currentInteraction.OnInteract();
            }

            canInteract = !InputHub.Instance.Interact;
        }
    }
}
