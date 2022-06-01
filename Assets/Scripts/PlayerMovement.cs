using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMOD;
using FMODUnity;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputManager inputManager;
    private Animator anim;
    private FMODEventPlayable playable;
    private MonsterIA monsterIA;
    private Lighter lighter;

    [SerializeField] private GameObject soundWave;
    public event System.EventHandler OnStepSound;

    [SerializeField] public Transform lastStepSound;

    //Sound

    [SerializeField] EventReference stepsound;
    public FMOD.Studio.EventInstance playerState;

    [SerializeField] EventReference stepsoundOnCarpet;
    public FMOD.Studio.EventInstance stepSoundOnCarpetInstance;

    private Vector2 movementSpeed;
    private Vector2 moveVector;

    [SerializeField] private Vector2 movementSpeedLighterOn;
    [SerializeField] private Vector2 movementSpeedLighterOff;

    private bool catchedByMonster;
    public bool overCarpet;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lighter = GameObject.Find("Lighter").GetComponent<Lighter>();

        inputManager = new InputManager();
       
       
        monsterIA = GameObject.Find("Monster").GetComponent<MonsterIA>();
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
    }

    private void Update()
    {
        ReadMovementValue();
        ChangeMovementSpeed();
        WalkAnimation();
        FlipSprite();

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
        if (lighter.lighterIsOn)
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
        {
            anim.SetBool("Walk_LightOn", lighter.lighterIsOn);
            anim.SetBool("Walk_LightOff", !lighter.lighterIsOn);
        }
        else
        {
            anim.SetBool("Walk_LightOn", false);
            anim.SetBool("Walk_LightOff", false);
        }
            
    }

    private void StopMovement(object sender, System.EventArgs e)
    {
        catchedByMonster = true;
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

        if (collision.CompareTag("Teleport"))
        {
            transform.position = collision.GetComponent<ChangeRoomTrigger>().GetDestination().position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Carpet"))
        {
            overCarpet = false;
        }
    }
}
