using UnityEngine;
using Pathfinding;

public class MonsterAnimations : MonoBehaviour
{
    private Animator anim;
    private AIPath path;
    private SpriteRenderer sprite;

    [SerializeField] private bool isMoving;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        path = GetComponent<AIPath>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (path.desiredVelocity.x >= 0.01f)
        {
            sprite.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (path.desiredVelocity.x <= -0.01f)
        {
            sprite.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if ((Vector2)path.desiredVelocity != Vector2.zero)
            isMoving = true;
        else
            isMoving = false;

      
        anim.SetBool("Walk", isMoving);

    }
}
