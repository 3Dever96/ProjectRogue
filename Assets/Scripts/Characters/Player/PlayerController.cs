using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController Controller { get; private set; }

    public PlayerState CurrentState { get; private set; }
    [field: SerializeField] public PlayerGroundState GroundState { get; private set; } = new PlayerGroundState();
    [field: SerializeField] public PlayerAirState AirState { get; private set; } = new PlayerAirState();
    [field: SerializeField] public PlayerDashState DashState { get; private set; } = new PlayerDashState();

    public Vector3 MoveDirection { get; set; }
    public Vector3 LookDirection { get; set; }
    public float CurrentSpeed {  get; set; }
    public float VerticalSpeed {  get; set; }

    private void Start()
    {
        Controller = GetComponent<CharacterController>();

        SetState(GroundState);
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState(this);
            CurrentState.ChangeState(this);

            ApplyMovement();
        }
    }

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

    public void FaceDirection(float turnSpeed = 500f)
    {
        if (LookDirection == Vector3.zero) return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    public bool CheckCollitions(Vector3 originOffset, float radius)
    {
        return Physics.CheckSphere(transform.position + originOffset, radius, LayerMask.GetMask("Solid"));
    }

    public void ApplyMovement()
    {
        Vector3 velocity = CurrentSpeed * MoveDirection;
        velocity.y = VerticalSpeed;

        Controller.Move(velocity * Time.deltaTime);
    }

    public Vector3 GetInputDirection()
    {
        Vector3 direction = Camera.main.transform.right * InputHub.Instance.Move.x + Camera.main.transform.forward * InputHub.Instance.Move.y;
        direction.y = 0f;

        return direction;
    }
}
