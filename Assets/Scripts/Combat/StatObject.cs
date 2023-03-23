using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BaseStat;

[CreateAssetMenu(fileName = "StatObject", menuName = "Action Platformer Stats", order = 0)]
public class StatObject : ScriptableObject
{
    [SerializeField]  List<BaseStat> characterStats = new List<BaseStat>();

    private void InitializeStat(StatType statType, float value)
    {
       foreach(BaseStat stat in characterStats)
       {
            stat.modifiedValue = stat.baseValue;
       }
    }
    // Get a single stat's modified value
    public float GetStatBaseValue(StatType statType)
    {
        return characterStats.FirstOrDefault(stat => stat.statType == statType)?.baseValue ?? 0;
    }
    public BaseStat GetStat(StatType statType)
    {
        return characterStats.FirstOrDefault(stat => stat.statType == statType);
    }

    // Modify a single stat's value
    public void ModifyStat(StatType statType, float amount)
    {
        GetStat(statType).modifiedValue -= amount;
    }
}
