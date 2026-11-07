using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralizes player input values so other systems can read movement and jump state from one place.
/// </summary>
public class InputHub : MonoBehaviour
{
    // Keep a single persistent instance so the input state survives scene changes.
    public static InputHub Instance { get; private set; }

    // Stores the current movement vector from the input action.
    public Vector2 Move { get; private set; }

    // Tracks whether the jump input is currently being pressed.
    public bool Jump { get; private set; }

    // Reference to the PlayerInput component used to listen for input events.
    private PlayerInput playerInput;

    private void Awake()
    {
        // Ensure only one InputHub exists at a time.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Keep this object alive across scene loads.
        DontDestroyOnLoad(gameObject);

        // Cache the input component so we can subscribe to its action events.
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        // Update the relevant property based on which action fired.
        switch (context.action.name)
        {
            case "Move":
                Move = context.ReadValue<Vector2>();
                break;
            case "Jump":
                Jump = context.ReadValueAsButton();
                break;
        }
    }

    private void OnEnable()
    {
        // Subscribe to the action trigger event whenever this object is enabled.
        playerInput.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid duplicate event handling and memory issues.
        playerInput.onActionTriggered -= OnAction;
    }
}
