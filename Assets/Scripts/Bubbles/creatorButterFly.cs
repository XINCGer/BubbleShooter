using UnityEngine;
using System.Collections;

public class creatorButterFly : MonoBehaviour {
	public GameObject butterFly_hd;
	public GameObject butterFly_ld;
	GameObject thePrefab;

	// Use this for initialization
	void Start () {
			thePrefab = butterFly_hd;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void createButterFly(int revertButterFly){
		GameObject b = Instantiate(thePrefab, thePrefab.transform.position, transform.rotation) as GameObject;
		b.GetComponent<butterfly>().revertButterFly = revertButterFly;
		b.transform.Rotate(new Vector3(0f, 0f, 0f));
	}

}
