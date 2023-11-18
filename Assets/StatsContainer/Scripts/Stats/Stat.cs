using UnityEngine;
using System;

/// <summary>
/// A simple class for storing a stat type as well as its current and maximum values.
/// </summary>
[Serializable] 
public class Stat
{
    [field: SerializeField] public StatType StatType { get; private set; }
    [field: SerializeField, HideInInspector] public float Value { get; private set; }
    [field: SerializeField, HideInInspector] public float MaxValue { get; private set; }

    public Stat(StatType statType)
    {
        StatType = statType;
    }

    /// <summary>
    /// Gets a link to a stat from the stats container. Used in other classes where the stat is stored as a variable.
    /// </summary>
    /// <param name="statType">Type of stat to search.</param>
    /// <param name="statContainer">Container of stats in which to search.</param>
    /// <returns>Found stat or null.</returns>
    public static Stat BindStat(StatType statType, StatsContainer statContainer) =>
        statContainer.GetStat(statType);

    /// <summary>
    /// Gets a link to a stat from the stats container. Used in other classes where the stat is stored as a variable.
    /// </summary>
    /// <param name="stat">A stat with a specific type.</param>
    /// <param name="statContainer">Container of stats in which to search.</param>
    /// <returns>Found stat or null.</returns>
    public static Stat BindStat(Stat stat, StatsContainer statContainer) =>
        statContainer.GetStat(stat.StatType);

    public void AddValue(float value) =>
        Value = Mathf.Clamp(Value + value, 0, MaxValue);
    public void SubstractValue(float value) =>
        Value = Mathf.Clamp(Value - value, 0, MaxValue);

    /// <summary>
    /// Adds current and maximum stat value.
    /// </summary>
    /// <param name="stat">The stat whose value needs to be added to the current stat.</param>
    public void AddValues(Stat stat)
    {
        Value += stat.Value;
        MaxValue += stat.MaxValue;
    }

    /// <summary>
    /// Substracts current and maximum stat value.
    /// </summary>
    /// <param name="stat">The stat whose value needs to be subtracted from the current stat.</param>
    public void RemoveValues(Stat stat)
    {
        Value -= stat.Value;
        MaxValue -= stat.MaxValue;
    }
}
