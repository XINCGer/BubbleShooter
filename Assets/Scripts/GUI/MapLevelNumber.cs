using UnityEngine;
using System.Collections;

public class MapLevelNumber : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().sortingLayerName = "New Layer 1";
        GetComponent<Renderer>().sortingOrder = 1;
        int num = int.Parse( transform.parent.name.Replace( "Level", "" ) );
        GetComponent<TextMesh>().text = "" + num;
        if( num >= 10 ) transform.position += Vector3.left * 0.05f;
        if( num == 1 || num == 11 ) transform.position += Vector3.right * 0.05f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
