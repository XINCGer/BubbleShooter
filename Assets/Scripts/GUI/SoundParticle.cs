using UnityEngine;
using System.Collections;

public class SoundParticle : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }


    public void Stop()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().swish[0]);
    }
    public void Hit()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().hit);
    }

}
