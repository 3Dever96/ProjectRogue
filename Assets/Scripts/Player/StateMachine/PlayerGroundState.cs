using UnityEngine;

namespace ProjectRogue.Player
{
    [System.Serializable]
    public class PlayerGroundState : PlayerMoveState
    {
        [SerializeField] float runSpeed;
        [SerializeField] float accel;
        [SerializeField] float decel;
        [SerializeField] float fric;
        [SerializeField] float turnAngle;

        float moveSpeed;

        bool canJump;

        public override void StartState(PlayerController player)
        {
            player.VerticalSpeed = player.StickForce;
            player.Direction = player.transform.forward;

            canJump = false;
        }

        public override void UpdateState(PlayerController player)
        {
            Vector3 direction = Camera.main.transform.right * player.Input.Move.x + Camera.main.transform.forward * player.Input.Move.y;
            direction.y = 0f;
            direction = direction.normalized;

            if (player.Input.Move != Vector2.zero)
            {
                if (Vector3.Angle(direction, player.Direction) > turnAngle)
                {
                    player.CurrentSpeed -= decel * Time.deltaTime;

                    if (player.CurrentSpeed <= 0f)
                    {
                        player.CurrentSpeed = 0f;
                        player.Direction = direction;
                    }
                }
                else
                {
                    player.CurrentSpeed = Mathf.Clamp(player.CurrentSpeed + accel * Time.deltaTime, 0f, moveSpeed);

                    player.Direction = direction;
                }
            }
            else
            {
                player.CurrentSpeed -= Mathf.Min(fric * Time.deltaTime, player.CurrentSpeed);
            }

            moveSpeed = player.Input.Move.magnitude * runSpeed;

            player.FaceDirection(player.Direction, player.TurnSpeed);

            if (player.Input.Jump && canJump)
            {
                player.VerticalSpeed = player.JumpSpeed;
            }

            if (!player.Input.Jump && !canJump)
            {
                canJump = true;
            }

            Vector3 velocity = player.CurrentSpeed * player.Direction;
            velocity.y = player.VerticalSpeed;

            player.Velocity = velocity;
        }

        public override void ChangeState(PlayerController player)
        {
            if (player.VerticalSpeed > 0f || !Physics.CheckSphere(player.transform.position, player.Controller.radius - 0.01f, LayerMask.GetMask("Solid")))
            {
                player.SetState(player.airState);
            }
        }

        public override void ExitState(PlayerController player)
        {
            
        }
    }
}
