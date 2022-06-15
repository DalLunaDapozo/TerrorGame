using UnityEngine;
using Pathfinding;
using FMODUnity;

public class MonsterAnimations : MonoBehaviour
{
    private Animator anim;
    private AIPath path;
    private SpriteRenderer sprite;

    [SerializeField] private bool isMoving;
    [SerializeField] EventReference stepsound;
    public FMOD.Studio.EventInstance monsterState;

    public FMODUnity.StudioEventEmitter eventEmitter;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        path = GetComponent<AIPath>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void Start()
    {
        monsterState = FMODUnity.RuntimeManager.CreateInstance(stepsound);
        monsterState.start();

       

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

    public void StepSound()
    {
        
        eventEmitter.Play();
        //FMODUnity.RuntimeManager.PlayOneShotAttached(stepsound, gameObject);
    }

}
