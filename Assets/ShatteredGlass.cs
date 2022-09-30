using UnityEngine;
using System;

public class ShatteredGlass : MonoBehaviour
{
    public static event EventHandler glass_steped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            glass_steped?.Invoke(this, EventArgs.Empty);
        }
        
    }
}