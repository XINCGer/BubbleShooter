using UnityEngine;
using System.Collections;

public class RabbitAnim : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	
	// Update is called once per frame
	public void Idle () {
        GetComponent<Animation>().Play( "rabbit_idle" );
	}
}
