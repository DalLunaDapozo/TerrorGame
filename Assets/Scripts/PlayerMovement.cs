using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputManager inputManager;

    private Vector2 movementSpeed;
    private Vector2 moveVector;

    public bool lighterIsOn;

    [SerializeField] private GameObject lighter;

    [SerializeField] private Vector2 movementSpeedLighterOn;
    [SerializeField] private Vector2 movementSpeedLighterOff;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputManager = new InputManager();
        inputManager.Player.Enable();

        inputManager.Player.Lighter.performed += LighterAction;
    }

    private void Start()
    {
        if (lighter.activeSelf)
            lighterIsOn = true;
    }

    private void Update()
    {
        ReadMovementValue();
        ChangeMovementSpeed();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementSpeed.x * moveVector.x, movementSpeed.y * moveVector.y) * Time.deltaTime;
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

    private void LighterAction(InputAction.CallbackContext callbackContext)
    {
        if (lighterIsOn)
        {
            lighter.SetActive(false);
            lighterIsOn = false;        
        }
        else
        {
            lighter.SetActive(true);
            lighterIsOn = true;
        }
            
    }

}
