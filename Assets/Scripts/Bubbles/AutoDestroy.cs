using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		Invoke("Hide",3);
	}

	void Hide(){
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
