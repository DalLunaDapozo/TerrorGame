using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using FMODUnity;
using Pathfinding;

//ENUMS
#region ENUMS
public enum CurrentRoom { MainRoom, BedRoom, BathRoom, Storage, Kitchen, SecondFloorMain, RitualRoom, OwnBedroom, Corridor, Library, Closet }
public enum Status { alert, patrol, gotcha }
#endregion

public class MonsterAI : MonoBehaviour 
{
    #region VARIABLES

    #region COMPONENTS

    //TO FIND

    private Transform player;
    private GameController gameController;
    private Lighter lighter;
    private MonsterAnimations monsterAnimations;

    #endregion

    #region PATHFINDING VARIABLES

    //PRIVATE

    private Seeker seeker;
    private AIPath aiPath;
    private Path path;
    
    private int currentWayPoint;
    
    private Vector2 moveSpot;

    //PUBLIC
    [Header("Pathfinding")]
    public float movementSpeed = 200f;
    public float maxSpeedCap = 1000f;
    public float nextWaypointDistance = 3f;
    
    [SerializeField] public List<Transform> pointsToGo;
    
    private float normalMovementSpeed;

    #endregion

    #region STATUS VARIABLES
    
    //STATUS AND LOCATION
    [Header("Status")]
    [SerializeField] public Status status = Status.patrol;
    [SerializeField] public CurrentRoom currentRoom = CurrentRoom.MainRoom;

    #endregion

    #region PATROL VARIABLES
    
    [Header("Patrol Variables")]
    
    [SerializeField] private float timeBetweenPoints;
    [SerializeField] private float timeBeforeChangingRooms;
   
    [SerializeField] private float movingToAnotherRoomMovementSpeed;

    private Transform CurrentRoomTransform;
    private float minX, maxX, minY, maxY;
    private float currentTimeBeforeChangingRooms;
    private float currentWaitTimeBetweenPoints;
   
    [HideInInspector] public bool goingToAnotherRoom = false;
    [HideInInspector] public bool oneRoomPicked = false;
    
    private Vector2 randomRoomToGo;
   
    #endregion

    #region ALERT VARIABLES

    [Header("Alert Status")]

    [SerializeField] private float alertCurrent;
    [SerializeField] private float decreaseTime;
    [SerializeField] private float gotchaDistance;
    [SerializeField] private float gotchaDistanceGrowRate;
    [SerializeField] private float alertMinRadius;
    [SerializeField] private float moveSpeedRateWhileInAlert;
    [SerializeField] private float timeBeforeStartFollowingSound;
    [SerializeField] private float inAlertZoneAmountMultiplier;

    [Header("How much sounds alert the monster")]
    [SerializeField] private float stepAlertAmount;
    [SerializeField] private float lighterAlertAmount;

    private float timeBeforeStartFollowingSoundCurrent;
    public bool chasingLastSound = false;
   
    public bool playerCatched = false;
    private bool isKilling = false;

    public bool hasGrowled;

    //EVENT
    
    public event System.EventHandler OnPlayerCatched;
    public event System.EventHandler StopPlayerMovement;

    #endregion

    #region SOUND

    [SerializeField] EventReference alertGrowl;
    public FMOD.Studio.EventInstance alertGrowlInstance;
    //private bool hasGrowl = false;

    #endregion
    
    #endregion

    #region UNITY_METHODS

    private void Awake()
    {
        //IN GAMEOBJECT

        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        monsterAnimations = GetComponent<MonsterAnimations>();

        //TO FIND !!! ( (0) )

        player = GameObject.Find("Player").GetComponent<Transform>();
        lighter = GameObject.Find("Lighter").GetComponent<Lighter>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        //FMOD INSTANCES
        
        //alertGrowlInstance = FMODUnity.RuntimeManager.CreateInstance(alertGrowl);
    }
    private void Start()
    {
        //DEFAULT MOVEVEMNT SPEED 
        
        normalMovementSpeed = movementSpeed;

        //START WITH RANDOM POS
        
        GetFloorSize();
        RestartTimerBeforeChangingRoom();
        moveSpot = GetNewPosition();
        
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    private void OnEnable()
    {
        //EVENTS

        player.GetComponent<PlayerMovement>().OnStepSound += StepSoundEvent;
        lighter.OnLighterSound += LighterSoundEvent;

        //START SOUNDS INSTANCE

        //alertGrowlInstance.start();
    }
    private void OnDisable()
    {
        //EVENTS

        lighter.OnLighterSound -= LighterSoundEvent;

        //START SOUNDS INSTANCE

        //alertGrowlInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void Update()
    {
        TimerBeforeGoingToAnotherRoom();
        StatusDependingOnPlayerDistance();
        BehaviourDependingOnStatus();
        DecreaseAlertFieldOverTime();
      
        UpdateAIPath();
    }

    #endregion
    
    #region PATHFIND METHODS
  
    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(transform.position, moveSpot, OnPathComplete);
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
    private void UpdateAIPath()
    {
        aiPath.maxSpeed = movementSpeed;
    }

    #endregion

    #region PATROL METHODS

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
            case CurrentRoom.Library:
                AuxiliarMethod_CurrentRoomTransform("Library");
                break;
        }


        BoxCollider2D floorSize = CurrentRoomTransform.GetComponent<BoxCollider2D>();

        minX = (floorSize.bounds.center.x - (floorSize.bounds.extents.x));
        maxX = (floorSize.bounds.center.x + floorSize.bounds.extents.x);
        minY = (floorSize.bounds.center.y - floorSize.bounds.extents.y);
        maxY = (floorSize.bounds.center.y + floorSize.bounds.extents.y);
    }
    private void AuxiliarMethod_CurrentRoomTransform(string roomName)
    {
        CurrentRoomTransform = GameObject.Find(roomName).gameObject.transform;
    }
    private Vector2 GetNewPosition()
    {
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY);
        Vector3 newPosition = new Vector3(randomX, randomY, 0f);

        return newPosition;
    }
    private void Main_PatrolBehaviour()
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
            movementSpeed = movingToAnotherRoomMovementSpeed;
            moveSpot = randomRoomToGo;
        }
           
    }
    public void RestartTimerBeforeChangingRoom()
    {
        currentTimeBeforeChangingRooms = timeBeforeChangingRooms;
    }
    public void AddPointToList(Transform point)
    {
        pointsToGo.Add(point);
    }
    public void RemovePointFromList(Transform point)
    {
        pointsToGo.Remove(point);
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
   
    #endregion

    #region ON ALERT METHODS

    private void Main_AlertBehaviour()
    {
        Collider2D circle = Physics2D.OverlapCircle(transform.position, alertCurrent, 1 << 7);

        if (circle != null)
        {
            moveSpot = (circle.transform.position);
            chasingLastSound = true;
        }

        
        if (Vector2.Distance(moveSpot, transform.position) < 0.2f)
        {
            chasingLastSound = false;
        }
        

        if (!hasGrowled)
            StartCoroutine("AlertAnimation");

        IncreaseMovementSpeedWhenAlert();
    }
    private void DecreaseAlertFieldOverTime()
    {
        if (alertCurrent > alertMinRadius)
            alertCurrent -= Time.deltaTime * decreaseTime;

        if (gotchaDistance >= 2.5f)
            gotchaDistance -= Time.deltaTime * decreaseTime;
    }
    private void AlertVariablesGoDefault()
    {
       
        chasingLastSound = false;
        timeBeforeStartFollowingSoundCurrent = timeBeforeStartFollowingSound;

        if (!goingToAnotherRoom)
            movementSpeed = normalMovementSpeed;
    }
    private void IncreaseMovementSpeedWhenAlert()
    {
        if (movementSpeed < maxSpeedCap)
        {
            movementSpeed += moveSpeedRateWhileInAlert;
            gotchaDistance += gotchaDistanceGrowRate;
        }
    }
   
    private void OnCatchingPlayer()
    {
        OnPlayerCatched?.Invoke(this, System.EventArgs.Empty);
      
        chasingLastSound = false;
    }

    IEnumerator AlertAnimation()
    {
        hasGrowled = true;
        monsterAnimations.anim.SetBool("Alert", true);

        aiPath.canMove = false;

        yield return new WaitForSeconds(monsterAnimations.anim.GetCurrentAnimatorStateInfo(0).length);

        aiPath.canMove = true;

        monsterAnimations.anim.SetBool("Alert", false);
        chasingLastSound = true;
    }
    IEnumerator KillingAnimation()
    {
       
        isKilling = true;
        
        StopPlayerMovement?.Invoke(this, System.EventArgs.Empty);
        
        playerCatched = true;
       
        monsterAnimations.anim.SetBool("isKilling", isKilling);
        player.GetComponent<PlayerMovement>().SetActivateSprite(false);
      

        yield return new WaitForSeconds(monsterAnimations.anim.GetCurrentAnimatorStateInfo(0).length + 2);

        OnCatchingPlayer();

        yield return new WaitForSeconds(3);
        
        isKilling = false;
        monsterAnimations.anim.SetBool("isKilling", isKilling);
        
        aiPath.canMove = true;

        aiPath.canSearch = true;
        aiPath.isStopped = false;

        alertCurrent = alertMinRadius;
        playerCatched = false;
    }
    #endregion

    #region BEHAVIOUR METHODS

    private void BehaviourDependingOnStatus()
    {
        switch (status)
        {
            case Status.alert:

                if (!isKilling)
                    Main_AlertBehaviour();
               
                break;

            case Status.patrol:

                if (!isKilling)
                {
                    hasGrowled = false;
                    AlertVariablesGoDefault();
                    Main_PatrolBehaviour();
                }

                break;

            case Status.gotcha:

                aiPath.canMove = false;
                aiPath.canSearch = false;
                aiPath.isStopped = true;

                if (!playerCatched)
                StartCoroutine("KillingAnimation");
                
                break;
        }

    }
    private void StatusDependingOnPlayerDistance()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);

        if (distance <= gotchaDistance)
        {
            status = Status.gotcha;
        }
        else if (distance <= alertCurrent)
        {
            status = Status.alert;
        }
        else if (distance > alertCurrent && !chasingLastSound)
        {
            status = Status.patrol;
        }

    }



    #endregion

    #region EVENT LISTENERS

    private void StepSoundEvent(object sender, System.EventArgs e)
    {
        SoundEventAmount(stepAlertAmount);
    }
   
    private void LighterSoundEvent(object sender, System.EventArgs e)
    {
        SoundEventAmount(lighterAlertAmount);
    }

    //AUXILIAR VARIABLES
    private void SoundEventAmount(float amount)
    {
        if (!player.GetComponent<PlayerLocation>().isSecondFloor && player.GetComponent<PlayerLocation>().playerCurrentRoom != CurrentRoom.Closet)
        {
            if (status == Status.patrol)
                alertCurrent += amount;
            else if (status == Status.alert)
                alertCurrent += amount * inAlertZoneAmountMultiplier;
        }
    }
    
    #endregion

    #region GIZMOS

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertCurrent);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gotchaDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(moveSpot, 1f);

    }
   
    #endregion


    public void ScanPathfind()
    {

    }
}
