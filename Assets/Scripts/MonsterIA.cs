using UnityEngine;

public class MonsterIA : MonoBehaviour
{

    public enum Status {alert, patrol, chase }
    
    private Rigidbody2D rb;
    private Transform player;

    private Vector2 movementSpeed;

    [SerializeField] private Vector2 playerPos;
    [SerializeField] private Status status = Status.patrol;

    [SerializeField] private Vector2 patrolSpeed;
    [SerializeField] private Vector2 alertSpeed;
    [SerializeField] private Vector2 chaseSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").gameObject.transform;
    }

    private void Update()
    {
        playerPos = player.position;
    }

    private void FixedUpdate()
    {
        
    }

    private void GoToDirection(Vector2 direcionToGo)
    {
        rb.velocity = movementSpeed * Time.deltaTime;
    }

    private void ChangeMovementSpeed()
    {
        switch (status)
        {
            case Status.alert:

                movementSpeed = alertSpeed;
                break;
            case Status.patrol:

                movementSpeed = patrolSpeed;
                break;
            case Status.chase:

                movementSpeed = chaseSpeed;
                break;
        }
    }

}
