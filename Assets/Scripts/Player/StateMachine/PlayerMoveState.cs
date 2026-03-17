using UnityEngine;

namespace ProjectRogue.Player
{
    public abstract class PlayerMoveState
    {
        public bool canAttack;
        public abstract void StartState(PlayerController player);
        public abstract void UpdateState(PlayerController player);
        public abstract void ChangeState(PlayerController player);
        public abstract void ExitState(PlayerController player);
    }
}
