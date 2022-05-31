using UnityEngine;
using FMODUnity;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class Lighter : MonoBehaviour
{

    private Light2D light2D;
    private InputManager input;
    private GameObject fire;
    private FMOD.Studio.EventInstance lighterloopInstance;
    
    [SerializeField] EventReference lightersound;
    [SerializeField] EventReference lighteroff;
    [SerializeField] EventReference lighterloop;
    
    [SerializeField] private float lightIntensityLow;
    [SerializeField] private float lightIntensityHigh;

    public bool lighterIsOn;
    
    private void Awake()
    {
        input = new InputManager();
        light2D = GetComponent<Light2D>();
        fire = GameObject.Find("Fire");
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
        lighterIsOn = true;
        fire.SetActive(true);
        light2D.intensity = lightIntensityHigh;

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
            fire.SetActive(false);
            light2D.intensity = lightIntensityLow;
            lighterIsOn = false;
            FMODUnity.RuntimeManager.PlayOneShot(lighteroff);
        }
        else
        {
            fire.SetActive(true);
            light2D.intensity = lightIntensityHigh;
            lighterIsOn = true;
            FMODUnity.RuntimeManager.PlayOneShot(lightersound);
        }

    }
}
