using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    [Header("UI")]
    public AudioClip countdown;
    public AudioClip gameStart;
    public AudioClip victory;
    public AudioClip defeat;

    [Header("Tower")]
    public AudioClip placement;
    public AudioClip hover;
    public AudioClip upgrade;
    public AudioClip electric;
    public AudioClip boomerang;
    public AudioClip star;
    public AudioClip baseHit;

    [Header("Enemy")]
    public AudioClip spawn;
    public AudioClip hit;
    public AudioClip death;
    public AudioClip dragon;
    public AudioClip dragonFly;
}