using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stores the entity's current numeric stats, keyed by stat name.
/// Example keys might include "Health", "Attack", or "Speed".
/// </summary>
public class EntityStats : MonoBehaviour
{
    [Header("Entity Stats")]
    // Holds all current entity stats in a name-to-value dictionary for easy lookup and modification.
    [SerializeField] public Dictionary<string, CharacterStat> stats = new Dictionary<string, CharacterStat>();
    [SerializeField] private float currentHp;

    [Header("Hit Visuals")]
    [SerializeField] private Renderer avatar;
    [SerializeField] private Color hitColor;
    [SerializeField] private float hitStopTime;
    [SerializeField] private float maxIFrames;
    [SerializeField] private float maxBlinkTime;

    private bool isInvincible;

    [Header("Events")]
    public UnityEvent OnHpChange;
    public UnityEvent OnDeath;

    private void Start()
    {
        currentHp = stats["HP"].Value;
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHp -= damage;
        OnHpChange?.Invoke();
        if (currentHp <= 0f)
        {
            OnDeath?.Invoke();
        }
        StartCoroutine(InvincibilityRoutine());
    }

    public void Heal(float healAmount)
    {
        currentHp += healAmount;
        OnHpChange?.Invoke();
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        Color originalColor = avatar.material.color;
        float previousTimeScale = Time.timeScale;

        avatar.material.color = hitColor;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);

        // When adding pause support, check the GameManager state before restoring time:
        // only resume gameplay time if the game is not paused. The GameManager should restore time when unpaused.
        Time.timeScale = previousTimeScale;

        float invincibilityDuration = Mathf.Max(0f, maxIFrames);
        float blinkInterval = Mathf.Max(0.01f, maxBlinkTime);
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            // Use scaled delta time so the invincibility timer stops while the game is paused (timeScale is zero).
            avatar.material.color = Mathf.FloorToInt(elapsed / blinkInterval) % 2 == 0 ? hitColor : originalColor;

            yield return null;
            elapsed += Time.deltaTime;
        }

        avatar.material.color = originalColor;
        isInvincible = false;
    }
}
