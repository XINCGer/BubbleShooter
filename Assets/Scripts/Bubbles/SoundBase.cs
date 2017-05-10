using UnityEngine;
using System.Collections;

/// <summary>
/// 用来存储游戏中所有声音片段的单例类
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundBase : MonoBehaviour
{

    private static SoundBase _instance;
    public AudioClip click;
    public AudioClip[] combo;
    public AudioClip[] swish;
    public AudioClip bug;
    public AudioClip bugDissapier;
    public AudioClip pops;
    public AudioClip boiling;
    public AudioClip hit;
    public AudioClip kreakWheel;
    public AudioClip spark;
    public AudioClip winSound;
    public AudioClip gameOver;
    public AudioClip scoringStar;
    public AudioClip scoring;
    public AudioClip alert;
    public AudioClip aplauds;
    public AudioClip OutOfMoves;
    public AudioClip Boom;
    public AudioClip black_hole;


    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public static SoundBase GetInstance()
    {
        return _instance;
    }

}
