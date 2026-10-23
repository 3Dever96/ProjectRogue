using Animancer;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private AnimancerComponent anim;
    [SerializeField] private AnimationClip closedAnim;
    [SerializeField] private AnimationClip openAnim;

    private bool isOpen;

    public void OnInteract()
    {
        if (!isOpen)
        {
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
        }
    }
}
