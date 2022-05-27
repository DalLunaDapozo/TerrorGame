using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPointToGo : MonoBehaviour
{
    [SerializeField] private Transform monster;
    
    private float distance;

    public float range;
    private void Start()
    {
        monster = GameObject.Find("Monster").transform;
    }
    private void Update()
    {
        distance = Vector2.Distance(transform.position, monster.position);
        
        if (distance <= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
