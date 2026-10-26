using UnityEngine;

/// <summary>
/// Controls player state delegation and executes character movement.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController Controller { get; private set; }
    public Animator Animator { get; private set; }

    // --- State Machine ---
    public PlayerState CurrentState { get; private set; }
    [field: SerializeField] public PlayerGroundState GroundState { get; private set; } = new PlayerGroundState();
    [field: SerializeField] public PlayerAirState AirState { get; private set; } = new PlayerAirState();
    [field: SerializeField] public PlayerDashState DashState { get; private set; } = new PlayerDashState();

    // --- Movement Vectors & Mechanics ---
    public Vector3 MoveDirection { get; set; }
    public Vector3 LookDirection { get; set; }
    public float CurrentSpeed { get; set; }
    public float VerticalSpeed { get; set; }

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();

        // Establish the baseline state on spawn
        SetState(GroundState);
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            // Process behaviors, check state changes, and apply calculations to the controller
            CurrentState.UpdateState(this);
            CurrentState.ChangeState(this);

            ApplyMovement();
        }
    }

    /// <summary>
    /// Safely exits the previous state and enters a new state.
    /// </summary>
    public void SetState(PlayerState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            CurrentState.StartState(this);
        }
    }

    /// <summary>
    /// Smoothly rotates the transform towards the current look direction.
    /// </summary>
    public void FaceDirection(float turnSpeed = 500f)
    {
        if (LookDirection == Vector3.zero) return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Checks for a physical collision sphere against the 'Solid' layer.
    /// </summary>
    public bool CheckCollitions(Vector3 originOffset, float radius)
    {
        return Physics.CheckSphere(transform.position + originOffset, radius, LayerMask.GetMask("Solid"));
    }

    /// <summary>
    /// Translates directional speeds into world coordinates and moves the CharacterController.
    /// </summary>
    public void ApplyMovement()
    {
        Vector3 velocity = CurrentSpeed * MoveDirection;
        velocity.y = VerticalSpeed;

        Controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Calculates movement relative to the Main Camera perspective and flattens the Y-axis.
    /// </summary>
    public Vector3 GetInputDirection()
    {
        Vector3 direction = Camera.main.transform.right * InputHub.Instance.Move.x + Camera.main.transform.forward * InputHub.Instance.Move.y;
        direction.y = 0f; // Prevent movement input from pulling the player into the air/ground

        return direction;
    }
}
