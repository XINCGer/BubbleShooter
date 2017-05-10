using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Perfect : MonoBehaviour {
    public Sprite[] images;
	// Use this for initialization
	void OnEnable () {
        GetComponent<Image>().sprite = images[Random.Range( 0, images.Length )];
        GetComponent<Image>().SetNativeSize();
        StartCoroutine(PerfectAction());
	}

    IEnumerator PerfectAction()
    {
        yield return new WaitForSeconds( 2 );
        gameObject.SetActive( false );
    }
	// Update is called once per frame
	void Update () {
	
	}
}
