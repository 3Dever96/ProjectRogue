using UnityEngine;

/// <summary>
/// Base class for all player behavior states.
/// Each concrete state defines how the player starts, updates, transitions, and exits a behavior.
/// </summary>
[System.Serializable]
public abstract class PlayerState
{
    // Whether the attack manager may start or chain an attack in this state.
    public bool canAttack;

    // Called when this state becomes active.
    public abstract void StartState(PlayerController player);

    // Called every frame while this state is active.
    public abstract void UpdateState(PlayerController player);

    // Called to trigger a transition to another state.
    public abstract void ChangeState(PlayerController player);

    // Called when leaving this state so cleanup or reset logic can happen.
    public abstract void EndState(PlayerController player);
}