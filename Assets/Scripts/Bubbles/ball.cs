using UnityEngine;
using System.Collections;
using System.Threading;
using InitScriptName;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ball : MonoBehaviour
{
    public Sprite[] sprites;
    public Sprite[] boosts;
    public bool isTarget;

    //	 public OTSprite sprite;                    //star's sprite class
    Vector2 speed =                     // 每秒星星的移动速度
        new Vector2( 250, 250 );
    public Vector3 target;
    Vector2 worldPos;
    Vector3 forceVect;
    public bool setTarget;
    public float startTime;
    float duration = 1.0f;
    public GameObject mesh;
    Vector2[] meshArray;
    public bool findMesh;
    Vector3 dropTarget;
    float row;
    string str;
    public bool newBall;
    float mTouchOffsetX;
    float mTouchOffsetY;
    float xOffset;
    float yOffset;
    public Vector3 targetPosition;
    bool stopedBall;
    private bool destroyed;
    public bool NotSorting;
    ArrayList fireballArray = new ArrayList();

    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            if( value )
            {
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;

            }
            destroyed = value;
        }
    }
    public ArrayList nearBalls = new ArrayList();
    //	private OTSpriteBatch spriteBatch = null;  
    GameObject Meshes;
    public int countNEarBalls;
    float bottomBorder;
    float topBorder;
    float leftBorder;
    float rightBorder;
    float gameOverBorder;
    bool gameOver;
    bool isPaused;
    public AudioClip swish;
    public AudioClip pops;
    public AudioClip join;
    Vector3 meshPos;
    bool dropedDown;
    bool rayTarget;
    RaycastHit2D[] bugHits;
    RaycastHit2D[] bugHits2;
    RaycastHit2D[] bugHits3;
    public bool falling;
    Animation rabbit;
    private int HitBug;
    private bool fireBall;
    private static int fireworks;
    private bool touchedTop;
    private bool touchedSide;
    /// <summary>
    /// 火球消除的移动限制
    /// </summary>
    private int fireBallLimit = 10;
    private bool launched;
    private bool animStarted;

    public int HitBug1
    {
        get { return HitBug; }
        set
        {
            if( value < 3 )
                HitBug = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        rabbit = GameObject.Find( "Rabbit" ).gameObject.GetComponent<Animation>();
        meshPos = new Vector3( -1000, -1000, -10 );
        //  sprite = GetComponent<OTSprite>();
        //sprite.passive = true;
        //	sprite.onCollision = OnCollision;
        dropTarget = transform.position;
        //		spriteBatch = GameObject.Find("SpriteBatch").GetComponent<OTSpriteBatch>();    
        Meshes = GameObject.Find( "-Ball" );
        // Add the custom tile action controller to this tile
        //      sprite.AddController(new MyActions(this));  

        bottomBorder = Camera.main.GetComponent<MainScript>().bottomBorder;
        topBorder = Camera.main.GetComponent<MainScript>().topBorder;
        leftBorder = Camera.main.GetComponent<MainScript>().leftBorder;
        rightBorder = Camera.main.GetComponent<MainScript>().rightBorder;
        gameOverBorder = Camera.main.GetComponent<MainScript>().gameOverBorder;
        gameOver = Camera.main.GetComponent<MainScript>().gameOver;
        isPaused = Camera.main.GetComponent<MainScript>().isPaused;
        dropedDown = Camera.main.GetComponent<MainScript>().dropingDown;
    }

    IEnumerator AllowLaunchBall()
    {
        yield return new WaitForSeconds( 2 );
        MainScript.StopControl = false;

    }

    public void PushBallAFterWin()
    {
		GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );

        setTarget = true;
        startTime = Time.time;
        target = Vector3.zero;
        Invoke( "StartFall", 0.4f );
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetMouseButtonUp( 0 ) )
        {
            GameObject ball = gameObject;
            if( !ClickOnGUI(Input.mousePosition) && !launched && !ball.GetComponent<ball>().setTarget && MainScript.Instance.newBall2 == null /*&& MainScript.Instance.newBall == null*/ && newBall && !Camera.main.GetComponent<MainScript>().gameOver && ( GamePlay.Instance.GameStatus == GameState.Playing || GamePlay.Instance.GameStatus == GameState.WaitForChicken ) )
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
                worldPos = pos;
                if( worldPos.y > -1.5f && !MainScript.StopControl )
                {
                    launched = true;
                    rabbit.Play( "rabbit_move" );
                    SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot( SoundBase.GetInstance().swish[0] );
                    mTouchOffsetX = ( worldPos.x - ball.transform.position.x ); //+ MathUtils.random(-10, 10);
                    mTouchOffsetY = ( worldPos.y - ball.transform.position.y );
                    xOffset = (float)Mathf.Cos( Mathf.Atan2( mTouchOffsetY, mTouchOffsetX ) );
                    yOffset = (float)Mathf.Sin( Mathf.Atan2( mTouchOffsetY, mTouchOffsetX ) );
                    speed = new Vector2( xOffset, yOffset );
                    if(!fireBall)
                        GetComponent<CircleCollider2D>().enabled = true;
                    target = worldPos;

                    setTarget = true;
                    startTime = Time.time;
                    dropTarget = transform.position;
                    InitScript.Instance.BoostActivated = false;
                    MainScript.Instance.newBall = gameObject;
                    MainScript.Instance.newBall2 = gameObject;
                    GetComponent<Rigidbody2D>().AddForce( target - dropTarget, ForceMode2D.Force );

                    //Debug.DrawLine( DrawLine.waypoints[0], target );
                    //Debug.Break();
                }
            }
        }
        if( transform.position != target && setTarget && !stopedBall && !isPaused && Camera.main.GetComponent<MainScript>().dropDownTime < Time.time )
        {
            float totalVelocity = Vector3.Magnitude( GetComponent<Rigidbody2D>().velocity );
            if( totalVelocity > 20 )
            {
                float tooHard = totalVelocity / ( 20 );
                GetComponent<Rigidbody2D>().velocity /= tooHard;

            }
            else if( totalVelocity < 15 )
            {
                float tooSlowRate = totalVelocity / ( 15 );
                if( tooSlowRate != 0 )
                    GetComponent<Rigidbody2D>().velocity /= tooSlowRate;


            }

            if( GetComponent<Rigidbody2D>().velocity.y < 1.5f && GetComponent<Rigidbody2D>().velocity.y > 0 ) GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x, 1.7f );
        }
        if( setTarget )
            triggerEnter();

		if( (transform.position.y <= -10 || transform.position.y >= 5) && fireBall && !Destroyed  )
		{
			MainScript.Instance.CheckFreeChicken();
			setTarget = false;
			launched = false;
			DestroySingle(gameObject, 0.00001f);
			MainScript.Instance.checkBall = gameObject;
		}
    }

    bool ClickOnGUI(Vector3 mousePos )
    {
        UnityEngine.EventSystems.EventSystem ct
           = UnityEngine.EventSystems.EventSystem.current;


        if ( ct.IsPointerOverGameObject())
            return true;
        return false;
    }

    public void SetBoost( BoostType boostType )
    {
        tag = "Ball";
        GetComponent<SpriteRenderer>().sprite = boosts[(int)boostType - 1];
        if( boostType == BoostType.ColorBallBoost )
        {
        }
        if( boostType == BoostType.FireBallBoost )
        {
            GetComponent<SpriteRenderer>().sortingOrder = 10;
            GetComponent<CircleCollider2D>().enabled = false; 
            fireBall = true;
            fireballArray.Add( gameObject );
        }
    }

    void FixedUpdate()
    {
        if( Camera.main.GetComponent<MainScript>().gameOver ) return;

        if( stopedBall )
        {

            transform.position = meshPos;
            stopedBall = false;
            if( newBall )
            {
                newBall = false;
                gameObject.layer = 9;
                Camera.main.GetComponent<MainScript>().checkBall = gameObject;
                this.enabled = false;
            }

        }

    }

    public GameObject findInArrayGameObject( ArrayList b, GameObject destObj )
    {
        foreach( GameObject obj in b )
        {

            if( obj == destObj ) return obj;
        }
        return null;
    }


    public bool findInArray( ArrayList b, GameObject destObj )
    {
        foreach( GameObject obj in b )
        {

            if( obj == destObj ) return true;
        }
        return false;
    }

    public ArrayList addFrom( ArrayList b, ArrayList b2 )
    {
        foreach( GameObject obj in b )
        {
            if( !findInArray( b2, obj ) )
            {
                b2.Add( obj );
            }
        }
        return b2;
    }

    public void changeNearestColor()
    {
        GameObject gm = GameObject.Find( "Creator" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, 1 << 9 );
        foreach( Collider2D obj in fixedBalls )
        {
            gm.GetComponent<creatorBall>().createBall( obj.transform.position );
            Destroy( obj.gameObject );
        }

    }


    public void checkNextNearestColor( ArrayList b, int counter )
    {
        //		Debug.Log(b.Count);
        Vector3 distEtalon = transform.localScale;
        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] meshes = Physics2D.OverlapCircleAll( transform.position, 1.0f, layerMask );
        foreach( Collider2D obj1 in meshes )
        {
            if( obj1.gameObject.tag == tag )
            {
                GameObject obj = obj1.gameObject;
                float distTemp = Vector3.Distance( transform.position, obj.transform.position );
                if( distTemp <= 1.0f )
                {
                    if( !findInArray( b, obj ) )
                    {
                        counter++;
                        b.Add( obj );
                        obj.GetComponent<bouncer>().checkNextNearestColor( b, counter );
                        //		destroy();
                        //obj.GetComponent<mesh>().checkNextNearestColor();
                        //		obj.GetComponent<mesh>().destroy();
                    }
                }
            }
        }
    }

    public void checkNearestColor()
    {
        int counter = 0;
        GameObject[] fixedBalls = GameObject.FindObjectsOfType( typeof( GameObject ) ) as GameObject[];			// change color tag of the rainbow
        foreach( GameObject obj in fixedBalls )
        {
            if( obj.layer == 9 && ( obj.name.IndexOf( "Rainbow" ) > -1 ) )
            {
                obj.tag = tag;
            }
        }

        ArrayList b = new ArrayList();
        b.Add( gameObject );
        Vector3 distEtalon = transform.localScale;
        GameObject[] meshes = GameObject.FindGameObjectsWithTag( tag );
        foreach( GameObject obj in meshes )
        {    													// detect the same color balls
            float distTemp = Vector3.Distance( transform.position, obj.transform.position );
            if( distTemp <= 0.9f && distTemp > 0 )
            {
                b.Add( obj );
                obj.GetComponent<bouncer>().checkNextNearestColor( b, counter );
            }
        }
        MainScript.Instance.countOfPreparedToDestroy = b.Count;
        if( b.Count >= 3 )
        {
            MainScript.Instance.ComboCount++;
            destroy( b, 0.00001f );
            MainScript.Instance.CheckFreeChicken();
        }
        if( b.Count < 3 )
        {
            Camera.main.GetComponent<MainScript>().bounceCounter++;
            MainScript.Instance.ComboCount = 0;
        }

        b.Clear();
        Camera.main.GetComponent<MainScript>().dropingDown = false;
           FindLight( gameObject );

    }


    public void StartFall()
    {
        enabled = false;

        if(mesh != null)
            mesh.GetComponent<Grid>().Busy = null;
        if( gameObject == null ) return;
        if( LevelData.mode == ModeGame.Vertical && isTarget )
        {
            Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
        }
        else if( LevelData.mode == ModeGame.Animals && isTarget )
        {
            StartCoroutine( FlyToTarget() );
        }
        setTarget = false;
        transform.SetParent( null );
        gameObject.layer = 13;
        gameObject.tag = "Ball";
        if( gameObject.GetComponent<Rigidbody2D>() == null ) gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        gameObject.GetComponent<Rigidbody2D>().fixedAngle = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity + new Vector2( Random.Range( -2, 2 ), 0 );
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        gameObject.GetComponent<CircleCollider2D>().radius = 0.3f;
        GetComponent<ball>().falling = true;

    }

    IEnumerator FlyToTarget()
    {
        Vector3 targetPos = new Vector3( 2.3f, 6, 0 );
        if(MainScript.Instance.TargetCounter1 < MainScript.Instance.TotalTargets)
            MainScript.Instance.TargetCounter1++;

        AnimationCurve curveX = new AnimationCurve( new Keyframe( 0, transform.position.x ), new Keyframe( 0.5f, targetPos.x ) );
        AnimationCurve curveY = new AnimationCurve( new Keyframe( 0, transform.position.y ), new Keyframe( 0.5f, targetPos.y ) );
        curveY.AddKey( 0.2f, transform.position.y - 1 );
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = 0.2f;
        float distCovered = 0;
        while( distCovered < 0.6f )
        {
            distCovered = ( Time.time - startTime );
            transform.position = new Vector3( curveX.Evaluate( distCovered ), curveY.Evaluate( distCovered ), 0 );
            transform.Rotate( Vector3.back * 10 );
            yield return new WaitForEndOfFrame();
        }
        Destroy( gameObject );

    }

    public bool checkNearestBall( ArrayList b )
    {
        if(( MainScript.Instance.TopBorder.transform.position.y - transform.position.y <= 0.5f && LevelData.mode != ModeGame.Rounded ) || ( LevelData.mode == ModeGame.Rounded && tag == "chicken" ) )
        {
            Camera.main.GetComponent<MainScript>().controlArray = addFrom( b, Camera.main.GetComponent<MainScript>().controlArray );
            b.Clear();
            return true;    /// don't destroy
        }
        if( findInArray( Camera.main.GetComponent<MainScript>().controlArray, gameObject ) ) { b.Clear(); return true; } /// don't destroy
        b.Add( gameObject );
        foreach( GameObject obj in nearBalls )
        {
            if( obj != gameObject && obj != null )
            {
                if( obj.gameObject.layer == 9 )
                {
                    //if(findInArray(Camera.main.GetComponent<MainScript>().controlArray, obj.gameObject)){b.Clear(); return true;} /// don't destroy
                    //else{
                    float distTemp = Vector3.Distance( transform.position, obj.transform.position );
                    if( distTemp <= 0.9f && distTemp > 0 )
                    {
                        if( !findInArray( b, obj.gameObject ) )
                        {
                            //print( gameObject + " " + distTemp );
                            Camera.main.GetComponent<MainScript>().arraycounter++;
                            if( obj.GetComponent<ball>().checkNearestBall( b ) )
                                return true;
                        }
                    }
                    //}
                }
            }
        }
        return false;

    }

    public void connectNearBalls()
    {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, layerMask );
        nearBalls.Clear();

        foreach( Collider2D obj in fixedBalls )
        {
            if( nearBalls.Count <= 7 )
            {
                    nearBalls.Add( obj.gameObject );
            }
        }
        countNEarBalls = nearBalls.Count;
    }

    IEnumerator pullToMesh( Transform otherBall = null )
    {
        //	AudioSource.PlayClipAtPoint(join, new Vector3(5, 1, 2));
        GameObject busyMesh = null;
        float searchRadius = 0.2f;
        while( findMesh )
        {
            Vector3 centerPoint = transform.position;
            Collider2D[] fixedBalls1 = Physics2D.OverlapCircleAll( centerPoint, 0.1f, 1 << 10 );  //meshes

                foreach( Collider2D obj1 in fixedBalls1 )
                {
                    if( obj1.gameObject.GetComponent<Grid>() == null ) DestroySingle( gameObject, 0.00001f );
                    else if( obj1.gameObject.GetComponent<Grid>().Busy == null )
                    {
                        findMesh = false;
                        stopedBall = true;
                        if( meshPos.y <= obj1.gameObject.transform.position.y )
                        {
                            meshPos = obj1.gameObject.transform.position;
                            busyMesh = obj1.gameObject;
                        }
                    }
                }
                if( findMesh )
                {
                    Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( centerPoint, searchRadius, 1 << 10 );  //meshes
                    foreach( Collider2D obj in fixedBalls )
                    {
                        if( obj.gameObject.GetComponent<Grid>() == null ) DestroySingle( gameObject, 0.00001f );
                        else if( obj.gameObject.GetComponent<Grid>().Busy == null )
                        {
                            findMesh = false;
                            stopedBall = true;


                            if( meshPos.y <= obj.gameObject.transform.position.y )
                            {
                                meshPos = obj.gameObject.transform.position;
                                busyMesh = obj.gameObject;
                            }

                            //yield return new WaitForSeconds(1f/10f);
                        }
                    }
                }
                if( busyMesh != null )
                {
                    busyMesh.GetComponent<Grid>().Busy = gameObject;
                    gameObject.GetComponent<bouncer>().offset = busyMesh.GetComponent<Grid>().offset;
                    if( LevelData.mode == ModeGame.Rounded )
                        LockLevelRounded.Instance.Rotate( target, transform.position );

                }
                transform.parent = Meshes.transform;
                Destroy( GetComponent<Rigidbody2D>() );
                //  rigidbody2D.isKinematic = true;
                yield return new WaitForFixedUpdate();
                // StopCoroutine( "pullToMesh" );
                dropTarget = transform.position;

                if( findMesh ) searchRadius += 0.2f;

            yield return new WaitForFixedUpdate();
        }
        MainScript.Instance.connectNearBallsGlobal();
     //   FindLight( gameObject );

        if( busyMesh != null )
        {
            Hashtable animTable = MainScript.Instance.animTable;
            animTable.Clear();
            PlayHitAnim( transform.position, animTable );
        }
        creatorBall.Instance.OffGridColliders();

        yield return new WaitForSeconds( 0.5f );

        // StartCoroutine( MainScript.Instance.destroyAloneBall() );
    }

    public void PlayHitAnim( Vector3 newBallPos, Hashtable animTable )
    {

        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, layerMask );
        float force = 0.15f;
        foreach( Collider2D obj in fixedBalls )
        {
            if( !animTable.ContainsKey( obj.gameObject ) && obj.gameObject != gameObject && animTable.Count < 50 )
                obj.GetComponent<ball>().PlayHitAnimCorStart( newBallPos, force, animTable );
        }
        if( fixedBalls.Length > 0 && !animTable.ContainsKey( gameObject ) )
            PlayHitAnimCorStart( fixedBalls[0].gameObject.transform.position, 0, animTable );
    }

    public void PlayHitAnimCorStart( Vector3 newBallPos, float force, Hashtable animTable )
    {
        if( !animStarted )
        {
            StartCoroutine( PlayHitAnimCor( newBallPos, force, animTable ) );
            PlayHitAnim( newBallPos, animTable );
        }
    }

    public IEnumerator PlayHitAnimCor( Vector3 newBallPos, float force, Hashtable animTable )
    {
        animStarted = true;
        animTable.Add( gameObject, gameObject );
        if( tag == "chicken" ) yield break;
        yield return new WaitForFixedUpdate();
        float dist = Vector3.Distance( transform.position, newBallPos );
        force = 1 / dist + force;
        newBallPos = transform.position - newBallPos;
        if( transform.parent == null )
        {
            animStarted = false;
            yield break;
        }
        newBallPos = Quaternion.AngleAxis( transform.parent.parent.rotation.eulerAngles.z, Vector3.back ) * newBallPos;
        newBallPos = newBallPos.normalized;
        newBallPos = transform.localPosition + ( newBallPos * force / 10 );

        float startTime = Time.time;
        Vector3 startPos = transform.localPosition;
        float speed = force * 5;
        float distCovered = 0;
        while( distCovered < 1 && !float.IsNaN( newBallPos.x ) )
        {
            distCovered = ( Time.time - startTime ) * speed;
            if( this == null ) yield break;
            //   if( destroyed ) yield break;
            if( falling )
            {
     //           transform.localPosition = startPos;
                yield break;
            }
            transform.localPosition = Vector3.Lerp( startPos, newBallPos, distCovered );
            yield return new WaitForEndOfFrame();
        }
        Vector3 lastPos = transform.localPosition;
        startTime = Time.time;
        distCovered = 0;
        while( distCovered < 1 && !float.IsNaN( newBallPos.x ) )
        {
            distCovered = ( Time.time - startTime ) * speed;
            if( this == null ) yield break;
            if( falling )
            {
          //      transform.localPosition = startPos;
                yield break;
            }
            transform.localPosition = Vector3.Lerp( lastPos, startPos, distCovered );
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = startPos;
        animStarted = false;
    }
    void OnTriggerStay2D( Collider2D other )
    {
        if( findMesh && other.gameObject.layer == 9 )
        {
            //	StartCoroutine(pullToMesh());
        }
    }

    public void FindLight(GameObject activatedByBall)
    {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( transform.position, 0.5f, layerMask );
        int i = 0;
        foreach( Collider2D obj in fixedBalls )
        {
            i++;
            if( i <= 10 )
            {
                if( ( obj.gameObject.tag == "light"  ) && GamePlay.Instance.GameStatus == GameState.Playing ) {
                    DestroySingle( obj.gameObject );
                    DestroySingle( activatedByBall );
                }
                else if( ( obj.gameObject.tag == "cloud"  ) && GamePlay.Instance.GameStatus == GameState.Playing ) 
                {
                    obj.GetComponent<ColorBallScript>().ChangeRandomColor();
                }

            }
        }
    }


    void OnCollisionEnter2D( Collision2D coll )
    {
        OnTriggerEnter2D( coll.collider );
    }

    void OnTriggerEnter2D( Collider2D other )
    {
        // stop
        if( other.gameObject.name.Contains( "ball" ) && setTarget && name.IndexOf( "bug" ) < 0 )
        {
            if( !other.gameObject.GetComponent<ball>().enabled )
            {
                if( ( other.gameObject.tag == "black_hole" ) && GamePlay.Instance.GameStatus == GameState.Playing )
                {
                    SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot( SoundBase.GetInstance().black_hole );
                    DestroySingle( gameObject );
                }

                if( !fireBall )
                    StopBall( true, other.transform );
                else
                {
					if( other.gameObject.tag.Contains( "animal" ) || other.gameObject.tag.Contains( "empty" ) || other.gameObject.tag.Contains( "chicken" ) ) return;
                    fireBallLimit--;
                    if( fireBallLimit > 0)
                        DestroySingle( other.gameObject, 0.000000000001f );
                    else
                    {
                        StopBall();
                        destroy( fireballArray, 0.000000000001f );

                    }


                }
      //           FindLight(gameObject);
           }
            //          }
        }
        else if( other.gameObject.name.IndexOf( "ball" ) == 0 && setTarget && name.IndexOf( "bug" ) == 0 )
        {
            if( other.gameObject.tag == gameObject.tag )
            {
                Destroy( other.gameObject );
                //                Score.Instance.addScore(3);
            }
        }

        else if( other.gameObject.name == "TopBorder" && setTarget )
        {
            if( LevelData.mode == ModeGame.Vertical || LevelData.mode == ModeGame.Animals )
            {
                if( !findMesh )
                {
                    transform.position = new Vector3( transform.position.x, transform.position.y, transform.position.z );
                    StopBall();

                    if( fireBall )
                    {
                        destroy( fireballArray, 0.000000000001f );
                    }
                }

            }
        }

    }

    void StopBall( bool pulltoMesh = true, Transform otherBall = null )
    {
        launched = true;
        MainScript.lastBall = gameObject.transform.position;
        creatorBall.Instance.EnableGridColliders();
        target = Vector2.zero;
        setTarget = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        findMesh = true;
		GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );

        if( GetComponent<SpriteRenderer>().sprite == boosts[0] )  //color ball boost
        {
            DestroyAround();
        }
        if( pulltoMesh )
            StartCoroutine( pullToMesh( otherBall ) );

    }


    void DestroyAround()
    {
        ArrayList b = new ArrayList();
        b.Add( gameObject );
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] meshes = Physics2D.OverlapCircleAll( transform.position, 1f, layerMask );
        foreach( Collider2D obj1 in meshes )
        {
            GameObject obj = obj1.gameObject;
            if( !findInArray( b, obj ) && obj.tag != "chicken" && !obj.tag.Contains( "animal" ) && !obj.tag.Contains( "empty" ) )
            {
                b.Add( obj );
            }
        }
        if( b.Count >= 0 )
        {
            MainScript.Instance.ComboCount++;
            destroy( b, 0.001f );
        }

    }

    void DestroyLine()
    {

        ArrayList b = new ArrayList();
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        RaycastHit2D[] fixedBalls = Physics2D.LinecastAll( transform.position + Vector3.left * 10, transform.position + Vector3.right * 10, layerMask );
        int i = 0;
        foreach( RaycastHit2D item in fixedBalls )
        {
                if( !findInArray( b, item.collider.gameObject ) )
                {
                    b.Add( item.collider.gameObject );
                }
        }


        if( b.Count >= 0 )
        {
            MainScript.Instance.ComboCount++;
            MainScript.Instance.destroy( b);
        }

        MainScript.Instance.StartCoroutine( MainScript.Instance.destroyAloneBall() );
    }


    public void CheckBallCrossedBorder()
    {
        if( Physics2D.OverlapCircle( transform.position, 0.1f, 1 << 14 ) != null || Physics2D.OverlapCircle( transform.position, 0.1f, 1 << 17 ) != null )
        {
            DestroySingle( gameObject, 0.00001f );
        }

    }

    void triggerEnter()
    {

        // check if we collided with a bottom block and adjust our speed and rotation accordingly
        if( transform.position.y <= bottomBorder && target.y < 0 )
        {
            growUp();
            StopBall( false );
            //target = new Vector2( target.x, target.y * -1 );
        }
        else
        {

            //// check if we collided with a left block and adjust our speed and rotation accordingly
            if( transform.position.x <= leftBorder && target.x < 0 && !touchedSide && fireBall )
            {
                //  touchedSide = true;
                Invoke( "CanceltouchedSide", 0.1f );
                 target = new Vector2( target.x * -1, target.y );
                GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x * -1, GetComponent<Rigidbody2D>().velocity.y );
            }
            // check if we collided with a right block and adjust our speed and rotation accordingly
            if( transform.position.x >= rightBorder && target.x > 0 && !touchedSide && fireBall )
            {
                //  touchedSide = true;
                Invoke( "CanceltouchedSide", 0.1f );
                 target = new Vector2( target.x * -1, target.y );
                GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x * -1, GetComponent<Rigidbody2D>().velocity.y );
            }
//             check if we collided with a right block and adjust our speed and rotation accordingly
            if( transform.position.y >= topBorder && target.y > 0 && LevelData.mode == ModeGame.Rounded && !touchedTop )
            {
                touchedTop = true;
                // target = new Vector2( target.x, -target.y );
                GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y * -1 );
                //         print( target.y );
            }

        }



    }

    void CanceltouchedSide()
    {
        touchedSide = false;

    }

    public void destroy( ArrayList b, float speed = 0.1f )
    {
        StartCoroutine( DestroyCor( b, speed ) );
    }

    IEnumerator DestroyCor( ArrayList b, float speed = 0.1f )
    {
        ArrayList l = new ArrayList();
        foreach( GameObject obj in b )
        {
            l.Add( obj );
        }

        Camera.main.GetComponent<MainScript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;
        foreach( GameObject obj in l )
        {
            if( obj == null ) continue;
            if( obj.name.IndexOf( "ball" ) == 0 ) obj.layer = 0;
            //GameObject obj2 = findInArrayGameObject( b, obj );
            //if(obj2 != null)
            obj.GetComponent<ball>().growUp();
            soundPool++;
            GetComponent<Collider2D>().enabled = false;
            if( scoreCounter > 3 )
            {
                rate += 10;
                scoreCounter += rate;
            }
            scoreCounter += 10;
            if( b.Count > 10 && Random.Range( 0, 10 ) > 5 ) MainScript.Instance.perfect.SetActive( true );
            obj.GetComponent<ball>().Destroyed = true;
            //		Destroy(obj);

            //  Camera.main.GetComponent<MainScript>().explode( obj.gameObject );
            if(b.Count<10 || soundPool % 20 == 0)
                yield return new WaitForSeconds( speed );

            //			Destroy(obj);
        }
        //if (name.IndexOf("bug") < 0)
        //    Score.Instance.addScore(scoreCounter);
        MainScript.Instance.PopupScore( scoreCounter, transform.position );
     //   StartCoroutine( MainScript.Instance.destroyAloneBall() );

    }

    void DestroySingle( GameObject obj, float speed = 0.1f )
    {
        Camera.main.GetComponent<MainScript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;
        if( obj.name.IndexOf( "ball" ) == 0 ) obj.layer = 0;
        obj.GetComponent<ball>().growUp();
        soundPool++;

        if( obj.tag == "light" )
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot( SoundBase.GetInstance().spark );
            obj.GetComponent<ball>().DestroyLine();
        }

        if( scoreCounter > 3 )
        {
            rate += 10;
            scoreCounter += rate;
        }
        scoreCounter += 10;
        obj.GetComponent<ball>().Destroyed = true;
        MainScript.Instance.PopupScore( scoreCounter, transform.position );

    }

    public void SplashDestroy()
    {
        if (setTarget) MainScript.Instance.newBall2 = null;
        Destroy( gameObject );
    }

    public void destroy()
    {
        growUpPlaySound();
        destroy( gameObject );
    }

    public void destroy( GameObject obj )
    {
        if( obj.name.IndexOf( "ball" ) == 0 ) obj.layer = 0;

        Camera.main.GetComponent<MainScript>().bounceCounter = 0;
        //	collider.enabled = false;
        obj.GetComponent<ball>().destroyed = true;
        //	Destroy(obj);
        //obj.GetComponent<ball>().growUpPlaySound();
        obj.GetComponent<ball>().growUp();
        //	Invoke("playPop",1/(float)Random.Range(2,10));
        Camera.main.GetComponent<MainScript>().explode( obj.gameObject );
        //     if (name.IndexOf("bug") < 0)
        //       Score.Instance.addScore(3);

    }

    public void growUp()
    {
        StartCoroutine( explode() );
    }

    public void growUpPlaySound()
    {
        Invoke( "growUpDelayed", 1 / (float)Random.Range( 2, 10 ) );
    }

    public void growUpDelayed()
    {
        StartCoroutine( explode() );
    }

    void playPop()
    {
    }


    IEnumerator explode()
    {

        float startTime = Time.time;
        float endTime = Time.time + 0.1f;
        Vector3 tempPosition = transform.localScale;
        Vector3 targetPrepare = transform.localScale * 1.2f;

        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;


        while( !isPaused && endTime > Time.time )
        {
            //transform.position  += targetPrepare * Time.deltaTime;
            transform.localScale = Vector3.Lerp( tempPosition, targetPrepare, ( Time.time - startTime ) * 10 );
            //	transform.position  = targetPrepare ;
            yield return new WaitForEndOfFrame();
        }
        //      yield return new WaitForSeconds(0.01f );
        GameObject prefab = Resources.Load( "Particles/BubbleExplosion" ) as GameObject;

        GameObject explosion = (GameObject)Instantiate( prefab, gameObject.transform.position + Vector3.back * 20f, Quaternion.identity );
        if( mesh != null )
            explosion.transform.parent = mesh.transform;
        //   if( !isPaused )
        CheckNearCloud();

        if( LevelData.mode == ModeGame.Vertical && isTarget )
        {
            Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
        }
        else if( LevelData.mode == ModeGame.Animals && isTarget )
        {
           // Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
        }
        Destroy( gameObject, 1 );

    }
    void CheckNearCloud()
    {
        int layerMask = 1 << LayerMask.NameToLayer( "Ball" );
        Collider2D[] meshes = Physics2D.OverlapCircleAll( transform.position, 1f, layerMask );
        foreach( Collider2D obj1 in meshes )
        {
            if( obj1.gameObject.tag == "cloud" )
            {
                GameObject obj = obj1.gameObject;
                float distTemp = Vector3.Distance( transform.position, obj.transform.position );
                if( distTemp <= 1f )
                {
                    obj.GetComponent<ColorBallScript>().ChangeRandomColor();
                }
            }
        }

    }

    public void ShowFirework()
    {
        fireworks++;
        if( fireworks <= 2 )
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot( SoundBase.GetInstance().hit );

    }




}
