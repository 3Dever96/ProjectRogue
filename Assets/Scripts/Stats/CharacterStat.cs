using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Represents a single character stat value and all modifiers applied to it.
/// This class recalculates the final stat value whenever it becomes stale.
/// </summary>
[System.Serializable]
public class CharacterStat
{
    // The base number before any modifiers are applied.
    public float BaseValue;

    // Gets the final computed stat value after all modifiers are applied.
    public virtual float Value
    {
        get
        {
            if (isDirty || BaseValue != lastBaseValue)
            {
                _value = CalculateFinalValue();
                lastBaseValue = BaseValue;
                isDirty = false;
            }

            return _value;
        }
    }

    // Tracks whether the cached value needs to be recalculated.
    protected bool isDirty = true;

    // Cached final value after applying all stat modifiers.
    protected float _value;

    // Remembers the last base value used when recalculating the stat.
    protected float lastBaseValue = float.MinValue;

    // All modifiers currently affecting this stat.
    protected readonly List<StatModifier> statModifiers;

    // Read-only view of the modifiers list.
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public CharacterStat(float baseValue) : this()
    {
        BaseValue = baseValue;
    }

    // Adds a new modifier and marks the current value as stale.
    public virtual void AddModifier(StatModifier statModifier)
    {
        statModifiers.Add(statModifier);
        statModifiers.Sort(SortModifiers);

        isDirty = true;
    }

    // Removes one matching modifier and recalculates when needed.
    public virtual bool RemoveModifier(StatModifier statModifier)
    {
        if (statModifiers.Remove(statModifier))
        {
            isDirty = true;
            return true;
        }

        return false;
    }

    // Removes all modifiers from the given source object.
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                statModifiers.RemoveAt(i);
                didRemove = true;
            }
        }

        if (didRemove)
        {
            isDirty = true;
        }

        return didRemove;
    }

    // Combines base value and modifiers into the final stat result.
    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float percentAdd = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModType.PercentAdd)
            {
                percentAdd += mod.Value;
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + percentAdd;
                    percentAdd = 0;
                }
            }
            else if (mod.Type == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
        }

        return Mathf.Floor(finalValue);
    }

    // Sorts modifiers by their order so calculations are deterministic.
    protected virtual int SortModifiers(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
        {
            return -1;
        }
        else if (a.Order > b.Order)
        {
            return 1;
        }

        return 0;
    }
}
