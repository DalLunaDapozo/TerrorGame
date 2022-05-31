using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public enum CurrentRoom { mainroom, bedroom, bathroom, storage, kitchen }

public class MonsterIA : MonoBehaviour
{
    public enum Status {alert, patrol, gotcha}
   
    //COMPONENTS
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerLocation playerLocation;
    private GameController gameController;
    private BoxCollider2D boxCollider;

    //EVENTS
    public event System.EventHandler OnPlayerCatched;
    
    //TRANSFORMS: 
    private Transform player;
    private Transform CurrentRoomTransform;

    //PUBLIC VARIABLES
    [Header("Status")]
    [SerializeField] private Status status = Status.patrol;
    [SerializeField] public CurrentRoom currentRoom = CurrentRoom.mainroom;
    [SerializeField] private CurrentRoom playerCurrentRoom;
    
    [Header("Patrol Status")]
    [SerializeField] private float patrolMovementSpeed;
    [SerializeField] private float movingToAnotherRoomMovementSpeed;
    [SerializeField] private float timeBetweenPoints;
    [SerializeField] private float timeBeforeChangingRooms;

    [SerializeField] public List<Transform> pointsToGo;

    public bool goingToAnotherRoom;

    public float currentTimeBeforeChangingRooms;

    private float currentWaitTimeBetweenPoints;
    public bool oneRoomPicked = false;
    private Vector2 randomRoomToGo;
    private Vector2 moveSpot;
    private float minX, maxX, minY, maxY;
    
    [Header("Alert Status")]
    [SerializeField] private float gotchaDistance;
    [SerializeField] private float alertCurrent;
    [SerializeField] private float alertMin;
    [SerializeField] private float decreaseTime;
    [SerializeField] private float stepAlertAmount;
    [SerializeField] private float alertMovementSpeed;
    [SerializeField] private float moveSpeedRateWhileInAlert;
    [SerializeField] private float timeBeforeStartFollowingSound;

    private float alertMinMoveSpeed;

    public bool chasingLastSound;

    public Transform lastSpotHeard;

    private bool playerCatched = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        player = GameObject.Find("Player").gameObject.transform;
        
        playerMovement = player.GetComponent<PlayerMovement>();
        playerLocation = player.GetComponent<PlayerLocation>();
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();

        playerMovement.OnStepSound += AlertListenEvents;
    }

    private void Start()
    {
        GetFloorSize();
        RestartTimerBeforeChangingRoom();
        moveSpot = GetNewPosition();
        alertMinMoveSpeed = alertMovementSpeed;
    }

    private void Update()
    {
        PlayerCurrentRoom();
        StatusDependingOnPlayerDistance();
        StatusBehaviour();
        AlertFieldOverTime();
        TimerBeforeGoingToAnotherRoom();
        
    }
 
    //PATROL STATUS
    public void GetFloorSize()
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

        minX = (floorSize.bounds.center.x - (floorSize.bounds.extents.x));
        maxX = (floorSize.bounds.center.x + floorSize.bounds.extents.x);
        minY = (floorSize.bounds.center.y - floorSize.bounds.extents.y);
        maxY = (floorSize.bounds.center.y + floorSize.bounds.extents.y);
    }
    private Vector2 GetNewPosition()
    {
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY);
        Vector3 newPosition = new Vector3(randomX, randomY, 0f);
        
        return newPosition;
    }
    private void PatrolMovement()
    {
        if (!goingToAnotherRoom)
        {
           
            if (Vector2.Distance(transform.position, moveSpot) <= 0.2f)
            {
                if (currentWaitTimeBetweenPoints <= 0)
                {
                    moveSpot = GetNewPosition();
                    currentWaitTimeBetweenPoints = timeBetweenPoints;
                }
                else
                    currentWaitTimeBetweenPoints -= Time.deltaTime;
            }

            Move(patrolMovementSpeed);
        }
        else
        {
            moveSpot = randomRoomToGo;
            Move(movingToAnotherRoomMovementSpeed);
        }

        
    }
    private void Move(float moveSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpot, moveSpeed * Time.deltaTime);
    }
    private void TimerBeforeGoingToAnotherRoom()
    {
        if (currentTimeBeforeChangingRooms <= 0 && !oneRoomPicked)
        {
            goingToAnotherRoom = true;
            randomRoomToGo = ChooseRandomRoomToGo();
            oneRoomPicked = true;
        }
            

        else
            currentTimeBeforeChangingRooms -= Time.deltaTime;
    }
    private Vector2 ChooseRandomRoomToGo()
    {
        int index = UnityEngine.Random.Range(0, pointsToGo.Count);
        return pointsToGo[index].transform.position;
    }
    private void AuxiliarMethod_CurrentRoomTransform(string roomName)
    {
        CurrentRoomTransform = GameObject.Find(roomName).gameObject.transform;
    }
    public void AddPointToList(Transform point)
    {
        pointsToGo.Add(point);
    }
    public void RemovePointFromList(Transform point)
    {
        pointsToGo.Remove(point);
    }
    public void RestartTimerBeforeChangingRoom()
    {
        currentTimeBeforeChangingRooms = timeBeforeChangingRooms;
    }
    
    //ALERT STATUS

    private void AlertListenEvents(object sender, System.EventArgs e)
    {
        if (status == Status.patrol)
            alertCurrent += stepAlertAmount;
        else if(status == Status.alert)
            alertCurrent += stepAlertAmount * 2f;
    }
    private void AlertFieldOverTime()
    {
        if (alertCurrent > alertMin)
        {
            alertCurrent -= Time.deltaTime * decreaseTime;
        }
    }  
    private void AlertBehaviour()
    {
        Collider2D circle = Physics2D.OverlapCircle(transform.position, alertCurrent, 1 << 7);

        if (circle != null)
        {
            Debug.Log("Step Heard");
            moveSpot = (circle.transform.position);
           
        }
        IncreaseMovementSpeedWhenAlert();
        AlertChaseSequence();
    }

    private void IncreaseMovementSpeedWhenAlert()
    {
        if (alertMovementSpeed < 6f)
        {
            alertMovementSpeed += moveSpeedRateWhileInAlert;
        }
    }

    private void AlertChaseSequence()
    {
        if (!chasingLastSound)
        {
            if (timeBeforeStartFollowingSound < 0f)
                chasingLastSound = true;

            else
                timeBeforeStartFollowingSound -= Time.deltaTime;

        }
        else
            Move(alertMovementSpeed);    
    }

    //BEHAVIOUR

    private void StatusBehaviour()
    {
        switch (status)
        {
            case Status.alert:

                AlertBehaviour();

                break;
            
            case Status.patrol:

                alertMovementSpeed = alertMinMoveSpeed;
                chasingLastSound = false;
                PatrolMovement();

                break;

            case Status.gotcha:

                playerCatched = true;
                OnCatchingPlayer();
                
                break;
        }

    }
   
    //CHECK ON PLAYER
    private void PlayerCurrentRoom()
    {
        playerCurrentRoom = playerLocation.playerCurrentRoom;
    }
    private void StatusDependingOnPlayerDistance()
    {
        float distance = Vector3.Distance(this.transform.position, player.position);
        
        if (distance <= gotchaDistance)
        {
            status = Status.gotcha;
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
    private void OnCatchingPlayer()
    {
        if (playerCatched)
            OnPlayerCatched?.Invoke(this, EventArgs.Empty);
    }
    
    //DEBUG METHODS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertCurrent);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gotchaDistance);

        if(CurrentRoomTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(CurrentRoomTransform.position, CurrentRoomTransform.GetComponent<BoxCollider2D>().bounds.size);
        }
       
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, randomRoomToGo);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(moveSpot, 1f);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
            GetNewPosition();
    }
}
