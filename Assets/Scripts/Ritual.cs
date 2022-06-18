using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ritual : MonoBehaviour
{
    private Animator anim;
  

    [SerializeField] SkullsManager skullsManager;
    [SerializeField] private GameObject key;
    
    public event System.EventHandler OnHeartEnding;
    public bool lightOnStart;

    [SerializeField] FMODUnity.EventReference ritualSound;
    public FMOD.Studio.EventInstance ritualSoundState;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (lightOnStart)
            PlayRitualAnimation();
    }

    private void OnEnable()
    {
        skullsManager.OneKeyCandleLighted += SpawnEvent;
    }

    public void PlayRitualAnimation()
    {
        
        anim.SetTrigger("Ritual");
        FMODUnity.RuntimeManager.PlayOneShot(ritualSound);
    }    

    private void SpawnObject(GameObject something)
    {
        Instantiate(something, transform.position, Quaternion.identity);
    }

    private void SpawnEvent(object sender, System.EventArgs e)
    {
        PlayRitualAnimation();
        SpawnObject(key);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Heart"))
        {
            OnHeartEnding?.Invoke(this, System.EventArgs.Empty);
        }
    }

}
