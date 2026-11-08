using UnityEngine;

/// <summary>
/// Contains the animation and gameplay values used for a single attack.
/// </summary>
[System.Serializable]
public struct AttackData
{

    // Animation played for the attack.
    public AnimationClip animation;

    // Horizontal and vertical movement speeds applied during the attack.
    public float currentSpeed;
    public float verticalSpeed;

    // How long the attack remains active.
    public float attackDuration;

    /// <summary>
    /// Creates attack data with the given animation and gameplay values.
    /// </summary>
    public AttackData(AnimationClip animation, float currentSpeed, float verticalSpeed, float attackDuration)
    {
        this.animation = animation;
        this.currentSpeed = currentSpeed;
        this.verticalSpeed = verticalSpeed;
        this.attackDuration = attackDuration;
    }
}