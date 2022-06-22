using UnityEngine.InputSystem;
using UnityEngine;

public class Candle : MonoBehaviour
{

    public bool canLightCandle;
    public bool lightIsOn;


    private InputManager input;
    private Animator anim;
    private BoxCollider2D boxCol;

    [SerializeField] private GameObject fire;
        
    //HUERGO CABEZA DE CHORLITO

    private void Awake()
    {
        input = new InputManager();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void Update()
    {
        if (lightIsOn)
        {
            anim.SetTrigger("FireCandle");
        }

        if (!lightIsOn && input.Player.Lighter.triggered && canLightCandle)
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
