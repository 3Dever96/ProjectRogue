using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the player's current numeric stats, keyed by stat name.
/// Example keys might include "Health", "Attack", or "Speed".
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // Holds all current player stats in a name-to-value dictionary for easy lookup and modification.
    [SerializeField] public Dictionary<string, float> stats = new Dictionary<string, float>();
}
