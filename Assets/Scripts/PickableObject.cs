using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{

    public bool isKey;
    public bool isHeart;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hand"))
        {
            transform.parent = collision.gameObject.transform;
            transform.position = collision.gameObject.transform.position;
        }

        if (isKey && collision.gameObject.CompareTag("Door"))
        {
            collision.gameObject.SetActive(false);
            Destroy(gameObject);
        }

        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
    }

}
