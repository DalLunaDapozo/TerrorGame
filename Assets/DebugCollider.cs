using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollider : MonoBehaviour
{
    public BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col.transform.position, col.size);
        
    }
}
