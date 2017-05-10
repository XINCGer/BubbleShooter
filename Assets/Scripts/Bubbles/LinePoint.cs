using UnityEngine;
using System.Collections;

public class LinePoint : MonoBehaviour {
    int nextWayPoint;
    float timeToLerp = 5; //lerp for two seconds.
    float timeLerped = 0.0f;
    float speed = 5;
    public Vector2 startPoint;
    public Vector2 nextPoint;
	// Use this for initialization
	void Start () {
        transform.position = DrawLine.waypoints[0];
        nextWayPoint++;
	}
	
	// Update is called once per frame
	void Update () {
        if (startPoint == nextPoint) GetComponent<SpriteRenderer>().enabled = false;

        timeLerped += Time.deltaTime;
        //transform.position = Vector2.Lerp(DrawLine.waypoints[nextWayPoint - 1], DrawLine.waypoints[nextWayPoint], timeLerped / timeToLerp);
        //    transform.LookAt( DrawLine.waypoints[nextWayPoint], Vector3.right);
        //    transform.rotation = Quaternion.identity;
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
        if ((Vector2)transform.position == nextPoint)
        {
                nextWayPoint = 0;
                transform.position = startPoint;
            
        }
	}
}
