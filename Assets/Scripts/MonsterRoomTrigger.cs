using Cinemachine;
using UnityEngine;

public class MonsterRoomTrigger : MonoBehaviour
{
    
    public CurrentRoom roomName;
    private MonsterAI monster;
    private void Awake()
    {
        try { monster = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("hola"); }

    }
    private void Start()
    { 
        monster = GameObject.Find("Monster").GetComponent<MonsterAI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.gameObject.tag == "Monster")
        {
            Debug.Log("Hola :)");
            ChangeMonsterRoom();
        }
    }

    private void ChangeMonsterRoom()
    {
        monster.currentRoom = roomName;
        monster.GetFloorSize();
        monster.goingToAnotherRoom = false;
        monster.oneRoomPicked = false;
        monster.RestartTimerBeforeChangingRoom();
    }

   
}
