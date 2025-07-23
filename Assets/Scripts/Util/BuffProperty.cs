using UnityEngine;

public enum AttributeModifyType
{
    Add = 0,
    AddPercentage = 1,
    Multiply = 2,
}
//可修改的float数值
public struct BuffProperty
{
    public float cachedValue { get; private set; }
    public float baseValue { get; private set; }
    private float bonus;
    private float riser;
    private float multiplier;
    private float maxValue;
    private bool isInt;
    public BuffProperty(float value, float max = -1, bool isInt = false)
    {
        baseValue = cachedValue = value;
        bonus = 0;
        riser = 0;
        multiplier = 1;
        maxValue = max;
        this.isInt = isInt;
    }
    private void RecalculateValue()
    {
        cachedValue = (baseValue * (1 + riser) + bonus) * multiplier;
        if (maxValue >= 0)
            cachedValue = Mathf.Min(cachedValue, maxValue);
        if (isInt)
            cachedValue = Mathf.RoundToInt(cachedValue);
    }
    //修改基础参数，例如全局加成
    public void OverrideBaseValue(float newValue)
    {
        baseValue = newValue;
        RecalculateValue();
    }
    //修改当前数值
    public void ModifiValue(float modifier, AttributeModifyType modifiType)
    {
        switch (modifiType)
        {
            case AttributeModifyType.Add:
                bonus += modifier;
                break;
            case AttributeModifyType.AddPercentage:
                riser += modifier;
                break;
            case AttributeModifyType.Multiply:
                multiplier *= modifier;
                break;
        }
        RecalculateValue();
    }
}