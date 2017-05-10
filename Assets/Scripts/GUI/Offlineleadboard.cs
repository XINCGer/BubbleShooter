using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Offlineleadboard : MonoBehaviour {
    Text label;
	// Use this for initialization
	void OnEnable () {
        label = transform.Find( "Slot" ).Find( "Score" ).GetComponent<Text>();
        label.text = "" +  PlayerPrefs.GetInt( "Score" + PlayerPrefs.GetInt( "OpenLevel"));
	}
	
	// Update is called once per frame
	void OnDisable () {
        label.text = "";
	}
}
