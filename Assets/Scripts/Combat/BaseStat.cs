
[System.Serializable]
public class BaseStat 
{
    public enum StatType
    {
        Health,
        Damage,
        Armor,
        Speed,
        AttackSpeed
    }
    public StatType statType;
    public float baseValue;
    public float modifiedValue;

    public BaseStat(StatType statType, float baseValue)
    {
        this.statType = statType;
        this.baseValue = baseValue;
        this.modifiedValue = baseValue;
    }
}