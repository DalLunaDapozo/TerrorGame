using UnityEngine;


[ExecuteInEditMode]
public class OrderInLayerEntity : MonoBehaviour
{

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        sr.sortingOrder = -(int)(transform.position.y * 100);
    }
}
