using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the entity's current numeric stats, keyed by stat name.
/// Example keys might include "Health", "Attack", or "Speed".
/// </summary>
public class EntityStats : MonoBehaviour
{
    // Holds all current entity stats in a name-to-value dictionary for easy lookup and modification.
    [SerializeField] public Dictionary<string, CharacterStat> stats = new Dictionary<string, CharacterStat>();
}
