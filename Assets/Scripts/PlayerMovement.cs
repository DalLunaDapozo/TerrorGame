using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMOD;
using FMODUnity;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [HideInInspector] public InputManager inputManager;
    private Animator anim;
    private FMODEventPlayable playable;
    private MonsterIA monsterIA;

    public event System.EventHandler OnStepSound;

    //fmod yo el mas capo

    [SerializeField] EventReference stepsound;
    [SerializeField] EventReference lightersound;
    [SerializeField] EventReference lighteroff;
    [SerializeField] EventReference lighterloop;
    public FMOD.Studio.EventInstance playerState;
    public FMOD.Studio.EventInstance lighterloopInstance;



    private Vector2 movementSpeed;
    private Vector2 moveVector;

    public bool lighterIsOn;

    [SerializeField] private GameObject lighter;
    [SerializeField] private GameObject fire;
    private Light2D lightIntensity;
    [SerializeField] private float lightIntensityLow;
    [SerializeField] private float lightIntensityHigh;

    [SerializeField] private Vector2 movementSpeedLighterOn;
    [SerializeField] private Vector2 movementSpeedLighterOff;

    private bool catchedByMonster;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        lightIntensity = lighter.GetComponent<Light2D>();
            
        inputManager = new InputManager();
       
        monsterIA = GameObject.Find("Monster").GetComponent<MonsterIA>();
        
    }

    private void OnEnable()
    {
        inputManager.Player.Enable();
        inputManager.Player.Lighter.performed += LighterAction;
        monsterIA.OnPlayerCatched += StopMovement;
    }
    private void OnDisable()
    {
        inputManager.Player.Disable();
        inputManager.Player.Lighter.performed -= LighterAction;
        monsterIA.OnPlayerCatched -= StopMovement;
    }
    
    private void Start()
    {
        lighterIsOn = true;
        fire.SetActive(true);
        lightIntensity.intensity = lightIntensityHigh;


        playerState = FMODUnity.RuntimeManager.CreateInstance(stepsound);
        playerState.start();

        lighterloopInstance = FMODUnity.RuntimeManager.CreateInstance(lighterloop);


    }

    private void Update()
    {
        ReadMovementValue();
        ChangeMovementSpeed();
        WalkAnimation();
        FlipSprite();

        if (lighterIsOn)
        {
            lighterloopInstance.start();
            lighterloopInstance.release();

        }
        else
        {
            lighterloopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

    }

    private void FixedUpdate()
    {
        if (!catchedByMonster)
            rb.velocity = new Vector2(movementSpeed.x * moveVector.x, movementSpeed.y * moveVector.y) * Time.deltaTime;
        else
            rb.velocity = Vector2.zero;
    }

    private void ChangeMovementSpeed()
    {
        if (lighterIsOn)
            movementSpeed = movementSpeedLighterOn;
        else
            movementSpeed = movementSpeedLighterOff;
    }

    private void ReadMovementValue()
    {
        moveVector = inputManager.Player.Movement.ReadValue<Vector2>();
    }

    private void WalkAnimation()
    {
        if (moveVector != Vector2.zero)
            anim.SetBool("Walk", true);
        else
            anim.SetBool("Walk", false);
    }

    private void StopMovement(object sender, System.EventArgs e)
    {
        catchedByMonster = true;
    }

    private void LighterAction(InputAction.CallbackContext callbackContext)
    {
        if (lighterIsOn)
        {
            fire.SetActive(false);
            lightIntensity.intensity = lightIntensityLow;
            lighterIsOn = false;
            FMODUnity.RuntimeManager.PlayOneShot(lighteroff);
        }
        else
        {
            fire.SetActive(true);
            lightIntensity.intensity = lightIntensityHigh;
            lighterIsOn = true;
            FMODUnity.RuntimeManager.PlayOneShot(lightersound);
        }
            
    }

    public void StepSoundEvent()
    {
        OnStepSound?.Invoke(this, EventArgs.Empty);
        FMODUnity.RuntimeManager.PlayOneShot(stepsound);
      
    }
   
    private void FlipSprite()
    {
        if (moveVector.x < 0)
        
            GameObject.Find("PlayerSprite").transform.localScale = new Vector2(-1, 1);
            
        
        else if (moveVector.x > 0)
        
            GameObject.Find("PlayerSprite").transform.localScale = new Vector2(1, 1);
                
    }

}
