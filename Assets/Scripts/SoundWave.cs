using UnityEngine;

public class SoundWave : MonoBehaviour
{
    
    private float waveRadius = 0.1f;
    public static Collider2D circle;

    public float waveGrowRate;
    public float waveMaxRadius;

    private GameObject lastSpotPos;

    private void Start()
    {
        
        circle = Physics2D.OverlapCircle(transform.position, waveRadius);
    }

    private void Update()
    {
        if (waveRadius >= waveMaxRadius)
        {
            Destroy(gameObject);
        }
        else
        {
            waveRadius += waveGrowRate;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }
}
