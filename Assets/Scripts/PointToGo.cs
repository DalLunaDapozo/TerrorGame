using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToGo : MonoBehaviour
{
    public CurrentRoom roomIsOn;

    private MonsterIA monster;
    private float distance;

    [SerializeField] private GameObject child;

    public bool isActive;
    private bool added;

    private void Awake()
    {
        monster = GameObject.Find("Monster").GetComponent<MonsterIA>();
    }

    private void Update()
    {
        IsAvailable();
        SetActive(isActive);
        AddRemoveFromList();
        MonsterNearPoint();
    }

    private void SetActive(bool e)
    {
        child.SetActive(e);
    }

    private void AddRemoveFromList()
    {
        if (isActive && !added)
        {
            monster.AddPointToList(child.transform);
            added = true;
        }
        else if (!isActive & added)
        {
            monster.RemovePointFromList(child.transform);
            added = false;
        }    
           
    }

    private void MonsterNearPoint()
    {
        distance = Vector2.Distance(transform.position, monster.transform.position);

        if (distance <= 0.5f)
        {
            monster.currentRoom = roomIsOn;
            monster.GetFloorSize();
            monster.goingToAnotherRoom = false;
            monster.oneRoomPicked = false;
            monster.RestartTimer();
        }
    }

    private void IsAvailable()
    {
        if (monster.currentRoom == roomIsOn)
        {
            isActive = false;
            
        }
           
        else
        {
            isActive = true;
            
        }
            
    }

}
