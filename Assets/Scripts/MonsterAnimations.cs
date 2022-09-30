using UnityEngine;
using Pathfinding;
using FMODUnity;
using System.Collections;

public class MonsterAnimations : MonoBehaviour
{
    public Animator anim;
    public AIPath path;
    private SpriteRenderer sprite;
    private MonsterAI monsterAI;
    private PlayerMovement player;

    [SerializeField] public bool isMoving;

    [SerializeField] private FMODUnity.StudioEventEmitter alertGrowl;
    [SerializeField] FMODUnity.EventReference crackSound;

    public bool deactivated;
    public FMODUnity.StudioEventEmitter eventEmitter;
  
    private void Awake()
    {
        anim = GetComponent<Animator>();
        path = GetComponent<AIPath>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        monsterAI = GetComponent<MonsterAI>();
        eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();

        try { player = GameObject.Find("Player").GetComponent<PlayerMovement>(); }
        catch { Debug.Log("Player missing"); }
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

        if(!deactivated)
        {
            if ((Vector2)path.desiredVelocity != Vector2.zero)
                isMoving = true;
            else
                isMoving = false;
        }
       

      
        anim.SetBool("Walk", isMoving);
       
    }

   
    public void KillingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(crackSound, gameObject);
    }
    
    public void AlertGrowl()
    {
        alertGrowl.Play();
    }

    public void StepSound()
    {
        eventEmitter.Play();
    }

}
