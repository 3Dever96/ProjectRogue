using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class PlayerGroundState : PlayerState
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float runThreshold;
    [SerializeField] private float sprintThreshold;

    [SerializeField] private float stickForce;
    [SerializeField] private float jumpSpeed;

    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRecover;

    private float moveSpeed;
    private float moveThreshold;
    [SerializeField] private float currentStamina;

    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float groundCheckRadius;

    private bool canJump;
    private bool canSprint;
    private bool sprintRecover;

    public override void StartState(PlayerController player)
    {
        player.VerticalSpeed = stickForce;
        canJump = false;
        sprintRecover = false;
        currentStamina = maxStamina;
    }

    public override void UpdateState(PlayerController player)
    {
        player.MoveDirection = player.GetInputDirection().normalized;
        
        canSprint = !sprintRecover && currentStamina > 0f;

        if (!sprintRecover)
        {
            if (canSprint && InputHub.Instance.Sprint)
            {
                moveSpeed = sprintSpeed;
                moveThreshold = sprintThreshold;

                if (InputHub.Instance.Move != Vector2.zero)
                {
                    currentStamina = Mathf.Clamp(currentStamina - Time.deltaTime, 0f, maxStamina);
                    if (currentStamina <= 0f)
                    {
                        sprintRecover = true;
                    }
                }
                else
                {
                    currentStamina = Mathf.Clamp(currentStamina + staminaRecover * Time.deltaTime, 0f, maxStamina);
                }
            }
            else
            {
                moveSpeed = runSpeed;
                moveThreshold = runThreshold;
                currentStamina = Mathf.Clamp(currentStamina + staminaRecover * Time.deltaTime, 0f, maxStamina);
            }
        }
        else
        {
            moveSpeed = runSpeed;
            moveThreshold = runThreshold;
            currentStamina = Mathf.Clamp(currentStamina + staminaRecover * Time.deltaTime, 0f, maxStamina);
            if (currentStamina == maxStamina)
            {
                sprintRecover = false;
            }
        }

        player.CurrentSpeed = InputHub.Instance.Move != Vector2.zero ? moveSpeed * Mathf.Clamp(InputHub.Instance.Move.magnitude + moveThreshold, 0f, 1f) : 0f;
        player.LookDirection = player.MoveDirection != Vector3.zero ? player.MoveDirection : player.LookDirection;
        player.FaceDirection();

        if (InputHub.Instance.Jump && canJump)
        {
            player.VerticalSpeed = jumpSpeed;
        }

        canJump = !InputHub.Instance.Jump;
    }

    public override void ChangeState(PlayerController player)
    {
        if (player.VerticalSpeed > 0f || !player.CheckCollitions(groundOffset, groundCheckRadius))
        {
            player.SetState(player.AirState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        
    }
}
