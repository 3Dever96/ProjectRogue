using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Starts and chains the ground combo, or starts the single configured air attack.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerAttackManager : MonoBehaviour
{
    // Ground combo entries use contiguous keys; the air attack uses one fixed key.
    private const string GroundAttackKeyPrefix = "GroundAttack";
    private const string AirAttackKey = "AirAttack";

    private PlayerController player;

    [SerializeField] private Dictionary<string, AttackData> attackData = new Dictionary<string, AttackData>();

    // Time grounded without continuing the combo before its index resets.
    [SerializeField, Min(0f)] private float comboResetDelay = 1f;

    // How long an attack press is retained while waiting for the chain window.
    [SerializeField, Min(0f)] private float comboInputBuffer = 0.15f;

    // Index of the next ground combo entry to use.
    private int currentGroundAttack;

    // Number of consecutive GroundAttack entries found in the configured data.
    private int groundAttackCount;

    // Tracks idle time and the lifetime of a buffered combo input.
    private float comboIdleTimer;
    private float bufferedAttackTime;

    // Tracks attack-button edges so a held button is treated as one press.
    private bool attackButtonWasPressed;

    // How long the player must wait between air attacks while airborne.
    [SerializeField, Min(0f)] private float airAttackCooldown = 0.35f;

    // Whether a fresh press is waiting for the ground combo window.
    private bool hasBufferedAttack;
    private float airAttackCooldownTimer;

    private void Start()
    {
        player = GetComponent<PlayerController>();

        // Count consecutive entries so the combo can loop without an out-of-range key.
        while (attackData.ContainsKey(GroundAttackKeyPrefix + groundAttackCount))
        {
            groundAttackCount++;
        }

        if (groundAttackCount == 0)
        {
            Debug.LogError($"No ground attacks configured. Add an attack with the key \"{GroundAttackKeyPrefix}0\".", this);
        }
    }

    private void Update()
    {
        // Detect a new press rather than treating a held button as repeated input.
        bool attackButtonIsPressed = InputHub.Instance.Attack;
        bool attackButtonWasPressedThisFrame = attackButtonIsPressed && !attackButtonWasPressed;
        attackButtonWasPressed = attackButtonIsPressed;

        // Only grounded attacks can chain; aerial attacks are a single, unchained move.
        if (attackButtonWasPressedThisFrame
            && player.CurrentState == player.AttackState
            && player.LastState == player.GroundState)
        {
            hasBufferedAttack = true;
            bufferedAttackTime = comboInputBuffer;
        }

        if (player.CurrentState == player.AttackState)
        {
            // Time spent attacking does not count toward the grounded combo reset.
            comboIdleTimer = 0f;
        }
        else if (player.CurrentState == player.GroundState)
        {
            // Reset the sequence after the player has been idle on the ground.
            airAttackCooldownTimer = 0f;
            comboIdleTimer += Time.deltaTime;
            if (comboIdleTimer >= comboResetDelay)
            {
                currentGroundAttack = 0;
            }
        }
        else
        {
            // Airborne or other states cancel the grounded sequence and buffered input.
            currentGroundAttack = 0;
            comboIdleTimer = 0f;
            hasBufferedAttack = false;
        }

        if (player.CurrentState == player.GroundState)
        {
            hasBufferedAttack = false;
            if (attackButtonWasPressedThisFrame && player.CurrentState.canAttack)
            {
                // Begin a combo immediately when the player is grounded and eligible.
                StartNextGroundAttack();
            }

            return;
        }

        if (player.CurrentState == player.AirState)
        {
            // Air attacks are not chained or buffered; each requires a new press.
            hasBufferedAttack = false;
            airAttackCooldownTimer = Mathf.Max(0f, airAttackCooldownTimer - Time.deltaTime);
            if (attackButtonWasPressedThisFrame && player.CurrentState.canAttack && airAttackCooldownTimer <= 0f)
            {
                StartAirAttack();
            }

            return;
        }

        if (player.CurrentState != player.AttackState || !hasBufferedAttack)
        {
            return;
        }

        // Wait for the chain window, discarding the press if its buffer expires first.
        if (!player.AttackState.canAttack)
        {
            bufferedAttackTime -= Time.deltaTime;
            if (bufferedAttackTime <= 0f)
            {
                hasBufferedAttack = false;
            }

            return;
        }

        if (StartNextGroundAttack())
        {
            hasBufferedAttack = false;
        }
    }

    private bool StartNextGroundAttack()
    {
        // Resolve the next entry in the configured ground combo.
        string attackKey = GroundAttackKeyPrefix + currentGroundAttack;
        if (!attackData.TryGetValue(attackKey, out AttackData nextAttack))
        {
            Debug.LogError($"Ground attack data is missing for key \"{attackKey}\".", this);
            currentGroundAttack = 0;
            return false;
        }

        // A chain replaces the active attack; otherwise, transition into the attack state.
        if (player.CurrentState == player.AttackState)
        {
            if (!player.AttackState.TryChainAttack(player, nextAttack))
            {
                return false;
            }
        }
        else
        {
            player.AttackState.GetAttackData(nextAttack);
            player.SetState(player.AttackState);
        }

        // Advance only after successfully starting the selected attack.
        currentGroundAttack = (currentGroundAttack + 1) % groundAttackCount;
        comboIdleTimer = 0f;
        return true;
    }

    private void StartAirAttack()
    {
        // Air attacks can be repeated, but not instantly spammed while airborne.
        if (airAttackCooldownTimer > 0f)
        {
            return;
        }

        // The air move is a single fixed entry rather than an indexed combo.
        if (!attackData.TryGetValue(AirAttackKey, out AttackData airAttack))
        {
            Debug.LogError($"Air attack data is missing for key \"{AirAttackKey}\".", this);
            return;
        }

        // Preserve the air state as LastState so the attack returns to it on completion.
        player.AttackState.GetAttackData(airAttack);
        player.SetState(player.AttackState);
        airAttackCooldownTimer = airAttackCooldown;
        hasBufferedAttack = false;
    }
}
