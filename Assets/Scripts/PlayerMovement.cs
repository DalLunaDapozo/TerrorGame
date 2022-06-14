using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMOD;
using FMODUnity;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputManager inputManager;
    private Animator anim;
    private MonsterAI monsterIA;
    private Lighter lighter;
    private PlayerLocation location;

    [SerializeField] private GameObject soundWave;
    public event System.EventHandler OnStepSound;
    [SerializeField] public Transform lastStepSound;


    [SerializeField] EventReference stepsound;
    public FMOD.Studio.EventInstance playerState;
    [SerializeField] EventReference stepsoundOnCarpet;
    public FMOD.Studio.EventInstance stepSoundOnCarpetInstance;

    private Vector2 movementSpeed;
    private Vector2 moveVector;

    [SerializeField] private Vector2 movementSpeedLighterOn;
    [SerializeField] private Vector2 movementSpeedLighterOff;
    [SerializeField] private Vector2 movementSpeedRunning;
    [SerializeField] private Vector2 noLighterMovementSpeed;

    private bool doingSomethingImportant;
    private bool overCarpet;
    public bool noLighterScript;

    public bool isNearCandle;
    public bool candleAnimation;
    public bool lighterIsOn;

    public bool inSecondFloor = true;

    //RUNNING

    public bool IsRunning = false;
    private float animationSpeed;
    private float timeBeforeStartRunning = 2f;
    private float currentTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        location = GetComponent<PlayerLocation>();

        inputManager = new InputManager();

        if(!noLighterScript)
        {
            try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
            catch { UnityEngine.Debug.Log("hola"); }
            
            lighter = GameObject.Find("Lighter").GetComponent<Lighter>();
            anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerLighter");
        }
        else
            anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerNoLighter");

        if (monsterIA == null)
            return;
    }
 
    private void OnEnable()
    {
        inputManager.Player.Enable();
        if (monsterIA != null)
            monsterIA.OnPlayerCatched += StopMovement;
    }
    private void OnDisable()
    {
        inputManager.Player.Disable();
        if (monsterIA != null)
            monsterIA.OnPlayerCatched -= StopMovement;
    }
    
    private void Start()
    {
        playerState = FMODUnity.RuntimeManager.CreateInstance(stepsound);
        playerState.start();

        stepSoundOnCarpetInstance = FMODUnity.RuntimeManager.CreateInstance(stepsoundOnCarpet);
        stepSoundOnCarpetInstance.start();
        inSecondFloor = true;
        currentTime = timeBeforeStartRunning;
    }

    private void Update()
    {
        if (candleAnimation)
            LightingAnimation();
        else
        {
            ReadMovementValue();
            FlipSprite();
        }
      
        if (noLighterScript)
        {
            NoLighterWalkAnimation();
            movementSpeed = noLighterMovementSpeed;
        }
        else
        {
            lighterIsOn = lighter.lighterIsOn;           
            LighterWalkAnimation();
            ChangeMovementSpeed();
        }
    }

    private void FixedUpdate()
    {
        if (!doingSomethingImportant)
            rb.velocity = new Vector2(movementSpeed.x * moveVector.x, movementSpeed.y * moveVector.y) * Time.deltaTime;
        else
            rb.velocity = Vector2.zero;
    }


    private void ChangeMovementSpeed()
    {
        if (lighter.lighterIsOn)
        {
            if(IsRunning && inSecondFloor)
            {
                animationSpeed = 1.6f;
                movementSpeed = movementSpeedRunning;
            }  
            else
            {
                movementSpeed = movementSpeedLighterOn;
                animationSpeed = 1f;
            }
        }   
        else
        {
            movementSpeed = movementSpeedLighterOff;
            animationSpeed = 1f;
        }
            
    }

    private void ReadMovementValue()
    {
        moveVector = inputManager.Player.Movement.ReadValue<Vector2>();
    }
    private void NoLighterWalkAnimation()
    {
        if (moveVector != Vector2.zero)
            anim.SetBool("Walk", true);
        else
            anim.SetBool("Walk", false);

        if (moveVector.y > 0)
        {
            anim.SetBool("Down", false);
            anim.SetBool("Up", true);
        }
        else if (moveVector.y < 0)
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", true);
        }
    }
    private void LighterWalkAnimation()
    {
        
        
        if (moveVector != Vector2.zero)
        {
            anim.SetFloat("WalkAnimationSpeed", animationSpeed);
            
            if (currentTime > 0)
                currentTime -= Time.deltaTime;
            else if (currentTime <= 0)
                IsRunning = true;

            anim.SetBool("Walk_LightOn", lighter.lighterIsOn);
            anim.SetBool("Walk_LightOff", !lighter.lighterIsOn);
        }
        else
        {
            currentTime = timeBeforeStartRunning;
            IsRunning = false;
            anim.SetBool("Walk_LightOn", false);
            anim.SetBool("Walk_LightOff", false);
        }
            
    }

    private void StopMovement(object sender, System.EventArgs e)
    {
        doingSomethingImportant = true;
    }

    public void StepSoundEvent()
    {
        if (overCarpet)
            FMODUnity.RuntimeManager.PlayOneShot(stepsoundOnCarpet);
        else
        {
            OnStepSound?.Invoke(this, EventArgs.Empty);
            FMODUnity.RuntimeManager.PlayOneShot(stepsound);
            lastStepSound.transform.position = transform.position;
            //Instantiate(soundWave, transform.position, Quaternion.identity);
        }
    }
    public void TeleportPlayer(Transform pointToGo)
    {
        transform.position = pointToGo.position;
    }
   
    private void FlipSprite()
    {
        if (moveVector.x < 0)
        
            GameObject.Find("PlayerSprite").transform.localScale = new Vector2(-1, 1);
            
        
        else if (moveVector.x > 0)
        
            GameObject.Find("PlayerSprite").transform.localScale = new Vector2(1, 1);
                
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Carpet"))
        {
            overCarpet = true;
        }

        if(collision.CompareTag("Candle"))
        {
            isNearCandle = true;
        }

        if (collision.CompareTag("Teleport"))
        {
            transform.position = collision.GetComponent<ChangeRoomTrigger>().GetDestination().position;
            if (inSecondFloor)
            {
                location.playerCurrentRoom = CurrentRoom.MainRoom;
                inSecondFloor = false;
                
            }
            else if(!inSecondFloor)
            {
                location.playerCurrentRoom = CurrentRoom.SecondFloorMain;
                inSecondFloor = true;
            }
               
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Carpet"))
        {
            overCarpet = false;
        }

        if (collision.CompareTag("Candle"))
        {
            isNearCandle = false;
        }
    }

    private void LightingAnimation()
    {
        doingSomethingImportant = true;
        anim.SetBool("LightingAnimation", true);
        
    }

    public void LightingAnimationEventToStop()
    {
        anim.SetBool("LightingAnimation", false);
        doingSomethingImportant = false;
        candleAnimation = false; 
    }

}
