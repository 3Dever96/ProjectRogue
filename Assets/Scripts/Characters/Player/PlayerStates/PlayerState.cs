using UnityEngine;

[System.Serializable]
public abstract class PlayerState
{
    public abstract void StartState(PlayerController player);
    public abstract void UpdateState(PlayerController player);
    public abstract void ChangeState(PlayerController player);
    public abstract void ExitState(PlayerController player);
}
