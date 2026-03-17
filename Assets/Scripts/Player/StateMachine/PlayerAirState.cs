using UnityEngine;

namespace ProjectRogue.Player
{
    [System.Serializable]
    public class PlayerAirState : PlayerMoveState
    {
        Vector3 velocity;

        public override void StartState(PlayerController player)
        {
            velocity = player.Velocity;
        }

        public override void UpdateState(PlayerController player)
        {
            player.FaceDirection(player.Direction, player.TurnSpeed);

            if (!player.Input.Jump || Physics.CheckSphere(player.transform.position + Vector3.up * player.Controller.height, player.Controller.radius - 0.01f, LayerMask.GetMask("Solid")))
            {
                player.VerticalSpeed = Mathf.Min(0f, player.VerticalSpeed);
            }

            player.VerticalSpeed += player.Gravity * Time.deltaTime;

            velocity.y = player.VerticalSpeed;

            player.Velocity = velocity;
        }

        public override void ChangeState(PlayerController player)
        {
            if (player.VerticalSpeed <= 0f && Physics.CheckSphere(player.transform.position, player.Controller.radius - 0.01f, LayerMask.GetMask("Solid")))
            {
                player.SetState(player.groundState);
            }
        }

        public override void ExitState(PlayerController player)
        {
            
        }
    }
}
