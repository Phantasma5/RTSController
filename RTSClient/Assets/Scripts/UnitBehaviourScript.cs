using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBehaviourScript : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    private LinkedList<Action> myQueue = new LinkedList<Action>();
    private Action currentAction;
    private StatSystem myStatSystem;
    private AbilitySystem myAbilitySystem;
    private float attackCD;
    public enum ActionType
    {
        First,
        Movement,
        Attack,
        AttackMove,
        Stop,
        Ability,
        Patrol,
        Last
    }
    private void Start()
    {
        attackCD = Time.time;
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myStatSystem = GetComponent<StatSystem>();
        myAbilitySystem = GetComponent<AbilitySystem>();
    }
    #region Action Classes
    private class Action
    {
        public Vector3 location;
        public GameObject target;
        public AbilitySystem.AbilityHotkey keypress;
        public Action()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if ("Unit" == hit.collider.tag)
                {
                    target = hit.collider.gameObject;
                }
            }
            location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetButtonDown("Q"))
            {
                keypress = AbilitySystem.AbilityHotkey.Q;
            }
        }
        public virtual void Execute(UnitBehaviourScript aUnit)
        {
            Debug.Log("This should never run");
        }
        public virtual void RemoveAction(UnitBehaviourScript aUnit)
        {
            aUnit.myNavMeshAgent.SetDestination(aUnit.transform.position);
            aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
        }
    }
    private class Movement : Action
    {
        public override void Execute(UnitBehaviourScript aUnit)
        {
            if (aUnit.transform.position.x == location.x &&//Have I reached my destination
                    aUnit.transform.position.z == location.z)
            {
                aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                return;
            }

            if (aUnit.myNavMeshAgent.destination.x != location.x &&//Has the destination not been set
                aUnit.myNavMeshAgent.destination.z != location.z)
            {
                aUnit.myNavMeshAgent.SetDestination(location);//set the destination
            }
        }
    }
    private class Attack : Action
    {
        public Attack() : base()
        {

        }
        public Attack(GameObject aObj)
        {
            //logic
        }
        public override void Execute(UnitBehaviourScript aUnit)
        {
            if (null == target)//Is target dead?
            {
                aUnit.myNavMeshAgent.SetDestination(aUnit.transform.position);
                aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                return;
            }
            if (Vector3.Distance(aUnit.gameObject.transform.position, target.transform.position) <
                    aUnit.myStatSystem.GetValue(StatSystem.StatType.AttackRange))//TODO: Make efficent
            {
                aUnit.myNavMeshAgent.SetDestination(aUnit.gameObject.transform.position);//Stop Moving
                if (aUnit.attackCD < Time.time)
                {
                    StatSystem.TakeDamage(target.GetComponent<StatSystem>(), aUnit.myStatSystem);//deal damage code
                    aUnit.attackCD += aUnit.myStatSystem.GetValue(StatSystem.StatType.AttackSpeed);//set like this to avoid framerate advantages
                    if (aUnit.attackCD < Time.time)//So they can't build up more than one attack as a result of the above code
                    {
                        aUnit.attackCD = Time.time;
                    }
                }
            }
            else
            {
                aUnit.myNavMeshAgent.SetDestination(target.transform.position);//move towards target
            }

        }
    }
    private class Ability : Action
    {
        public override void Execute(UnitBehaviourScript aUnit)
        {
            AbilitySystem myAbilitySystem = aUnit.gameObject.GetComponent<AbilitySystem>();
            if (null == myAbilitySystem)
            {
                RemoveAction(aUnit);
                return;
            }
            if(0 >= myAbilitySystem.Abilities.Count)
            {
                RemoveAction(aUnit);
                return;
            }
            if (myAbilitySystem.Execute(keypress))
            {
                aUnit.myNavMeshAgent.SetDestination(aUnit.transform.position);
                aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                return;
            }
        }
        public override void RemoveAction(UnitBehaviourScript aUnit)
        {
            AbilitySystem myAbilitySystem = aUnit.gameObject.GetComponent<AbilitySystem>();
            if(null != myAbilitySystem)
            {
                myAbilitySystem.Refund();
            }
            base.RemoveAction(aUnit);
        }
    }
    private class AttackMove : Action
    {
        public override void Execute(UnitBehaviourScript aUnit)
        {
            if (aUnit.transform.position.x == location.x &&//Have I reached my destination
                    aUnit.transform.position.z == location.z)
            {
                aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                return;
            }

            if (aUnit.myNavMeshAgent.destination.x != location.x &&//Has the destination not been set
                aUnit.myNavMeshAgent.destination.z != location.z)
            {
                aUnit.myNavMeshAgent.SetDestination(location);//set the destination
            }

            foreach (var unit in GameObject.FindGameObjectsWithTag("Unit"))//TODO: Make efficent
            {
                if (!unit.GetComponent<NetworkSync>().owned &&
                    Vector3.Distance(aUnit.gameObject.transform.position, unit.transform.position) <
                    aUnit.myStatSystem.GetValue(StatSystem.StatType.AggroRange))//TODO: Make efficent
                {
                    //This is wordy because I was having trouble overloading the inherited constructor
                    //TODO: Bug rework inherited constructor stuff
                    AttackMove finishAttackMove = new AttackMove();
                    finishAttackMove.location = this.location;
                    Attack attackTarget = new Attack();
                    attackTarget.target = unit;
                    aUnit.myQueue.AddFirst(finishAttackMove);
                    aUnit.myQueue.AddFirst(attackTarget);
                    aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                    return;
                }
            }
        }
    }
    private class Patrol : Action
    {
        //TODO, actually implement patrol functionality
        public override void Execute(UnitBehaviourScript aUnit)
        {
            if (aUnit.transform.position.x == location.x &&//Have I reached my destination
                    aUnit.transform.position.z == location.z)
            {
                aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                return;
            }

            if (aUnit.myNavMeshAgent.destination.x != location.x &&//Has the destination not been set
                aUnit.myNavMeshAgent.destination.z != location.z)
            {
                aUnit.myNavMeshAgent.SetDestination(location);//set the destination
            }

            foreach (var unit in GameObject.FindGameObjectsWithTag("Unit"))//TODO: Make efficent
            {
                if (!unit.GetComponent<NetworkSync>().owned &&
                    Vector3.Distance(aUnit.gameObject.transform.position, unit.transform.position) <
                    aUnit.myStatSystem.GetValue(StatSystem.StatType.AggroRange))//TODO: Make efficent
                {
                    //This is wordy because I was having trouble overloading the inherited constructor
                    //TODO: Bug rework inherited constructor stuff
                    AttackMove finishAttackMove = new AttackMove();
                    finishAttackMove.location = this.location;
                    Attack attackTarget = new Attack();
                    attackTarget.target = unit;
                    aUnit.myQueue.AddFirst(finishAttackMove);
                    aUnit.myQueue.AddFirst(attackTarget);
                    aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
                    return;
                }
            }
        }
    }
    private class Stop : Action
    {
        public override void Execute(UnitBehaviourScript aUnit)
        {
            aUnit.myNavMeshAgent.SetDestination(aUnit.gameObject.transform.position);//clear the destinataion
            aUnit.currentAction = null;//set the pointer to this object in UnitBehaviourScript to be null
            return;
        }
    }
    #endregion
    public void QueueAction(ActionType aType, bool aShift)
    {
        myNavMeshAgent.SetDestination(transform.position);
        Action temp = null;
        switch (aType)
        {
            case ActionType.Movement:
                temp = new Movement();
                break;
            case ActionType.Attack:
                temp = new Attack();
                break;
            case ActionType.AttackMove:
                temp = new AttackMove();
                break;
            case ActionType.Stop:
                temp = new Stop();
                break;
            case ActionType.Ability:
                temp = new Ability();
                break;
            case ActionType.Patrol:
                temp = new Patrol();
                break;
        }
        if (aShift)
        {
            myQueue.AddLast(temp);
        }
        else
        {
            myQueue.Clear();
            if (null != currentAction)
            {
                currentAction.RemoveAction(this);
            }
            currentAction = temp;
        }
    }
    private void Update()
    {
        if (!GetComponent<NetworkSync>().owned)
        {
            return;
        }
        if (null == currentAction)
        {
            if (myQueue.Count <= 0)
            {
                return;
            }
            currentAction = myQueue.First.Value;
            myQueue.Remove(myQueue.First.Value);
        }
        currentAction.Execute(this);
    }
}
