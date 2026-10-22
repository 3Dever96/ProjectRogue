using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton manager that routes Unity Input System events into accessible properties.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputHub : MonoBehaviour
{
    // --- Input Properties ---
    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }
    public bool Dash { get; private set; }
    public bool Sprint { get; private set; }
    public bool Attack { get; private set; }
    public bool Magic { get; private set; }
    public bool Interact { get; private set; }
    public bool Pause { get; private set; }

    // --- Singleton Instance ---
    public static InputHub Instance;

    private PlayerInput input;

    private void Awake()
    {
        // Enforce Singleton Pattern
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        // Keep input active across scene loads
        DontDestroyOnLoad(gameObject);

        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Subscribe to input events
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        input.onActionTriggered -= OnAction;
    }

    /// <summary>
    /// Processes triggered input actions and updates corresponding properties.
    /// </summary>
    public void OnAction(InputAction.CallbackContext context)
    {
        // Map action names to specific properties using a threshold for button states
        switch (context.action.name)
        {
            case "Move": Move = context.ReadValue<Vector2>(); break;
            case "Jump": Jump = context.ReadValue<float>() > 0.5f; break;
            case "Dash": Dash = context.ReadValue<float>() > 0.5f; break;
            case "Sprint": Sprint = context.ReadValue<float>() > 0.5f; break;
            case "Attack": Attack = context.ReadValue<float>() > 0.5f; break;
            case "Magic": Magic = context.ReadValue<float>() > 0.5f; break;
            case "Interact": Interact = context.ReadValue<float>() > 0.5f; break;
            case "Pause": Pause = context.ReadValue<float>() > 0.5f; break;
        }
    }
}
