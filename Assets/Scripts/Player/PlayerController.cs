using UnityEngine;

namespace ProjectRogue.Player
{
    [RequireComponent(typeof(InputManager)), RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public PlayerMoveState CurrentState { get; private set; }

        public float Gravity { get { return gravity; } }
        public float JumpSpeed { get { return jumpSpeed; } }
        public float StickForce { get {  return stickForce; } }
        public float TurnSpeed { get { return turnSpeed; } }

        public float CurrentSpeed { get; set; }
        public float VerticalSpeed { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Direction { get; set; }

        public CharacterController Controller { get; private set; }
        public InputManager Input { get; private set; }

        [Header("Universal Variables")]
        [SerializeField] float gravity;
        [SerializeField] float jumpSpeed;
        [SerializeField] float stickForce;
        [SerializeField] float turnSpeed;

        [Header("States")]
        public PlayerGroundState groundState = new PlayerGroundState();
        public PlayerAirState airState = new PlayerAirState();

        void Start()
        {
            Controller = GetComponent<CharacterController>();
            Input = GetComponent<InputManager>();

            SetState(groundState);
        }

        void FixedUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.UpdateState(this);
                CurrentState.ChangeState(this);

                Controller.Move(Velocity * Time.deltaTime);
            }
        }

        public void SetState(PlayerMoveState newState)
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

        public void FaceDirection(Vector3 direction, float speed)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), speed * Time.deltaTime);
        }
    }
}
