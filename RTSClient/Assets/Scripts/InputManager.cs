using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [HideInInspector] private Vector3 camStartPos;
    [HideInInspector] private Vector3 camDragDif;
    [HideInInspector] private Vector3 camOrigin;
    [HideInInspector] private Vector3 selectStartPos;
    [HideInInspector] private Vector3 selectEndPos;
    [HideInInspector] private Vector3 selectCenter;
    [HideInInspector] private List<GameObject> selectedUnits = new List<GameObject>();
    [HideInInspector] private List<GameObject> selectionCleanup = new List<GameObject>();
    private void Update()
    {
        KeyPress();
        CameraInput();
    }
    private void KeyPress()
    {
        LeftClick();
        RightClick();
        if (Input.GetButtonDown("Q"))
        {
            foreach (var unit in selectedUnits)
            {
                unit.GetComponent<UnitBehaviourScript>().QueueAction(
                    UnitBehaviourScript.ActionType.Ability,
                    Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
            }
        }
        if (Input.GetButton("Stop"))
        {
            foreach (var unit in selectedUnits)
            {
                unit.GetComponent<UnitBehaviourScript>().QueueAction(
                    UnitBehaviourScript.ActionType.Stop,
                    Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
            }
        }
        if (Input.GetButton("Attack"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if ("Unit" == hit.collider.tag)
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.GetComponent<UnitBehaviourScript>().QueueAction(
                            UnitBehaviourScript.ActionType.Attack,
                            Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
                    }
                }
                else
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.GetComponent<UnitBehaviourScript>().QueueAction(
                        UnitBehaviourScript.ActionType.AttackMove,
                        Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
                    }
                }
            }
        }
        if (Input.GetButton("Patrol"))
        {
            foreach (var unit in selectedUnits)
            {
                unit.GetComponent<UnitBehaviourScript>().QueueAction(
                UnitBehaviourScript.ActionType.Patrol,
                Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
            }
        }
    }
    private void LeftClick()
    {
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            References.UserInterface.selectionBox.gameObject.SetActive(true);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                selectStartPos = hit.point;
            }
            //selectStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectStartPos.y = 0;
        }
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                selectEndPos = hit.point;
            }
            //selectEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectEndPos.y = 0;

            #region https://www.habrador.com/tutorials/select-units-within-rectangle/
            Vector3 boxStart = Camera.main.WorldToScreenPoint(selectStartPos);
            Vector3 boxEnd = Input.mousePosition;
            Vector3 boxCenter = (boxStart + boxEnd) / 2;
            float boxWidth = Mathf.Abs(boxStart.x - boxEnd.x);
            float boxHeight = Mathf.Abs(boxStart.y - boxEnd.y); ;
            References.UserInterface.selectionBox.position = boxCenter;
            References.UserInterface.selectionBox.sizeDelta = new Vector2(boxWidth, boxHeight);
            #endregion
        }
        if (Input.GetMouseButtonUp(0))
        {
            int newSelCount = 0;
            References.UserInterface.selectionBox.gameObject.SetActive(false);


            if (!Input.GetKey(KeyCode.LeftShift))
            {
                selectedUnits.Clear();
                foreach (var item in selectionCleanup)
                {
                    Destroy(item);
                }
                selectionCleanup.Clear();
            }
            foreach (var unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                if (selectedUnits.Contains(unit) || !unit.GetComponent<NetworkSync>().owned)
                {
                    continue;
                }

                //determine if this unit is within the selection box on release
                selectCenter = (selectStartPos + selectEndPos) / 2;
                float width = Mathf.Abs(selectStartPos.x - selectEndPos.x);
                float height = Mathf.Abs(selectStartPos.z - selectEndPos.z);
                if (Mathf.Abs(unit.transform.position.x - selectCenter.x) < width / 2 &&
                    Mathf.Abs(unit.transform.position.z - selectCenter.z) < height / 2)
                {
                    selectedUnits.Add(unit);
                    GameObject temp = Instantiate(References.SelectionIndicator, unit.transform.position, Quaternion.identity, unit.transform);
                    Vector3 pos = temp.transform.position;
                    pos.y = -1;
                    temp.transform.position = pos;
                    selectionCleanup.Add(temp);
                    newSelCount++;
                }
            }
            if (0 == newSelCount)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if ("Unit" == hit.collider.tag)
                    {
                        if (!selectedUnits.Contains(hit.collider.gameObject) && hit.collider.gameObject.GetComponent<NetworkSync>().owned)
                        {
                            selectedUnits.Add(hit.collider.gameObject);
                            GameObject temp = Instantiate(References.SelectionIndicator, hit.transform.position, Quaternion.identity, hit.transform);
                            Vector3 pos = temp.transform.position;
                            pos.y = -1;
                            temp.transform.position = pos;
                            selectionCleanup.Add(temp);
                        }
                    }
                }
            }
        }
    }
    private void RightClick()
    {
        if (!Input.GetMouseButtonDown(1))//right click
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            if ("Unit" == hit.collider.tag)
            {
                if (!hit.collider.gameObject.GetComponent<NetworkSync>().owned)
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.GetComponent<UnitBehaviourScript>().QueueAction(
                            UnitBehaviourScript.ActionType.Attack,
                            Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
                    }
                }
            }
            else
            {
                foreach (var unit in selectedUnits)
                {
                    unit.GetComponent<UnitBehaviourScript>().QueueAction(
                        UnitBehaviourScript.ActionType.Movement,
                        Input.GetKey(KeyCode.LeftShift));//if shift is held the action is queued
                }
            }
        }
    }
    private void CameraInput()
    {
        //camera grip
        if (Input.GetMouseButtonDown(2))
        {
            camStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2))
        {
            camDragDif = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            Camera.main.transform.position = camStartPos - camDragDif;
        }

        //Zoom logic
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom < 0f)
        {
            Camera.main.orthographicSize *= 1.1f;
        }
        if (zoom > 0f)
        {
            Camera.main.orthographicSize *= 0.9f;
        }
    }//end camera input
}//end class
