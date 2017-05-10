using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour
{
    LineRenderer line;
    bool draw = false;
    Color col;

    public static Vector2[] waypoints = new Vector2[3];
    public float addAngle = 90;
    public GameObject pointer;
    GameObject[] pointers = new GameObject[15];
    GameObject[] pointers2 = new GameObject[3];
    Vector3 lastMousePos;
    private bool startAnim;

    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRenderer>();
        GeneratePoints();
        GeneratePositionsPoints();
        HidePoints();
        waypoints[0] = transform.position;
        waypoints[1] = transform.position+Vector3.up*5;
    }

    void HidePoints()
    {
        foreach (GameObject item in pointers)
        {
            item.GetComponent<SpriteRenderer>().enabled = false;
        }

        foreach (GameObject item in pointers2)
        {
            item.GetComponent<SpriteRenderer>().enabled = false;
        }

    }

    private void GeneratePositionsPoints()
    {
        if (MainScript.Instance.boxCatapult.GetComponent<Grid>().Busy != null)
        {
            col = MainScript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<SpriteRenderer>().sprite.texture.GetPixelBilinear(0.6f, 0.6f);
            col.a = 1;
        }

        HidePoints();

        for (int i = 0; i < pointers.Length; i++)
        {
            Vector2 AB = waypoints[1] - waypoints[0];
            AB = AB.normalized;
            float step = i / 1.5f;

            if (step < (waypoints[1] - waypoints[0]).magnitude)
            {
                pointers[i].GetComponent<SpriteRenderer>().enabled = true;
                pointers[i].transform.position = waypoints[0] + (step * AB);
                pointers[i].GetComponent<SpriteRenderer>().color = col;
                pointers[i].GetComponent<LinePoint>().startPoint = pointers[i].transform.position;
                pointers[i].GetComponent<LinePoint>().nextPoint = pointers[i].transform.position;
                if (i > 0)
                    pointers[i - 1].GetComponent<LinePoint>().nextPoint = pointers[i].transform.position;
            }
        }
        for (int i = 0; i < pointers2.Length; i++)
        {
            Vector2 AB = waypoints[2] - waypoints[1];
            AB = AB.normalized;
            float step = i / 2f;

            if (step < (waypoints[2] - waypoints[1]).magnitude)
            {
                pointers2[i].GetComponent<SpriteRenderer>().enabled = true;
                pointers2[i].transform.position = waypoints[1] + (step * AB);
                pointers2[i].GetComponent<SpriteRenderer>().color = col;
                pointers2[i].GetComponent<LinePoint>().startPoint = pointers2[i].transform.position;
                pointers2[i].GetComponent<LinePoint>().nextPoint = pointers2[i].transform.position;
                if (i > 0)
                    pointers2[i - 1].GetComponent<LinePoint>().nextPoint = pointers2[i].transform.position;
            }
        }
    }

    void GeneratePoints()
    {
        for (int i = 0; i < pointers.Length; i++)
        {
            pointers[i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
            pointers[i].transform.parent = transform;
        }
        for (int i = 0; i < pointers2.Length; i++)
        {
            pointers2[i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
            pointers2[i].transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            draw = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            draw = false;
        }

        if (draw)
        {
            //  line.enabled = true;
            Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.back * 10;
       //     if( dir.y - 2 < transform.position.y ) { HidePoints(); return; }
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            if( !MainScript.StopControl )
            {//dir.y < 15.5 && dir.y > - 2 && 

                dir.z = 0;
                if (lastMousePos == dir)
                {
                    startAnim = true;
                }
                else startAnim = false;
                lastMousePos = dir;
                line.SetPosition(0, transform.position);

                waypoints[0] = transform.position;
                //int layerMask = ~(1 << LayerMask.NameToLayer("Mesh"));


                RaycastHit2D[] hit = Physics2D.LinecastAll( waypoints[0], waypoints[0] + ( (Vector2)dir - waypoints[0] ).normalized * 10 );
                foreach (RaycastHit2D item in hit)
                {
                    Vector2 point = item.point;
                //    if (point.y - waypoints[0].y < 1.5f) point += Vector2.up * 5;
                    line.SetPosition(1, point);
                    addAngle = 180;

                       if (waypoints[1].x < 0) addAngle = 0;
                       if( item.collider.gameObject.layer == LayerMask.NameToLayer( "Border" ) && item.collider.gameObject.name != "GameOverBorder" && item.collider.gameObject.name != "borderForRoundedLevels" )
                        {
                           Debug.DrawLine( waypoints[0], waypoints[1], Color.red );  //waypoints[0] + ( (Vector2)dir - waypoints[0] ).normalized * 10
                           Debug.DrawLine( waypoints[0], dir, Color.blue );
                           Debug.DrawRay( waypoints[0], waypoints[1] - waypoints[0], Color.green );
                           waypoints[1] = point;
                           waypoints[2] = point;
                           line.SetPosition( 1, dir );
                            waypoints[1] = point;
                            float angle = 0;
                            angle = Vector2.Angle(waypoints[0] - waypoints[1], (point - Vector2.up * 100) - (Vector2)point);
                            if (waypoints[1].x > 0) angle = Vector2.Angle(waypoints[0] - waypoints[1], (Vector2)point - (point - Vector2.up * 100));
                            waypoints[2] = Quaternion.AngleAxis(angle + addAngle, Vector3.back) * ((Vector2)point - (point - Vector2.up * 100));
                            Vector2 AB = waypoints[2] - waypoints[1];
                            AB = AB.normalized;
                            line.SetPosition(2, waypoints[2]);
                            break;
                        }
                        else if (item.collider.gameObject.layer == LayerMask.NameToLayer("Ball"))
                        {
                            Debug.DrawLine( waypoints[0], waypoints[1], Color.red );  //waypoints[0] + ( (Vector2)dir - waypoints[0] ).normalized * 10
                            Debug.DrawLine( waypoints[0], dir, Color.blue );
                            Debug.DrawRay( waypoints[0], waypoints[1] - waypoints[0], Color.green );
                            line.SetPosition( 1, point );
                            waypoints[1] = point;
                            waypoints[2] = point;
                            Vector2 AB = waypoints[2] - waypoints[1];
                            AB = AB.normalized;
                            line.SetPosition(2, waypoints[1] + (0.1f * AB));
                            break;
                        }
                        else
                        {

                            waypoints[1] = waypoints[0] + ( (Vector2)dir - waypoints[0] ).normalized * 10;
                            waypoints[2] = waypoints[0] + ( (Vector2)dir - waypoints[0] ).normalized * 10;
                        }

                    
 
                }
                if (!startAnim )
                    GeneratePositionsPoints();

            }

        }
        else if (!draw)
        {
            HidePoints();
        }

    }
}
