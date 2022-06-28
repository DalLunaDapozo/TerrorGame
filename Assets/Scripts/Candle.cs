using UnityEngine.InputSystem;
using UnityEngine;

public class Candle : MonoBehaviour
{

    public bool canLightCandle;
    public bool lightIsOn;


    private PlayerInput input;
    private Animator anim;
    private BoxCollider2D boxCol;

    [SerializeField] private GameObject fire;
        
    //HUERGO CABEZA DE CHORLITO

    private void Awake()
    {
        input = new PlayerInput();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        input.Keyboard.Enable();
    }

    private void Update()
    {
        if (lightIsOn)
        {
            anim.SetTrigger("FireCandle");
        }

        if (!lightIsOn && input.Keyboard.Lighter.triggered && canLightCandle)
        {
            lightIsOn = true;
            boxCol.enabled = false;
            fire.SetActive(true);
        }

    }

    private void Start()
    {
        lightIsOn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canLightCandle = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canLightCandle = false;
    }
}
