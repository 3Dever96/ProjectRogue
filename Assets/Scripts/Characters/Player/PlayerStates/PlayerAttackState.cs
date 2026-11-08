using UnityEngine;

/// <summary>
/// Runs an attack for its configured duration, then returns to the previous player state.
/// </summary>
[System.Serializable]
public class PlayerAttackState : PlayerState
{
    // Values that define the current attack.
    private AttackData attackData;

    // Elapsed time since the current attack began.
    private float attackTimer;

    // Hitbox enabled while the attack is active.
    [SerializeField] private GameObject hitbox;

    [Header("Combo Timing")]
    // Time range during which another attack can be chained.
    [SerializeField, Min(0f)] private float comboWindowStart = 0.2f;
    [SerializeField, Min(0f)] private float comboWindowEnd = 0.6f;

    public override void StartState(PlayerController player)
    {
        // Initialize the attack data previously supplied by the attack manager.
        BeginAttack(player, attackData);
    }

    public override void UpdateState(PlayerController player)
    {
        // Attack behavior is currently driven by its timer in ChangeState.
    }

    public override void ChangeState(PlayerController player)
    {
        attackTimer += Time.deltaTime;

        // Allow chaining only within the configured window and before the attack ends.
        canAttack = attackTimer >= comboWindowStart
            && attackTimer <= comboWindowEnd
            && attackTimer < attackData.attackDuration;

        if (attackTimer >= attackData.attackDuration)
        {
            canAttack = false;
            // Return to the state that was active before the attack began.
            player.SetState(player.LastState);
        }
    }

    public override void EndState(PlayerController player)
    {
        // Disable the hitbox and clear attack-specific state when leaving.
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }

        attackData = default;
        canAttack = false;
    }

    /// <summary>
    /// Starts the next combo attack if the current attack is inside its chain window.
    /// </summary>
    public bool TryChainAttack(PlayerController player, AttackData nextAttack)
    {
        if (!canAttack)
        {
            return false;
        }

        // Reuse this state for the next attack in the combo.
        BeginAttack(player, nextAttack);
        return true;
    }

    /// <summary>
    /// Sets the data used when this attack state starts.
    /// </summary>
    public void GetAttackData(AttackData attackData)
    {
        this.attackData = attackData;
    }

    private void BeginAttack(PlayerController player, AttackData data)
    {
        attackData = data;
        attackTimer = 0f;
        canAttack = false;

        if (hitbox != null)
        {
            // Toggle it off and on so a chained attack begins with a fresh hitbox activation.
            hitbox.SetActive(false);
            hitbox.SetActive(true);
        }

        // Move and face forward using the active attack's movement values.
        player.MoveDirection = player.transform.forward;
        player.LookDirection = player.MoveDirection;
        player.CurrentSpeed = attackData.currentSpeed;
        player.VerticalSpeed = attackData.verticalSpeed;
    }
}
