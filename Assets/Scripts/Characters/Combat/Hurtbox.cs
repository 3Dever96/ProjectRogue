using UnityEngine;

/// <summary>
/// Detects incoming damage and forwards it to the owning entity's stats.
/// This is typically attached to a child collider representing the entity's vulnerable area.
/// </summary>
public class Hurtbox : MonoBehaviour, IDamageable
{
    // Reference to the stats component that should receive incoming damage.
    private EntityStats myStats;

    private void Start()
    {
        // Try to find stats on this GameObject first; if it is on a parent, fall back to that.
        myStats = GetComponent<EntityStats>();

        if (myStats == null)
        {
            myStats = GetComponentInParent<EntityStats>();
        }
    }

    /// <summary>
    /// Applies damage to the entity represented by this hurtbox.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    public void TakeDamage(float damage)
    {
        // Forward the damage to the entity's stats container.
        myStats.TakeDamage(damage);
    }
}
