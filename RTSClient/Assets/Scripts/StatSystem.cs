using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSystem : MonoBehaviour
{
    public enum StatType
    {
        First,
        Health,
        AggroRange,
        AttackRange,
        Damage,
        AttackSpeed,
        Gold,
        Last
    }
    [System.Serializable]
    class Stat
    {
        public StatType type;
        public float value;
        public float maxValue;
    }
    [SerializeField] private List<Stat> myStats = new List<Stat>();
    public delegate void UpdateStatFunction(StatType aType, float aValue, float aMaxValue);
    private Dictionary<StatType, UpdateStatFunction> callbacks = new Dictionary<StatType, UpdateStatFunction>();

    public void SetValue(StatType aType, float aValue)
    {
        foreach (var item in myStats)
        {
            if (aType == item.type)
            {
                if (item.value != aValue)
                {
                    item.value = aValue;
                    RunCallback(aType);
                }
                return;
            }
        }
        Debug.Log("Error");
        return;
    }
    public void AddValue(StatType aType, float aValue)
    {
        foreach (var item in myStats)
        {
            if (aType == item.type)
            {
                if (0 != aValue)
                {
                    item.value += aValue;
                    RunCallback(aType);
                    return;
                }
            }
        }
        Debug.Log("Error");
        return;
    }
    public static void TakeDamage(StatSystem aDefender, StatSystem aAttacker)
    {
        aDefender.AddValue(StatType.Health, -aAttacker.GetValue(StatType.Damage));
    }
    public float GetValue(StatType aType)
    {
        foreach (var item in myStats)
        {
            if (aType == item.type)
            {
                return item.value;
            }
        }
        Debug.Log("Error");
        return int.MinValue;
    }
    public float GetMaxValue(StatType aType)
    {
        foreach (var item in myStats)
        {
            if (aType == item.type)
            {
                return item.maxValue;
            }
        }
        Debug.Log("Error");
        return int.MinValue;
    }
    public void AddCallback(StatType aType, UpdateStatFunction aFunc)
    {
        if (callbacks.ContainsKey(aType))
        {
            callbacks[aType] += aFunc;
        }
        else
        {
            callbacks[aType] = aFunc;
        }
    }
    public void RunCallback(StatType aType)
    {
        if (callbacks.ContainsKey(aType))
        {
            foreach (var item in myStats)
            {
                if (item.type == aType)
                {
                    callbacks[aType].Invoke(aType, item.value, item.maxValue);
                    if(null != GetComponent<NetworkSync>())
                    {
                        References.ClientNet.CallRPC("UpdateStatRPC", UCNetwork.MessageReceiver.ServerOnly, -1, GetComponent<NetworkSync>().GetId(), (int)aType, item.value);
                    }
                    return;
                }
            }
        }
        Debug.Log("Error");
        return;
    }
}
