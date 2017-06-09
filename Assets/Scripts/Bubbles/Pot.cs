using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pot : MonoBehaviour
{
    public int score;
    public Text label;
    public GameObject splashPrefab;

    // Use this for initialization
    void Start()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name.Contains("ball"))
        {
            col.gameObject.GetComponent<ball>().SplashDestroy();
            col.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            col.gameObject.GetComponent<Collider2D>().enabled = false;
            PlaySplash(col.contacts[0].point);
        }
    }

    void PlaySplash(Vector2 pos)
    {
        StartCoroutine(SoundsCounter());
        if (MainScript.Instance.potSounds < 4)
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().pops);

        GameObject splash = (GameObject)Instantiate(splashPrefab, transform.position + Vector3.up * 0.9f + Vector3.left * 0.35f, Quaternion.identity);
        Destroy(splash, 2f);

        MainScript.Instance.PopupScore(score * MainScript.doubleScore, transform.position + Vector3.up);
    }

    IEnumerator SoundsCounter()
    {
        MainScript.Instance.potSounds++;
        yield return new WaitForSeconds(0.2f);
        MainScript.Instance.potSounds--;
    }


    // Update is called once per frame
    void Update()
    {
        label.text = "" + score * MainScript.doubleScore;
    }
}
