using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetIcon : MonoBehaviour {
    public Sprite[] targets;
	// Use this for initialization
	void Start () {
	    GetComponent<Image>().sprite = targets[(int)LevelData.mode];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
