using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMOD;
using FMODUnity;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovementBrightHouse : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputManager inputManager;
    private Animator anim;
    private FMODEventPlayable playable;
    
    public event System.EventHandler OnStepSound;

    //Sound

    [SerializeField] private int footstepatt;
    [SerializeField] EventReference stepsound;
    [SerializeField] EventReference stepsoundOnCarpet;
    
    public FMOD.Studio.EventInstance stepSoundInstance;
    public FMOD.Studio.EventInstance stepSoundOnCarpetInstance;
    public FMODUnity.EventReference fmodEvent;

    [SerializeField] private Vector2 movementSpeed;
    private Vector2 moveVector;

    private bool overCarpet;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
  
        inputManager = new InputManager();

    }

    private void OnEnable()
    {
        inputManager.Player.Enable();
    }
    private void OnDisable()
    {
        inputManager.Player.Disable();
    }

    private void Start()
    {
        stepSoundInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        stepSoundInstance.start();
        

        stepSoundOnCarpetInstance = FMODUnity.RuntimeManager.CreateInstance(stepsoundOnCarpet);
        stepSoundOnCarpetInstance.start();
    }

    private void Update()
    {
        ReadMovementValue();
        stepSoundInstance.setParameterByName("Material", footstepatt);
        WalkAnimation();
        FlipSprite();

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementSpeed.x * moveVector.x, movementSpeed.y * moveVector.y) * Time.deltaTime;
    }

    private void ReadMovementValue()
    {
        moveVector = inputManager.Player.Movement.ReadValue<Vector2>();
    }

    private void WalkAnimation()
    {
        if (moveVector != Vector2.zero)
            anim.SetBool("Walk_LightOn", true);
        else
            anim.SetBool("Walk_LightOn", false);
    }


    public void StepSoundEvent()
    {
        OnStepSound?.Invoke(this, EventArgs.Empty);

        if (overCarpet)
            FMODUnity.RuntimeManager.PlayOneShot(stepsoundOnCarpet);


        else
            FMODUnity.RuntimeManager.PlayOneShot(stepsound);


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
