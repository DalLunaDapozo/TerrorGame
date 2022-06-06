using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using FMODUnity;
using Pathfinding;

public class MonsterIAOld : MonoBehaviour
{
    public enum Status {alert, patrol, gotcha}
   
    //COMPONENTS
    private PlayerMovement playerMovement;
    private PlayerLocation playerLocation;
    private GameController gameController;
    private Lighter lighter;
    private Rigidbody2D rb;

    private AIPath aiPath;
    private Seeker seeker;
    private Path path;
    public int currentWayPoint;
    public bool reachedEndOfPath;
    public float speed = 200f;
    public float newtWaypointDistance = 3f;


    //EVENTS
    public event System.EventHandler OnPlayerCatched;
    
    //TRANSFORMS: 
    private Transform player;
    private Transform CurrentRoomTransform;

    //PUBLIC VARIABLES
    [Header("Status")]
    [SerializeField] private Status status = Status.patrol;
    [SerializeField] public CurrentRoom currentRoom = CurrentRoom.MainRoom;
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
    
    [SerializeField] private float alertMovementSpeed;
    [SerializeField] private float moveSpeedRateWhileInAlert;
    [SerializeField] private float timeBeforeStartFollowingSound;
    private float timeBeforeStartFollowingSoundCurrent;

    [Header("How much sounds alert the monster")]
    [SerializeField] private float stepAlertAmount;
    [SerializeField] private float lighterAlertAmount;


    [SerializeField] EventReference alertGrowl;
    public FMOD.Studio.EventInstance alertGrowlInstance;
    
    private float alertMinMoveSpeed;

    public bool chasingLastSound;

    public Transform lastSpotHeard;

    private bool playerCatched = false;

    //private bool hasGrowl = false;

    private void Awake()
    {
        player = GameObject.Find("Player").gameObject.transform;
        lighter = GameObject.Find("Lighter").GetComponent<Lighter>();

        rb = GetComponent<Rigidbody2D>();

        aiPath = GetComponent<AIPath>();
     
        seeker = GetComponent<Seeker>();

        playerMovement = player.GetComponent<PlayerMovement>();
        playerLocation = player.GetComponent<PlayerLocation>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        playerMovement.OnStepSound += StepSoundEvent; 
        lighter.OnLighterSound += LighterSoundEvent;
    }

    private void Start()
    {

        alertGrowlInstance = FMODUnity.RuntimeManager.CreateInstance(alertGrowl);
        alertGrowlInstance.start();
        GetFloorSize();
        RestartTimerBeforeChangingRoom();
        moveSpot = GetNewPosition();
        alertMinMoveSpeed = alertMovementSpeed;

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        seeker.StartPath(transform.position, moveSpot, OnPathComplete);
    }

    private void FixedUpdate()
    {
        

        if (path == null)
            return;

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;


        }
        else
            reachedEndOfPath = false;

        Vector2 direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWayPoint]);

        if (distance < newtWaypointDistance)
        {
            currentWayPoint++;
        }

        if (rb.velocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f,1f,1);
        }
        else if (rb.velocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1);
        }

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
            case CurrentRoom.MainRoom:
                AuxiliarMethod_CurrentRoomTransform("MainRoom");
                break;
            case CurrentRoom.BedRoom:
                AuxiliarMethod_CurrentRoomTransform("BedRoom");
                break;
            case CurrentRoom.BathRoom:
                AuxiliarMethod_CurrentRoomTransform("BathRoom");
                break;
            case CurrentRoom.Storage:
                AuxiliarMethod_CurrentRoomTransform("Storage");
                break;
            case CurrentRoom.Kitchen:
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

           
        }
        else
        {
            moveSpot = randomRoomToGo;
            
        }

        
    }
    private void Move(float moveSpeed)
    {
       
        transform.position = Vector2.MoveTowards(transform.position, moveSpot, moveSpeed * Time.deltaTime);
    }
    
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
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

    private void StepSoundEvent(object sender, System.EventArgs e)
    {
        if (status == Status.patrol)
            alertCurrent += stepAlertAmount;
        else if (status == Status.alert)
            alertCurrent += stepAlertAmount * 2f;
    }

    private void LighterSoundEvent(object sender, System.EventArgs e)
    {
        if (status == Status.patrol)
            alertCurrent += lighterAlertAmount;
        else if (status == Status.alert)
            alertCurrent += lighterAlertAmount * 2f;
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
            FMODUnity.RuntimeManager.PlayOneShot(alertGrowl);
            moveSpot = (circle.transform.position);
            //hasGrowl = true;
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
            if (timeBeforeStartFollowingSoundCurrent < 0f)
                chasingLastSound = true;

            else
                timeBeforeStartFollowingSoundCurrent -= Time.deltaTime;

        }          
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

                //hasGrowl = false;
                timeBeforeStartFollowingSoundCurrent = timeBeforeStartFollowingSound;
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
