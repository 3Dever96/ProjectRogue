using UnityEngine;

/// <summary>
/// Represents an attack hit area that damages anything it overlaps.
/// This is typically attached to a weapon or attack collider that becomes active during an attack animation.
/// </summary>
public class Hitbox : MonoBehaviour
{
    // Reference to the attacking entity's stats so the hitbox can use the attack stat value.
    private EntityStats myStats;

    private void Start()
    {
        // Try to use stats on this object first; otherwise, search the parent hierarchy.
        myStats = GetComponent<EntityStats>();

        if (myStats == null)
        {
            myStats = GetComponentInParent<EntityStats>();
        }
    }

    /// <summary>
    /// Applies damage while another collider overlaps this hitbox.
    /// </summary>
    /// <param name="other">The collider that overlaps this hitbox.</param>
    private void OnTriggerStay(Collider other)
    {
        // Only damage objects that can receive damage.
        if (other.TryGetComponent(out IDamageable damageable))
        {
            // Use the attacker's ATK stat as the damage amount for this hit.
            damageable.TakeDamage(myStats.stats["ATK"].Value);
        }
    }
}
