using UnityEngine;

/// <summary>
/// Handles player movement, state transitions, and basic locomotion rules.
/// This component manages the CharacterController, active state, and runtime movement logic.
/// </summary>
[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(EntityStats))]
public class PlayerController : MonoBehaviour
{
    // Cached CharacterController used to move the player.
    public CharacterController Controller { get; private set; }
    public Animator Anim { get; private set; }

    // Reference to the player's stat container.
    public EntityStats Stats { get; private set; }

    [Header("Collision Detection")]
    // Layer mask used to detect ground contact.
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }

    // Offset for the ground-check origin.
    [field: SerializeField] public Vector3 GroundOffset { get; private set; }

    // Offset for the head-check origin.
    [field: SerializeField] public Vector3 HeadOffset { get; private set; }

    // Radius used for collision checks.
    [field: SerializeField] public float CheckRadius { get; private set; }

    [Header("Player States")]
    // Ground-state logic used when the player is on the ground.
    [field: SerializeField] public PlayerGroundState GroundState { get; private set; } = new PlayerGroundState();

    // Air-state logic used when the player is in the air.
    [field: SerializeField] public PlayerAirState AirState { get; private set; } = new PlayerAirState();

    // Attack-state logic used while an attack is active.
    [field: SerializeField] public PlayerAttackState AttackState { get; private set; } = new PlayerAttackState();
    [field: SerializeField] public PlayerHitState HitState { get; private set; } = new PlayerHitState();

    // The current state and the state that was active before the most recent transition.
    public PlayerState CurrentState { get; private set; }
    public PlayerState LastState { get; private set; }

    // Current horizontal movement speed.
    public float CurrentSpeed { get; set; }

    // Vertical velocity used for jump and fall movement.
    public float VerticalSpeed { get; set; }

    // Desired movement direction in world space.
    public Vector3 MoveDirection { get; set; }

    // Direction the player is facing or aiming toward.
    public Vector3 LookDirection { get; set; }

    private void Start()
    {
        // Cache required components for use throughout the script.
        Controller = GetComponent<CharacterController>();
        Anim = GetComponentInChildren<Animator>();
        Stats = GetComponent<EntityStats>();

        // Begin in the grounded state.
        SetState(GroundState);
    }

    private void Update()
    {
        // Let the active state update before checking whether it should transition.
        CurrentState?.UpdateState(this);
        CurrentState?.ChangeState(this);

        // Apply the final movement result.
        ApplyMovement();
    }

    // Switches to a new player state and invokes its lifecycle hooks.
    public void SetState(PlayerState state)
    {
        // Preserve the previous state so transitions can return to it later.
        LastState = CurrentState != null && CurrentState != state ? CurrentState : LastState;

        // Finish the old state before starting the new one so each state can clean up.
        CurrentState?.EndState(this);
        CurrentState = state;
        CurrentState?.StartState(this);
    }

    // Rotates the player toward the current look direction.
    public void FaceDirection(float turnSpeed = 500f)
    {
        if (LookDirection == Vector3.zero)
        {
            return;
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    // Checks whether any colliders are within a sphere at the given offset and layer mask.
    public bool CheckCollision(Vector3 origin, float radius, LayerMask layerMask)
    {
        return Physics.CheckSphere(transform.position + origin, radius, layerMask);
    }

    // Builds a movement direction based on input relative to the camera orientation.
    public Vector3 GetDirection()
    {
        Vector3 direction = Camera.main.transform.right * InputHub.Instance.Move.x + Camera.main.transform.forward * InputHub.Instance.Move.y;
        direction.y = 0f;
        return direction.normalized;
    }

    // Applies movement using the CharacterController and the current speed/vertical velocity.
    private void ApplyMovement()
    {
        Vector3 velocity = MoveDirection * CurrentSpeed;
        velocity.y = VerticalSpeed;
        Controller.Move(velocity * Time.deltaTime);
    }

    public void OnHit()
    {
        SetState(HitState);
    }
}