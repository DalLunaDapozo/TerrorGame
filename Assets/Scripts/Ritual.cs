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

    [SerializeField] FMODUnity.EventReference objectSpawnSound;
    public FMOD.Studio.EventInstance objectSpawnState;

    [SerializeField] private Material material;

    public float r;
    public float b;
    public float g;
    public float intensity;

    public Color color;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (lightOnStart)
            PlayRitualAnimation(ritualSound);
        color = new Color(r * intensity, g * intensity, b * intensity);
    }

    private void OnEnable()
    {
        skullsManager.OneKeyCandleLighted += SpawnEvent;
    }

    private void Update()
    {
        color = new Color(r * intensity, g * intensity, b * intensity);
        material.SetColor("_EmissionColor", color);
    }

    public void PlayRitualAnimation(FMODUnity.EventReference sound)
    {
        
        anim.SetTrigger("Ritual");
        FMODUnity.RuntimeManager.PlayOneShot(sound);
    }    

    private void SpawnObject(GameObject something)
    {
        Instantiate(something, transform.position, Quaternion.identity);
    }

    private void SpawnEvent(object sender, System.EventArgs e)
    {
        PlayRitualAnimation(objectSpawnSound);
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
