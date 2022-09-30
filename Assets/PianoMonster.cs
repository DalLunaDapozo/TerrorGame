using System;
using UnityEngine;

public class PianoMonster : MonoBehaviour
{

    [SerializeField] private float time_in_alert_mode;

    [SerializeField] private GameObject piano_sound;
    [SerializeField] private GameObject piano_music;

    [SerializeField] private Transform player_pos;
    
    private Animator anim;
    private float timer;

    public event EventHandler piano_monster_kill_player;

    private bool in_alert;
    private bool go_to_player;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ShatteredGlass.glass_steped += React_To_Sound;
    }

    private void Start()
    {
        timer = time_in_alert_mode;
    }
    private void Update()
    {
        Timer();
        anim.SetBool("alert", in_alert);
    }

    private void FixedUpdate()
    {
        
        if (go_to_player) Go_Towards_Player();
    }

    private void Timer()
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            in_alert = false;
            piano_music.gameObject.SetActive(true);
            piano_sound.gameObject.SetActive(false);
        }
        
    }

    private void Go_Towards_Player()
    {
        transform.position = Vector3.MoveTowards(transform.position, player_pos.position, 1f);
    }

    private void React_To_Sound(object sender, System.EventArgs e)
    {

        if (in_alert)
        {
            piano_monster_kill_player?.Invoke(this, EventArgs.Empty);
            go_to_player = true;
            anim.SetTrigger("go");
            return;
        }
        else
        {
            in_alert = true;
            timer = time_in_alert_mode;
            piano_music.gameObject.SetActive(false);
            piano_sound.gameObject.SetActive(true);
        }
        
    }
}
