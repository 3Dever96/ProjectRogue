using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    public CharacterController Controller { get; private set; }
    public PlayerStats Stats { get; private set; }

    [Header("Collision Detection")]
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }
    [field: SerializeField] public Vector3 GroundOffset { get; private set; }
    [field: SerializeField] public Vector3 HeadOffset { get; private set; }
    [field: SerializeField] public float CheckRadius { get; private set; }

    [Header("Player States")]
    [field: SerializeField] public PlayerGroundState GroundState { get; private set; } = new PlayerGroundState();
    [field: SerializeField] public PlayerAirState AirState { get; private set; } = new PlayerAirState();
    public PlayerState CurrentState { get; private set; }

    public float CurrentSpeed { get; set; }
    public float VerticalSpeed { get; set; }
    public Vector3 MoveDirection { get; set; }
    public Vector3 LookDirection { get; set; }

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
        Stats = GetComponent<PlayerStats>();

        SetState(GroundState);
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
        CurrentState?.ChangeState(this);

        ApplyMovement();
    }

    public void SetState(PlayerState state)
    {
        CurrentState?.EndState(this);
        CurrentState = state;
        CurrentState?.StartState(this);
    }

    public void FaceDirection(float turnSpeed = 500f)
    {
        if (LookDirection == Vector3.zero)
        {
            return;
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    public bool CheckCollision(Vector3 origin, float radius, LayerMask layerMask)
    {
        return Physics.CheckSphere(transform.position + origin, radius, layerMask);
    }

    public Vector3 GetDirection()
    {
        Vector3 direction = Camera.main.transform.right * InputHub.Instance.Move.x + Camera.main.transform.forward * InputHub.Instance.Move.y;
        direction.y = 0f;
        return direction.normalized;
    }

    private void ApplyMovement()
    {
        Vector3 velocity = MoveDirection * CurrentSpeed;
        velocity.y = VerticalSpeed;
        Controller.Move(velocity * Time.deltaTime);
    }
}