using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform interactPoint;
    [SerializeField] private float interactRadius;
    [SerializeField] private bool canInteract;

    private bool isCurrent;

    public bool CanInteract(Transform hub)
    {
        return Vector3.Distance(interactPoint.position, hub.position) <= interactRadius;
    }

    private void Start()
    {
        if (interactPoint == null)
        {
            interactPoint = transform;
        }
    }

    private void Update()
    {
        if (isCurrent)
        {
            transform.Rotate(0f, 45f * Time.deltaTime, 0f);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    public void OnInteract()
    {
        canInteract = false;
        Debug.Log("Interacted with: " + gameObject.name);
    }

    public void SetCurrent(bool isCurrent)
    {
        this.isCurrent = isCurrent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canInteract)
        {
            InteractionHub hub = other.GetComponent<InteractionHub>();

            if (hub != null)
            {
                hub.AddInteraction(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionHub hub = other.GetComponent<InteractionHub>();

        if (hub != null)
        {
            hub.RemoveInteraction(this);
        }
    }
}
