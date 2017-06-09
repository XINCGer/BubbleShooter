using UnityEngine;
using System.Collections;

public class butterfly : MonoBehaviour
{
    Vector3 tempPosition;
    Vector3 targetPrepare;
    bool started;
    bool isPaused;
    float startTime;
    bool arcadeMode;
    public int revertButterFly;
    // Use this for initialization
    void Start()
    {
        isPaused = Camera.main.GetComponent<MainScript>().isPaused;
        arcadeMode = Camera.main.GetComponent<MainScript>().arcadeMode;
        if (revertButterFly == 1)
        {
            transform.position = new Vector3(3, Random.Range(-2, 5), -20f);
            flyTo(new Vector3(transform.position.x - 18, transform.position.y, transform.localScale.z));
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);  //flip horrizontal
            transform.position = new Vector3(-3, Random.Range(-2, 5), -20f);
            flyTo(new Vector3(transform.position.x + 18, transform.position.y, transform.localScale.z));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -4 || transform.position.x > 4) Destroy(gameObject);
    }


    public void flyTo(Vector3 vector)
    {
        bounceTo(vector);
    }

    public void OnTriggerEnter2D(Collider2D owner)
    {
        // check if we collided with a top block and adjust our speed and rotation accordingly
        if (owner.gameObject.name.IndexOf("ball") == 0 && owner.gameObject.GetComponent<ball>().setTarget)
        {
            //	SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.Pickup);
            //	Score.Instance.ShowPopup( 50);
            //	Score.Instance.addScore( 50);
            Destroy(gameObject);
        }
    }

    public void bounceTo(Vector3 vector3)
    {
        vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
        tempPosition = transform.position;
        targetPrepare = vector3;
        startTime = Time.time;
        StartCoroutine(bonceCoroutine());
    }

    IEnumerator bonceCoroutine()
    {
        float speed = 1f;
        if (arcadeMode) speed = 0.3f;
        while (Vector3.Distance(transform.position, targetPrepare) > 1 && !isPaused)
        {
            //transform.position  += targetPrepare * Time.deltaTime;
            transform.position = Vector3.Lerp(tempPosition, targetPrepare, (Time.time - startTime) * speed);
            //	transform.position  = targetPrepare ;
            yield return new WaitForSeconds(1f / 50f);
        }
    }

}
