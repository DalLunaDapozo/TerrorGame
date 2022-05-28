using UnityEngine;
using System.Collections.Generic;

public enum CurrentRoom { mainroom, bedroom, bathroom, storage, kitchen }

public class MonsterIA : MonoBehaviour
{

    public enum Status {alert, patrol, chase }
   

    private Rigidbody2D rb;
    private Transform player;
    private Transform CurrentRoomTransform;
    private Vector2 movementSpeed;

    private PlayerMovement playerMovement;
    private PlayerLocation playerLocation;

    [SerializeField] private float distanceUntilStop; 
    [SerializeField] private float chaseField;
    [SerializeField] private float alertCurrent;
    [SerializeField] private float alertMin;
    [SerializeField] private float decreaseTime;
    [SerializeField] private float stepAlertAmount;

    [SerializeField] private Vector2 directionToGo;

    [SerializeField] private Status status = Status.patrol;
    [SerializeField] private CurrentRoom currentRoom = CurrentRoom.mainroom;
    [SerializeField] private Vector2 playerPos;

 

    [SerializeField] private Vector2 patrolSpeed;
    [SerializeField] private Vector2 alertSpeed;
    [SerializeField] private Vector2 chaseSpeed;

    [SerializeField] private Vector2 patrolRange;

    [SerializeField] private GameObject pointToGo;
    [SerializeField] private GameObject patrolBox;

    public Vector2 moveSpot;

    public float waitTime = 3f;
    public float currentWaitTime;

    float minX, maxX, minY, maxY;

    private bool goingToPoint;
  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").gameObject.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
       

        playerMovement.OnStepSound += AlertListenEvents;
    }

    private void Start()
    {
        GetFloorSize();
        moveSpot = GetNewPosition();
    }

    

    private void Update()
    {
        ChangeMovementSpeed();
        StatusDependingOnPlayerDistance();
        PlayerPos();
        StatusBehavoiur();
        AlertFieldOverTime();
    }

    private void GetFloorSize()
    {
        switch (currentRoom)
        {
            case CurrentRoom.mainroom:
                AuxiliarMethod_CurrentRoomTransform("MainRoom");
                break;
            case CurrentRoom.bedroom:
                AuxiliarMethod_CurrentRoomTransform("BedRoom");
                break;
            case CurrentRoom.bathroom:
                AuxiliarMethod_CurrentRoomTransform("BathRoom");
                break;
            case CurrentRoom.storage:
                AuxiliarMethod_CurrentRoomTransform("Storage");
                break;
            case CurrentRoom.kitchen:
                AuxiliarMethod_CurrentRoomTransform("Kitchen");
                break;
        }


        BoxCollider2D floorSize = CurrentRoomTransform.GetComponent<BoxCollider2D>();

        minX = (floorSize.bounds.center.x - floorSize.bounds.extents.x);
        maxX = (floorSize.bounds.center.x + floorSize.bounds.extents.x); 
        minY = (floorSize.bounds.center.y - floorSize.bounds.extents.y); 
        maxY = (floorSize.bounds.center.y + floorSize.bounds.extents.y); 
    }

    private void AuxiliarMethod_CurrentRoomTransform(string roomName)
    {
        CurrentRoomTransform = GameObject.Find(roomName).gameObject.transform;
    }

    private void TimeThatStaysInRoom()
    {

    }

    private void ChooseRandomRoomToGo()
    {

    }

    private Vector2 GetNewPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 newPosition = new Vector3(randomX, randomY, 0f);
        
        return newPosition;
    }

    private void PatrolMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position,moveSpot, movementSpeed.x * Time.deltaTime);

        if (Vector2.Distance(transform.position, moveSpot) <= 0.2f)
        {
            if (currentWaitTime <= 0)
            {
                moveSpot = GetNewPosition();
                currentWaitTime = waitTime;
            }
            else
                currentWaitTime -= Time.deltaTime;
        }
    
    }

    private void TimeThatStaysInAlertMode()
    {

    }
    
    private void AlertListenEvents(object sender, System.EventArgs e)
    {
        alertCurrent += stepAlertAmount;
    }

    private void AlertFieldOverTime()
    {
        if (alertCurrent > alertMin)
        {
            alertCurrent -= Time.deltaTime * decreaseTime;
        }
    }

    private void StatusBehavoiur()
    {
        switch (status)
        {
            case Status.alert:
                
                

                break;
            
            case Status.patrol:

                PatrolMovement();

                break;
            
            case Status.chase:

            GoToDirection(player.position);
           
                break;
        }

    }
    private void StatusDependingOnPlayerDistance()
    {
        float distance = Vector3.Distance(this.transform.position, player.position);
        
        if (distance <= chaseField)
        {
            status = Status.chase;
        }
        else if (distance <= alertCurrent)
        {
            status = Status.alert;
        }
        else if (distance > alertCurrent)
        {
            status = Status.patrol;
        }
    }
    private void PlayerPos()
    {
        playerPos = player.transform.position;
    }
    private void GoToDirection(Vector3 pointToGo)
    {
        Vector2 direction = pointToGo - transform.position;
        Vector2 newVector = direction.normalized * movementSpeed;

        rb.velocity = newVector;
    }
    private void ChangeMovementSpeed()
    {
        switch (status)
        {
            case Status.alert:
                movementSpeed = alertSpeed;
                break;
            case Status.patrol:
                movementSpeed = patrolSpeed;
                break;
            case Status.chase:
                movementSpeed = chaseSpeed;
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.zero, patrolRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertCurrent);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseField);
    }
}
