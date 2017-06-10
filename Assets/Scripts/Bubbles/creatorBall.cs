using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class creatorBall : MonoBehaviour
{
    public static creatorBall Instance;
    public GameObject ball_hd;
    public GameObject ball_ld;
    public GameObject bug_hd;
    public GameObject bug_ld;
    public GameObject thePrefab;
    GameObject ball;
    GameObject bug;
    string[] ballsForCatapult = new string[11];
    string[] ballsForMatrix = new string[11];
    string[] bugs = new string[11];
    public static int columns = 11;
    public static int rows = 70;
    public static List<Vector2> grid = new List<Vector2>();
    int lastRow;
    float offsetStep = 0.33f;
    //private OTSpriteBatch spriteBatch = null;  
    GameObject Meshes;
    [HideInInspector]
    public List<GameObject> squares = new List<GameObject>();
    int[] map;
    private int maxCols;
    private int maxRows;
    private LIMIT limitType;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        ball = ball_hd;
        bug = bug_hd;
        thePrefab.transform.localScale = new Vector3( 0.67f, 0.58f, 1 );
        Meshes = GameObject.Find( "-Ball" );
        // LevelData.LoadDataFromXML( MainScript.Instance.currentLevel );
        LoadLevel();
        //LevelData.LoadDataFromLocal(MainScript.Instance.currentLevel);
        if( LevelData.mode == ModeGame.Vertical || LevelData.mode == ModeGame.Animals )
            MoveLevelUp();
        else
        {
            // GameObject.Find( "TopBorder" ).transform.position += Vector3.down * 3.5f;
            GameObject.Find( "TopBorder" ).transform.parent = null;
            GameObject.Find( "TopBorder" ).GetComponent<SpriteRenderer>().enabled = false;
            GameObject ob = GameObject.Find( "-Meshes" );
            ob.transform.position += Vector3.up * 2f;
            LockLevelRounded slider = ob.AddComponent<LockLevelRounded>();
            GamePlay.Instance.GameStatus = GameState.PreTutorial;
        }
        createMesh();
        LoadMap( LevelData.map );
        Camera.main.GetComponent<MainScript>().connectNearBallsGlobal();
        StartCoroutine( getBallsForMesh() );
        ShowBugs();
    }

    public void LoadLevel()
    {
        MainScript.Instance.currentLevel = PlayerPrefs.GetInt("OpenLevel");// TargetHolder.level;
        if (MainScript.Instance.currentLevel == 0)
            MainScript.Instance.currentLevel = 1;
        LoadDataFromLocal(MainScript.Instance.currentLevel);

    }


    public bool LoadDataFromLocal(int currentLevel)
    {
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
        return true;
    }

    void ProcessGameDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        LevelData.colorsDict.Clear();
        int mapLine = 0;
        int key = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE "))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                LevelData.mode = (ModeGame)int.Parse(modeString);
            }
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                maxCols = int.Parse(sizes[0]);
                maxRows = int.Parse(sizes[1]);
            }
            else if (line.StartsWith("LIMIT "))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                limitType = (LIMIT)int.Parse(sizes[0]);
                LevelData.LimitAmount = int.Parse(sizes[1]);

            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                LevelData.colors = int.Parse(blocksString);
            }
            else if (line.StartsWith("STARS "))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                LevelData.star1 = int.Parse(blocksNumbers[0]);
                LevelData.star2 = int.Parse(blocksNumbers[1]);
                LevelData.star3 = int.Parse(blocksNumbers[2]);
            }
            else
            {   //Maps
                //lines分割，并获取行数
                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    int value =  int.Parse(st[i][0].ToString());
                    if (!LevelData.colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                    {
                        LevelData.colorsDict.Add(key, (BallColor)value);
                        key++;

                    }

                    LevelData.map[mapLine * maxCols + i] = int.Parse(st[i][0].ToString());
                }
                mapLine++;
            }
        }
        //随机颜色
        if (LevelData.colorsDict.Count == 0)
        {
            //添加固定颜色
            LevelData.colorsDict.Add(0, BallColor.yellow);
            LevelData.colorsDict.Add(1, BallColor.red);

            //添加随机颜色
            List<BallColor> randomList = new List<BallColor>();
            randomList.Add(BallColor.blue);
            randomList.Add(BallColor.green);
            //if (LevelData.mode != ModeGame.Rounded)
                randomList.Add(BallColor.violet);
            for (int i = 0; i < LevelData.colors - 2; i++)
            {
                BallColor randCol = BallColor.yellow;
                while (LevelData.colorsDict.ContainsValue(randCol))
                {
                    randCol = randomList[UnityEngine.Random.RandomRange(0, randomList.Count)];
                }
                LevelData.colorsDict.Add(2 + i, randCol);

            }

        }

    }

    public void LoadMap( int[] pMap )
    {
        map = pMap;
        int key = -1;
        int roww = 0;
        for( int i = 0; i < rows; i++ )
        {
            for( int j = 0; j < columns; j++ )
            {
                int mapValue = map[i * columns + j];
                if( mapValue > 0  )
                {
                    roww = i;
                    if (LevelData.mode == ModeGame.Rounded) roww = i +4;
                    createBall( GetSquare(roww, j ).transform.position, (BallColor)mapValue, false, i );
                }
                else if( mapValue == 0 && LevelData.mode == ModeGame.Vertical && i == 0 )
                {
                    Instantiate( Resources.Load( "Prefabs/TargetStar" ), GetSquare( i, j ).transform.position, Quaternion.identity );
                }
            }
        }
    }

    private void MoveLevelUp()
    {
        StartCoroutine( MoveUpDownCor() );
    }

    IEnumerator MoveUpDownCor( bool inGameCheck = false )
    {
        yield return new WaitForSeconds( 0.1f );
        if( !inGameCheck )
            GamePlay.Instance.GameStatus = GameState.BlockedGame;
        bool up = false;
        List<float> table = new List<float>();
        float lineY = -1.3f;//GameObject.Find( "GameOverBorder" ).transform.position.y;
        Transform bubbles = GameObject.Find( "-Ball" ).transform;
        int i = 0;
        foreach( Transform item in bubbles )
        {
            if( !inGameCheck )
            {
                if( item.position.y < lineY )
                {
                    table.Add( item.position.y );
                }
            }
            else if( !item.GetComponent<ball>().Destroyed )
            {
                if( item.position.y > lineY && MainScript.Instance.TopBorder.transform.position.y > 5f )
                {
                    table.Add( item.position.y );
                }
                else if( item.position.y < lineY + 1f )
                {
                    table.Add( item.position.y );
                    up = true;
                }
            }
            i++;
        }


        if( table.Count > 0 )
        {
            if( up ) AddMesh();

            float targetY = 0;
            table.Sort();
            if( !inGameCheck ) targetY = lineY - table[0] + 2.5f;
            else targetY = lineY - table[0] + 1.5f;
            GameObject Meshes = GameObject.Find( "-Meshes" );
            Vector3 targetPos = Meshes.transform.position + Vector3.up * targetY;
            float startTime = Time.time;
            Vector3 startPos = Meshes.transform.position;
            float speed = 0.5f;
            float distCovered = 0;
            while( distCovered < 1 )
            {
           //                     print( table.Count );
                speed += Time.deltaTime / 1.5f;
                distCovered = ( Time.time - startTime ) / speed;
                Meshes.transform.position = Vector3.Lerp( startPos, targetPos, distCovered );
                yield return new WaitForEndOfFrame();
                if( startPos.y > targetPos.y )
                {
                    if( MainScript.Instance.TopBorder.transform.position.y <= 5 && inGameCheck ) break;
                }
            }
        }

        //        Debug.Log("lift finished");
        if( GamePlay.Instance.GameStatus == GameState.BlockedGame )
            GamePlay.Instance.GameStatus = GameState.PreTutorial;
        else if( GamePlay.Instance.GameStatus != GameState.GameOver && GamePlay.Instance.GameStatus != GameState.Win )
            GamePlay.Instance.GameStatus = GameState.Playing;


    }

    public void MoveLevelDown()
    {
        StartCoroutine( MoveUpDownCor( true ) );
    }

    private bool BubbleBelowLine()
    {
        throw new System.NotImplementedException();
    }

    void ShowBugs()
    {
        int effset = 1;
        for( int i = 0; i < 2; i++ )
        {
            effset *= -1;
            CreateBug( new Vector3( 10 * effset, -3, 0 ) );

        }

    }

    public void CreateBug( Vector3 pos, int value = 1 )
    {
        Transform spiders = GameObject.Find( "Spiders" ).transform;
        List<Bug> listFreePlaces = new List<Bug>();
        foreach( Transform item in spiders )
        {
            if( item.childCount > 0 ) listFreePlaces.Add( item.GetChild( 0 ).GetComponent<Bug>() );
        }

        if( listFreePlaces.Count < 6 )
            Instantiate( bug, pos, Quaternion.identity );
        else
        {
            listFreePlaces.Clear();
            foreach( Transform item in spiders )
            {
                if( item.childCount > 0 )
                {
                    if( item.GetChild( 0 ).GetComponent<Bug>().color == 0 ) listFreePlaces.Add( item.GetChild( 0 ).GetComponent<Bug>() );
                }
            }
            if( listFreePlaces.Count > 0 )
                listFreePlaces[UnityEngine.Random.Range( 0, listFreePlaces.Count )].ChangeColor( 1 );
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator getBallsForMesh()
    {
        GameObject[] meshes = GameObject.FindGameObjectsWithTag( "Mesh" );
        foreach( GameObject obj1 in meshes )
        {
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll( obj1.transform.position, 0.2f, 1 << 9 );  //balls
            foreach( Collider2D obj in fixedBalls )
            {
                obj1.GetComponent<Grid>().Busy = obj.gameObject;
                //	obj.GetComponent<bouncer>().offset = obj1.GetComponent<Grid>().offset;
            }
        }
        yield return new WaitForSeconds( 0.5f );
    }

    public void EnableGridColliders()
    {
        foreach( GameObject item in squares )
        {
            item.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    public void OffGridColliders()
    {
        foreach( GameObject item in squares )
        {
            item.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void createRow( int j )
    {
        float offset = 0;
        GameObject gm = GameObject.Find( "Creator" );
        for( int i = 0; i < columns; i++ )
        {
            if( j % 2 == 0 ) offset = 0; else offset = offsetStep;
            Vector3 v = new Vector3( transform.position.x + i * thePrefab.transform.localScale.x + offset, transform.position.y - j * thePrefab.transform.localScale.y, transform.position.z );
            createBall( v );
        }
    }

    public GameObject createBall( Vector3 vec, BallColor color = BallColor.random, bool newball = false, int row = 1 )
    {
        GameObject b = null;
        List<BallColor> colors = new List<BallColor>();

        for( int i = 1; i < System.Enum.GetValues( typeof( BallColor ) ).Length; i++ )
        {
            colors.Add( (BallColor)i );
        }

        if( color == BallColor.random )
            color = (BallColor)LevelData.colorsDict[UnityEngine.Random.Range( 0, LevelData.colorsDict.Count )];
		if( newball && MainScript.colorsDict.Count > 0 )
        {
            if( GamePlay.Instance.GameStatus == GameState.Playing )
            {
                MainScript.Instance.GetColorsInGame();
                color = (BallColor)MainScript.colorsDict[UnityEngine.Random.Range( 0, MainScript.colorsDict.Count )];
            }
            else
                color = (BallColor)LevelData.colorsDict[UnityEngine.Random.Range( 0, LevelData.colorsDict.Count )];

        }



        b = Instantiate( ball, transform.position, transform.rotation ) as GameObject;
        b.transform.position = new Vector3( vec.x, vec.y, ball.transform.position.z );
        // b.transform.Rotate( new Vector3( 0f, 180f, 0f ) );
        b.GetComponent<ColorBallScript>().SetColor( color );
        b.transform.parent = Meshes.transform;
        b.tag = "" + color;

        GameObject[] fixedBalls = GameObject.FindObjectsOfType( typeof( GameObject ) ) as GameObject[];
        b.name = b.name + fixedBalls.Length.ToString();
        if( newball )
        {

            b.gameObject.layer = 17;
            b.transform.parent = Camera.main.transform;
            Rigidbody2D rig = b.AddComponent<Rigidbody2D>();
            // b.collider2D.isTrigger = false;
      //      rig.fixedAngle = true;
            b.GetComponent<CircleCollider2D>().enabled = false;
            rig.gravityScale = 0;
            if( GamePlay.Instance.GameStatus == GameState.Playing )
                b.GetComponent<Animation>().Play();
        }
        else
        {
            b.GetComponent<ball>().enabled = false;
            if( LevelData.mode == ModeGame.Vertical && row == 0 )
                b.GetComponent<ball>().isTarget = true;
            b.GetComponent<BoxCollider2D>().offset = Vector2.zero;
            b.GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );
            //Destroy( b.rigidbody2D );
            //b.rigidbody2D.isKinematic = true;
            //Destroy( b.GetComponent < BoxCollider2D>() );
            //b.AddComponent<BoxCollider2D>();
            //b.GetComponent<BoxCollider2D>().enabled = false;
            //b.GetComponent<BoxCollider2D>().enabled = true;
        }
        return b.gameObject;
    }

    void CreateEmptyBall( Vector3 vec )
    {
        GameObject b2 = Instantiate( ball, transform.position, transform.rotation ) as GameObject;
        b2.transform.position = new Vector3( vec.x, vec.y, ball.transform.position.z );
        // b.transform.Rotate( new Vector3( 0f, 180f, 0f ) );
        b2.GetComponent<ColorBallScript>().SetColor( 11 );
        b2.transform.parent = Meshes.transform;
        b2.tag = "empty";
        b2.GetComponent<ball>().enabled = false;
        b2.gameObject.layer = 9;
        b2.GetComponent<Animation>().Play( "cat_idle" );
        b2.GetComponent<SpriteRenderer>().sortingOrder = 20;
        b2.GetComponent<BoxCollider2D>().offset = Vector2.zero;
        b2.GetComponent<BoxCollider2D>().size = new Vector2( 0.5f, 0.5f );

    }

 
    int setColorFrame( string sTag )
    {
        int frame = 0;
        //		if(Camera.main.GetComponent<MainScript>().hd){
        if( sTag == "Orange" ) frame = 7;
        else if( sTag == "Red" ) frame = 3;
        else if( sTag == "Yellow" ) frame = 1;
        else if( sTag == "Rainbow" ) frame = 4;
        else if( sTag == "Pearl" ) frame = 6;
        else if( sTag == "Blue" ) frame = 11;
        else if( sTag == "DarkBlue" ) frame = 8;
        else if( sTag == "Green" ) frame = 10;
        else if( sTag == "Pink" ) frame = 5;
        else if( sTag == "Violet" ) frame = 2;
        else if( sTag == "Brown" ) frame = 9;
        else if( sTag == "Gray" ) frame = 12;
        return frame;
    }

    int setColorFrameBug( string sTag )
    {
        int frame = 0;
        if( sTag == "Orange" ) frame = 5;
        else if( sTag == "Red" ) frame = 3;
        else if( sTag == "Yellow" ) frame = 1;
        else if( sTag == "Rainbow" ) frame = 4;
        else if( sTag == "Pearl" ) frame = 10;
        else if( sTag == "Blue" ) frame = 10;
        else if( sTag == "DarkBlue" ) frame = 8;
        else if( sTag == "Green" ) frame = 7;
        else if( sTag == "Pink" ) frame = 4;
        else if( sTag == "Violet" ) frame = 2;
        else if( sTag == "Brown" ) frame = 9;
        else if( sTag == "Gray" ) frame = 6;
        return frame;
    }

    public string getRandomColorTag()
    {
        int color = 0;
        string sTag = "";
        if( MainScript.stage < 6 )
            color = UnityEngine.Random.Range( 0, 4 + MainScript.stage - 1 );
        else
            color = UnityEngine.Random.Range( 0, 4 + 6 );

        if( color == 0 ) sTag = "Orange";
        else if( color == 1 ) sTag = "Red";
        else if( color == 2 ) sTag = "Yellow";
        else if( color == 3 ) sTag = "Rainbow";
        else if( color == 4 ) sTag = "Blue";
        else if( color == 5 ) sTag = "Green";
        else if( color == 6 ) sTag = "Pink";
        else if( color == 7 ) sTag = "Violet";
        else if( color == 8 ) sTag = "Brown";
        else if( color == 9 ) sTag = "Gray";
        return sTag;
    }

    public void createMesh()
    {

        GameObject Meshes = GameObject.Find( "-Meshes" );
        float offset = 0;

        for( int j = 0; j < rows + 1; j++ )
        {
            for( int i = 0; i < columns; i++ )
            {
                if( j % 2 == 0 ) offset = 0; else offset = offsetStep;
                GameObject b = Instantiate( thePrefab, transform.position, transform.rotation ) as GameObject;
                Vector3 v = new Vector3( transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z );
                b.transform.parent = Meshes.transform;
                b.transform.localPosition = v;
                GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag( "Mesh" );
                b.name = b.name + fixedBalls.Length.ToString();
                b.GetComponent<Grid>().offset = offset;
                squares.Add( b );
                lastRow = j;
            }
        }
        creatorBall.Instance.OffGridColliders();

    }

    public void AddMesh()
    {
        GameObject Meshes = GameObject.Find( "-Meshes" );
        float offset = 0;
        int j = lastRow + 1;
        for( int i = 0; i < columns; i++ )
        {
            if( j % 2 == 0 ) offset = 0; else offset = offsetStep;
            GameObject b = Instantiate( thePrefab, transform.position, transform.rotation ) as GameObject;
            Vector3 v = new Vector3( transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z );
            b.transform.parent = Meshes.transform;
            b.transform.position = v;
            GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag( "Mesh" );
            b.name = b.name + fixedBalls.Length.ToString();
            b.GetComponent<Grid>().offset = offset;
            squares.Add( b );
        }
        lastRow = j;

    }

    public GameObject GetSquare( int row, int col )
    {
        return squares[row * columns + col];
    }


}
