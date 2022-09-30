using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartWave : MonoBehaviour
{
    private SpriteRenderer sprite;
    
    private Vector2 waveRadius;
    public float waveGrowRate;
    public float waveMaxRadius;

    private float x;
    private float y;


    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (transform.localScale.x >= waveMaxRadius)
        {
            Destroy(gameObject);
        }
        else
        {
            x += waveGrowRate;
            y += waveGrowRate;

            transform.localScale = new Vector2(x, y);
        }
    }

}
