using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "ScriptObj/ActionConditionSet")]
public class ActionConditionSet : ScriptableObject
{
    [SerializeField] private List<UnityEvent> conditions = new List<UnityEvent>();
    [SerializeField] private List<UnityEvent> actions = new List<UnityEvent>();



    public void RunConditions()
    {
        foreach (var item in conditions)
        {
            item.Invoke();
        }
    }
    public void RunActions()
    {
        foreach (var item in actions)
        {
            item.Invoke();
        }
    }
}
