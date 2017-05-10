using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private GameObject busy;

    public GameObject Busy
    {
        get { return busy; }
        set
        {
            if( value != null )
            {
                if( value.GetComponent<ball>() != null )
                {
                    if( !value.GetComponent<ball>().NotSorting  )
                    {

                //        value.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt( 1 / ( transform.position.y + 10 ) * 100 );
                        value.GetComponent<ball>().mesh = gameObject;
                        if( value.tag == "chicken" ) value.GetComponent<SpriteRenderer>().sortingOrder = 100;
                    }
                }

            }

            busy = value;
        }
    }

    GameObject[] meshes;
    bool destroyed;
    public float offset;
    bool triggerball;
    public GameObject boxFirst;
    public GameObject boxSecond;
    public static bool waitForAnim;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if( busy == null )
        {
            GameObject box = null;
            GameObject ball = null;
            if( name == "boxCatapult" && !Grid.waitForAnim )
            {
                box = boxSecond;
                ball = box.GetComponent<Grid>().busy;
                if( ball != null )
                {
                       ball.GetComponent<bouncer>().bounceToCatapult( transform.position );
                    //ball.GetComponent<ball>().newBall = true;

                    busy = ball;

                }
            }
            else if( name == "boxSecond" && !Grid.waitForAnim )
            {
                if( ( GamePlay.Instance.GameStatus == GameState.Playing || GamePlay.Instance.GameStatus == GameState.Win || GamePlay.Instance.GameStatus == GameState.WaitForChicken ) && LevelData.LimitAmount > 0 )
                {
                    busy = Camera.main.GetComponent<MainScript>().createFirstBall( transform.position );

                }
                //box = boxFirst;
                //ball = box.GetComponent<Grid>().busy;
                //if(ball != null){
                //    ball.GetComponent<bouncer>().bounceTo(transform.position);
                //    busy = ball;
                //}
            }
            else if( name == "boxFirst" && !Grid.waitForAnim )
            {
                //		busy = Camera.main.GetComponent<MainScript>().createFirstBall(transform.position);
            }

        }

        if( busy != null && !Grid.waitForAnim )
        {
            if( name == "boxCatapult" )
            {
                //if(Vector3.Distance(transform.position, busy.transform.position) > 1.6 )
                if( busy.GetComponent<ball>().setTarget )
                    busy = null;
            }
            else if( name == "boxFirst" )
            {
                if( Vector3.Distance( transform.position, busy.transform.position ) > 2 )
                    busy = null;
            }
            else if( name == "boxSecond" )
            {
                if( Vector3.Distance( transform.position, busy.transform.position ) > 0.9f )
                {
                    busy = null;
                }
            }
            /*			else{
                            if(Vector3.Distance(transform.position, busy.transform.position) > 25 )
                                busy = null;
                        }*/
        }
    }

    public void BounceFrom( GameObject box )
    {


        GameObject ball = box.GetComponent<Grid>().busy;
        if( ball != null && busy != null )
        {
            //		ball.GetComponent<bouncer>().bounceToCatapult(transform.position);
            busy.GetComponent<bouncer>().bounceTo( box.transform.position );
            box.GetComponent<Grid>().busy = busy;
            busy = ball;
        }
    }

    void setColorTag( GameObject ball )
    {
        if( ball.name.IndexOf( "Orange" ) > -1 )
        {
            ball.tag = "Fixed";
            //	tag = "Orange";
        }
        else if( ball.name.IndexOf( "Red" ) > -1 )
        {
            ball.tag = "Fixed";
            //	tag = "Red";
        }
        else if( ball.name.IndexOf( "Yellow" ) > -1 )
        {
            ball.tag = "Fixed";
            //	tag = "Yellow";
        }
    }

    /*void createBall(GameObject ball){
        setColorTag( ball);
        GameObject gm = GameObject.Find ("Creator");
        gm.GetComponent<creatorBall>().createBall();
    }*/

    void OnCollisionStay2D( Collision2D other )
    {
        if( other.gameObject.name.IndexOf( "ball" ) > -1 && busy == null )
        {

            busy = other.gameObject;
        }
    }

    //	void OnTriggerStay2D(Collider2D other){
    //		if(other.gameObject.name.IndexOf("ball")>-1 && busy == null) {
    //			busy = other.gameObject;
    //		}
    //	}

    void OnTriggerExit( Collider other )
    {
        //busy = null;
    }


    public void destroy()
    {
        tag = "Mesh";
        Destroy( busy );
        busy = null;
    }







}
