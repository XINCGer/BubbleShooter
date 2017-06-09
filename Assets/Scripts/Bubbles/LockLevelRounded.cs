using UnityEngine;
using System.Collections;

public class LockLevelRounded : MonoBehaviour
{
    Vector3 staticPos;
    public static LockLevelRounded Instance;
    Vector3 dir;
    Vector3 ballPos;
    float angle;
    Quaternion newRot;
    private bool addForce;
    // Use this for initialization
    void Start()
    {
        Instance = this;
        staticPos = transform.position;
        newRot = Quaternion.identity;
    }

    public void Rotate(Vector3 _dir, Vector3 _ballPos)
    {
        _dir = MainScript.Instance.boxCatapult.GetComponent<Grid>().transform.position;
        angle = Vector2.Angle(_dir - _ballPos, _ballPos - transform.position) / 4f;
        if (transform.position.x < _ballPos.x) angle *= -1;
        newRot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.back);
        addForce = true;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().kreakWheel);
    }

    void Update()
    {
        if (transform.rotation != newRot)
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime);
    }

}
