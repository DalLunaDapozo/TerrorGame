using UnityEngine;
using System;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private HideAndSeekTest hide_and_seek_game;
    

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        hide_and_seek_game.player_is_well_hidden += Play_not_get_kill_scene;
        hide_and_seek_game.player_is_not_well_hidden += Play_get_kill_scene;
    }

    private void Play_not_get_kill_scene(object sender, EventArgs e)
    {
        anim.Play("Gets_not_kill_scene");
    }

    private void Play_get_kill_scene(object sender, EventArgs e)
    {
        anim.Play("Gets_kill_scene");
    }
}
