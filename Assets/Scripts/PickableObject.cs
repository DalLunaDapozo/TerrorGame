using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    MonsterAI monster;
    MadnessManager madness;

    BoxCollider2D box;

    public bool isKey;
    public bool isHeart;

    private void Awake()
    {
        try { monster = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("Monster not found"); }

        try { madness = GameObject.Find("MadnessManager").GetComponent<MadnessManager>(); }
        catch { Debug.Log("MadnessManager not found"); }

        box = GetComponent<BoxCollider2D>();
        

    }

    private void Start()
    {
        madness.HeartAttackEvent += Drop;
        if(monster != null) monster.OnPlayerCatched += Drop;
    }

    private void OnDestroy()
    {
        madness.HeartAttackEvent -= Drop;
    }

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

    private void Drop(object sender, System.EventArgs e)
    {
        StartCoroutine(DropRoutine());
       
    }

    IEnumerator DropRoutine()
    {
       
        box.enabled = false;
        transform.DetachChildren();
        yield return new WaitForSeconds(3f);

        box.enabled = true;
    }

}
