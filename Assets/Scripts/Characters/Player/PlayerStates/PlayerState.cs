using UnityEngine;

/// <summary>
/// Abstract foundation defining mandatory operational lifecycle stages within the custom state machine.
/// </summary>
[System.Serializable]
public abstract class PlayerState
{
    public abstract void StartState(PlayerController player);
    public abstract void UpdateState(PlayerController player);
    public abstract void ChangeState(PlayerController player);
    public abstract void ExitState(PlayerController player);
}
