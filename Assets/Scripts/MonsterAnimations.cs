using UnityEngine;
using Pathfinding;
using FMODUnity;

public class MonsterAnimations : MonoBehaviour
{
    public Animator anim;
    private AIPath path;
    private SpriteRenderer sprite;
    private MonsterAI monsterAI;

    [SerializeField] private bool isMoving;

    public FMODUnity.StudioEventEmitter eventEmitter;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        path = GetComponent<AIPath>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        monsterAI = GetComponent<MonsterAI>();
        eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
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

        if (monsterAI.playerCatched)
        {
            anim.Play("MonsterKilling_1");
        }

       
    }

    public void StepSound()
    {
        
        eventEmitter.Play();
        //FMODUnity.RuntimeManager.PlayOneShotAttached(stepsound, gameObject);
    }

}
