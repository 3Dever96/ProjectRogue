using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHub : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }
    public bool Dash {  get; private set; }
    public bool Sprint {  get; private set; }
    public bool Attack {  get; private set; }
    public bool Magic {  get; private set; }
    public bool Interact {  get; private set; }
    public bool Pause {  get; private set; }

    public static InputHub Instance;

    private PlayerInput input;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        input.onActionTriggered -= OnAction;
    }

    public void OnAction(InputAction.CallbackContext context)
    {
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
