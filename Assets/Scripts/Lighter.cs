using UnityEngine;
using FMODUnity;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class Lighter : MonoBehaviour
{

    private Light2D light2D;
    private PlayerInput input;
    private GameObject fire;
    private PlayerMovement player;
    private MonsterAI monster;
    private GameController gameController;
    private FMOD.Studio.EventInstance lighterloopInstance;
    
    [SerializeField] EventReference lightersound;
    [SerializeField] EventReference lighteroff;
    [SerializeField] EventReference lighterloop;
    
    [SerializeField] private float lightIntensityLow;
    [SerializeField] public float lightIntensityHigh;

    public event System.EventHandler OnLighterSound;

    public bool lighterIsOn;

    public bool gamepad;
    public bool gas;

    private void Awake()
    {
        input = new PlayerInput();
        light2D = GetComponent<Light2D>();
        fire = GameObject.Find("Fire");
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();

        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        try { monster = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("Monster not found"); }
    }
    private void OnEnable()
    {
        if(gamepad)
        {
            input.Gamepad.Enable();

            input.Gamepad.Spark.performed += Spark;
            input.Gamepad.Gas.performed += GasOn;
            input.Gamepad.Gas.canceled += GasOff;
        }
        else
        {
            input.Keyboard.Enable();
            input.Keyboard.Lighter.performed += LighterAction;
        }

        monster.StopPlayerMovement += TurnOff;
        gameController.PlayerSpawned += TurnOn;
    }  
    private void OnDisable()
    {
        input.Gamepad.Disable();
        input.Keyboard.Disable();

        monster.OnPlayerCatched -= TurnOff;
        gameController.PlayerSpawned -= TurnOn;
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

        if(gamepad)
        {
            if (gas && lighterIsOn)
            {
                fire.SetActive(true);
                light2D.intensity = lightIntensityHigh;
            }
            else if (!gas)
            {
                fire.SetActive(false);
                light2D.intensity = lightIntensityLow;
                lighterIsOn = false;
            }
        }
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

    public void SetFire(bool a)
    {
        fire.SetActive(a);
    }

    private void TurnOff(object sender, System.EventArgs e)
    {
        lighterIsOn = false;
        input.Keyboard.Disable();
        input.Gamepad.Disable();
    }

    private void TurnOn(object sender, System.EventArgs e)
    {
        lighterIsOn = true;
        input.Keyboard.Enable();
        input.Gamepad.Enable();
    }
    public void SetLightIntensity(float a)
    {
        light2D.intensity = a;
    }

    private void GasOn(InputAction.CallbackContext callbackContext)
    {
        gas = true;
    }
    private void GasOff(InputAction.CallbackContext callbackContext)
    {
        gas = false;
    }
    private void Spark(InputAction.CallbackContext callbackContext)
    {
        if (lighterIsOn)
            return;
        else
        {
            if (gas)
            {
                float percentChance = 0.5f;
                if (Random.value <= percentChance)
                {
                    lighterIsOn = true;

                }

            }

            fire.SetActive(true);
            light2D.intensity = lightIntensityHigh;
            FMODUnity.RuntimeManager.PlayOneShot(lightersound);
            OnLighterSound?.Invoke(this, System.EventArgs.Empty);
            player.lastStepSound.transform.position = transform.position;
            light2D.intensity = lightIntensityLow;
            fire.SetActive(false);
        }
    }
}
