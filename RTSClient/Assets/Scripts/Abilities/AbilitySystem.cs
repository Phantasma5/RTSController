using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//remember ryan
//seperate out the conditions from the effects
public class AbilitySystem : MonoBehaviour
{
    public List<Ability> Abilities
    {
        get
        {
            return abilities;
        }
    }

    [SerializeField] private List<Ability> abilities = new List<Ability>();
    private Ability activeAbility;
    [HideInInspector] public bool failedChecks;
    public enum AbilityHotkey
    {
        First,
        Q,
        W,
        E,
        R,
        Last
    }

    [System.Serializable]
    public class Ability
    {
        [SerializeField] private string abilityName;
        [SerializeField] private float pulseTime;
        [HideInInspector] private float startTime;

        [SerializeField] private ActionConditionSet startActions;//scriptable objects
        [SerializeField] private ActionConditionSet refundActions;
        [SerializeField] private ActionConditionSet endActions;
        public Ability(AbilityHotkey aKeypress, List<Ability> abilities)
        {
            int index = -1;
            switch (aKeypress)
            {
                case AbilityHotkey.Q:
                    index = 0;
                    break;
                case AbilityHotkey.W:
                    index = 1;
                    break;
                case AbilityHotkey.E:
                    index = 2;
                    break;
                case AbilityHotkey.R:
                    index = 3;
                    break;
            }
            //manually doing a deep copy out because C# is weird about value vs reference types
            this.abilityName = abilities[index].abilityName;
            this.pulseTime = abilities[index].pulseTime;
            this.startActions = abilities[index].startActions;
            this.refundActions = abilities[index].refundActions;
            this.endActions = abilities[index].endActions;
            this.startTime = Time.time;
        }
        public bool PulseCheck()
        {
            if ((Time.time - startTime) < pulseTime)
            {
                //TODO: Check pulse intervel and execute pulse actions
                return false;//pulse time has not elapsed
            }
            return true;//pulse time has elapsed
        }
        public void RunStartConditions()
        {
            startActions.RunConditions();
        }
        public void RunStartActions()
        {
            startActions.RunActions();
        }
        public void RunEndConditions()
        {
            endActions.RunConditions();
        }
        public void RunEndActions()
        {
            endActions.RunActions();
        }
        public void RunRefundActions()
        {
            refundActions.RunActions();
        }
    }
    public void Refund()
    {
        if (null == activeAbility)
        {
            return;
        }
        activeAbility.RunRefundActions();
        activeAbility = null;
        return;
    }
    public bool Execute(AbilityHotkey aKeypress)
    {
        bool done = false;
        if (activeAbility == null)
        {
            activeAbility = new Ability(aKeypress, abilities);
            failedChecks = false;
            References.SendingAbilitySystem = this;
            activeAbility.RunStartConditions();
            if (failedChecks)//failed condition
            {
                done = true;
                activeAbility = null;
                return done;
            }
            else//passed all conditions
            {
                activeAbility.RunStartActions();
            }
        }
        if (activeAbility.PulseCheck())//true if pulseTime has elapsed
        {
            failedChecks = false;
            References.SendingAbilitySystem = this;
            activeAbility.RunEndConditions();
            if (failedChecks)//failed condition
            {
                done = true;
                return done;
            }
            else//passed all conditions
            {
                activeAbility.RunEndActions();
            }

            activeAbility = null;
            done = true;
        }
        return done;
    }



    private void FailChecks()
    {
        failedChecks = true;
    }
}
