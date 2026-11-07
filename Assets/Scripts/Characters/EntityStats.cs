using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks an entity's stats and health, and handles damage, healing, and hit feedback.
/// </summary>
public class EntityStats : MonoBehaviour
{
    [Header("Entity Stats")]
    // Maps stat names to their corresponding stat data.
    [SerializeField] public Dictionary<string, CharacterStat> stats = new Dictionary<string, CharacterStat>();

    // Current health value, initialized from the HP stat when the entity starts.
    [SerializeField] private float currentHp;

    [Header("Hit Visuals")]
    // Renderer whose material color is changed to show damage and invincibility.
    [SerializeField] private Renderer avatar;

    // Color displayed when the entity is hit.
    [SerializeField] private Color hitColor;

    // Duration of the brief freeze-frame effect after taking damage.
    [SerializeField] private float hitStopTime;

    // Duration of the invincibility period after taking damage.
    [SerializeField] private float maxIFrames;

    // Length of each on/off phase in the invincibility blink effect.
    [SerializeField] private float maxBlinkTime;

    // Prevents the entity from taking damage again during its invincibility period.
    private bool isInvincible;

    [Header("Events")]
    // Invoked whenever the entity's health changes.
    public UnityEvent OnHpChange;

    // Invoked when the entity's health reaches zero or below.
    public UnityEvent OnDeath;

    private void Start()
    {
        // Initialize current health from the configured HP stat.
        currentHp = stats["HP"].Value;
    }

    // Applies damage unless the entity is currently invincible.
    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHp -= damage;
        OnHpChange?.Invoke();

        // Notify listeners when damage reduces health to zero or less.
        if (currentHp <= 0f)
        {
            OnDeath?.Invoke();
        }

        // Start hit feedback and temporarily prevent further damage.
        StartCoroutine(InvincibilityRoutine());
    }

    // Restores health and notifies listeners of the change.
    public void Heal(float healAmount)
    {
        currentHp += healAmount;
        OnHpChange?.Invoke();
    }

    // Runs the hit-stop, invincibility timer, and blinking visual effect.
    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // Save the current color and time scale so they can be restored afterward.
        Color originalColor = avatar.material.color;
        float previousTimeScale = Time.timeScale;

        // Briefly freeze gameplay while showing the hit color.
        avatar.material.color = hitColor;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);

        // When adding pause support, check the GameManager state before restoring time:
        // only resume gameplay time if the game is not paused. The GameManager should restore time when unpaused.
        Time.timeScale = previousTimeScale;

        // Clamp the configured duration and blink interval to safe minimum values.
        float invincibilityDuration = Mathf.Max(0f, maxIFrames);
        float blinkInterval = Mathf.Max(0.01f, maxBlinkTime);
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            // Use scaled delta time so the invincibility timer stops while the game is paused (timeScale is zero).
            // Alternate between the hit color and the entity's original color.
            avatar.material.color = Mathf.FloorToInt(elapsed / blinkInterval) % 2 == 0 ? hitColor : originalColor;

            // Wait one frame before updating the timer and blink color again.
            yield return null;
            elapsed += Time.deltaTime;
        }

        // Restore the original appearance and allow the entity to take damage again.
        avatar.material.color = originalColor;
        isInvincible = false;
    }
}
