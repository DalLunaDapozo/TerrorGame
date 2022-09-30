using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private PlayerMovement player;
    private Rigidbody2D rb;

    [SerializeField] private float alertZone;
    [SerializeField] private float speed;

    private bool freakingOut;

    private Vector2 playerPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        try {player = GameObject.Find("Player").GetComponent<PlayerMovement>(); }
        catch { Debug.Log("Player not found"); }
    }

    private void Update()
    {
        StatusDependingOnPlayerDistance();

        if (freakingOut)
            playerPos = (player.transform.position - transform.position).normalized * speed;
    }

    private void FixedUpdate()
    {
        OnFreakingOut();
    }

    private void OnFreakingOut()
    {
        if (freakingOut)
            rb.velocity = playerPos;
    }

    private void StatusDependingOnPlayerDistance()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);

        if (distance <= alertZone)
            freakingOut = true;
        else
            freakingOut = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertZone);

    }
}
