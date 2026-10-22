using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// State handler processing running, sprinting, stamina tracking, and jump triggers while grounded.
/// </summary>
[System.Serializable]
public class PlayerGroundState : PlayerState
{
    [Header("Speed Thresholds")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float runThreshold;
    [SerializeField] private float sprintThreshold;

    [Header("Forces")]
    [SerializeField] private float stickForce; // Keeps player glued to slopes/steps
    [SerializeField] private float jumpSpeed;

    [Header("Stamina System")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRecover;
    [SerializeField] private float currentStamina;

    private float moveSpeed;
    private float moveThreshold;

    [Header("Collision Bounds")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float groundCheckRadius;

    private bool canJump;
    private bool canSprint;
    private bool sprintRecover; // Enforces a penalty pause if stamina empties completely
    private bool canDash;

    public override void StartState(PlayerController player)
    {
        player.VerticalSpeed = stickForce;
        canJump = false;
        sprintRecover = false;
        currentStamina = maxStamina;
        canDash = false;
    }

    public override void UpdateState(PlayerController player)
    {
        player.MoveDirection = player.GetInputDirection().normalized;

        canSprint = !sprintRecover && currentStamina > 0f;

        // --- Stamina and Speed Processing ---
        if (!sprintRecover)
        {
            if (canSprint && InputHub.Instance.Sprint)
            {
                moveSpeed = sprintSpeed;
                moveThreshold = sprintThreshold;

                if (InputHub.Instance.Move != Vector2.zero)
                {
                    // Burn stamina while actively moving and sprinting
                    currentStamina = Mathf.Clamp(currentStamina - Time.deltaTime, 0f, maxStamina);
                    if (currentStamina <= 0f)
                    {
                        sprintRecover = true;
                    }
                }
                else
                {
                    // Regain stamina if holding sprint button but standing still
                    currentStamina = Mathf.Clamp(currentStamina + staminaRecover * Time.deltaTime, 0f, maxStamina);
                }
            }
            else
            {
                // Regular walk/run settings
                moveSpeed = runSpeed;
                moveThreshold = runThreshold;
                currentStamina = Mathf.Clamp(currentStamina + staminaRecover * Time.deltaTime, 0f, maxStamina);
            }
        }
        else
        {
            // Forced cooldown: locked to run speed until stamina fully replenishes
            moveSpeed = runSpeed;
            moveThreshold = runThreshold;
            currentStamina = Mathf.Clamp(currentStamina + staminaRecover * Time.deltaTime, 0f, maxStamina);
            if (currentStamina == maxStamina)
            {
                sprintRecover = false;
            }
        }

        // Apply calculated speeds scaled by analog stick depth
        player.CurrentSpeed = InputHub.Instance.Move != Vector2.zero ? moveSpeed * Mathf.Clamp(InputHub.Instance.Move.magnitude + moveThreshold, 0f, 1f) : 0f;
        player.LookDirection = player.MoveDirection != Vector3.zero ? player.MoveDirection : player.LookDirection;
        player.FaceDirection();

        // --- Jump Input Action ---
        if (InputHub.Instance.Jump && canJump)
        {
            player.VerticalSpeed = jumpSpeed;
        }

        canJump = !InputHub.Instance.Jump; // Key release buffer rule
    }

    public override void ChangeState(PlayerController player)
    {
        // Fall or enter air state if moving up or running off ledges
        if (player.VerticalSpeed > 0f || !player.CheckCollitions(groundOffset, groundCheckRadius))
        {
            player.SetState(player.AirState);
        }

        // Handle dash transition
        if (InputHub.Instance.Dash && canDash)
        {
            player.SetState(player.DashState);
        }

        canDash = !InputHub.Instance.Dash;
    }

    public override void ExitState(PlayerController player)
    {
        // Intentional cleanup placeholder
    }
}
