using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for storing and displaying a collection of stats.
/// </summary>
public class StatsContainer : MonoBehaviour
{
    private Stat _lastUsedStat;

    [SerializeField]
    // If you change the variable name,
    // do not forget to change it in the StatsContainerTabsEditor script,
    // otherwise the serialization will break.
    private List<Stat> _stats = new List<Stat>();

    /// <summary>
    /// Searches for stats by type.
    /// </summary>
    /// <param name="statType">Type of stat to search (SO)</param>
    /// <returns>Found stat or null</returns>
    private Stat FindStat(StatType statType)
    {
        for (int i = 0; i < _stats.Count; i++)
            if (StatType.Compare(_stats[i].StatType, statType))
                return _stats[i];

        return null;
    }

    /// <summary>
    /// Returns the current stat value by stat type.
    /// </summary>
    /// <param name="statType">Stat type to get it from the stat container.</param>
    public float GetStatValue(StatType statType)
    {
        if (_lastUsedStat != null && StatType.Compare(_lastUsedStat.StatType, statType))
            return _lastUsedStat.Value;

        var stat = FindStat(statType);
        if (stat == null) return -1;

        _lastUsedStat = stat;
        return _lastUsedStat.Value;
    }

    public void AddEmptyStat() =>
        _stats.Add(new Stat(null));

    /// <summary>
    /// If a stat is already present in the stat collection, then it simply adds its value to the one found, otherwise it adds a new type with its values.
    /// </summary>
    /// <param name="newStat"></param>
    public void AddStat(Stat newStat)
    {
        var stat = FindStat(newStat.StatType);
        if (stat == null)
        {
            stat = new Stat(newStat.StatType);
            _stats.Add(stat);
        }
        stat.AddValues(newStat);
    }

    /// <summary>
    /// Searches for stats by type and deletes if found.
    /// </summary>
    public void RemoveStat(StatType statType)
    {
        var oldStat = FindStat(statType);
        if (oldStat != null)
            _stats.Remove(oldStat);
    }

    public Stat GetStatByIndex(int index) =>
        _stats[index];

    public Stat GetStat(StatType statType) => 
        FindStat(statType);
}
