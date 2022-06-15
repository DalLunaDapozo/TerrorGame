using UnityEngine;
using FMODUnity;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class Lighter : MonoBehaviour
{

    private Light2D light2D;
    private InputManager input;
    private GameObject fire;
    private PlayerMovement player;
    private FMOD.Studio.EventInstance lighterloopInstance;
    
    [SerializeField] EventReference lightersound;
    [SerializeField] EventReference lighteroff;
    [SerializeField] EventReference lighterloop;
    
    [SerializeField] private float lightIntensityLow;
    [SerializeField] private float lightIntensityHigh;

    public event System.EventHandler OnLighterSound;

    public bool lighterIsOn;
    
    private void Awake()
    {
        input = new InputManager();
        light2D = GetComponent<Light2D>();
        fire = GameObject.Find("Fire");
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        input.Player.Enable();
        input.Player.Lighter.performed += LighterAction;
    }  
    private void OnDisable()
    {
        input.Player.Disable();
        input.Player.Lighter.performed -= LighterAction;
    }
    private void Start()
    {
        lighterIsOn = false;
        fire.SetActive(false);
        light2D.intensity = lightIntensityLow;

        lighterloopInstance = FMODUnity.RuntimeManager.CreateInstance(lighterloop);
    }
    private void Update()
    {
        FireLoopSound();
    }

    private void FireLoopSound()
    {
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
    private void LighterAction(InputAction.CallbackContext callbackContext)
    {
        if (lighterIsOn)
        {
            if (player.isNearCandle)
                player.SendMessage("LightingAnimation");
            else
            {
                fire.SetActive(false);
                light2D.intensity = lightIntensityLow;
                lighterIsOn = false;
                FMODUnity.RuntimeManager.PlayOneShot(lighteroff);
            }
        }
        else
        {

            FMODUnity.RuntimeManager.PlayOneShot(lightersound);
            OnLighterSound?.Invoke(this, System.EventArgs.Empty);
            player.lastStepSound.transform.position = transform.position;

            float percentChance = 0.5f;
            if (Random.value <= percentChance)
            {
                fire.SetActive(true);
                light2D.intensity = lightIntensityHigh;
                lighterIsOn = true;

            }
            else
                return;
        }

    }
}
