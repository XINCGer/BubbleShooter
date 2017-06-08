using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InitScriptName;


[RequireComponent(typeof(AudioSource))]
public class MainScript : MonoBehaviour
{
    public int currentLevel;

    public static MainScript Instance;
    GameObject ball;
    Vector2 speed = new Vector2(250, 250); // 星星每秒移动的速度         
    GameObject PauseDialogLD;
    GameObject OverDialogLD;
    GameObject PauseDialogHD;
    GameObject OverDialogHD;
    GameObject UI_LD;
    GameObject UI_HD;
    GameObject PauseDialog;
    GameObject OverDialog;
    GameObject FadeLD;
    GameObject FadeHD;
    GameObject AppearLevel;
    Target target;
    Vector2 worldPos;
    Vector2 startPos;
    float startTime;
    float duration = 1.0f;
    bool setTarget;
    float mTouchOffsetX;
    float mTouchOffsetY;
    float xOffset;
    float yOffset;
    public int bounceCounter = 0;
    GameObject[] fixedBalls;
    public Vector2[][] meshArray;
    int offset;
    public GameObject checkBall;
    public GameObject newBall;
    float waitTime = 0f;
    int revertButterFly = 1;
    private static int score;

    public static int Score
    {
        get { return score; }
        set { score = value; }
    }
    public static int stage = 1;
    const int STAGE_1 = 0;
    const int STAGE_2 = 300;
    const int STAGE_3 = 750;
    const int STAGE_4 = 1400;
    const int STAGE_5 = 2850;
    const int STAGE_6 = 4100;
    const int STAGE_7 = 5500;
    const int STAGE_8 = 6900;
    const int STAGE_9 = 8500;
    public int arraycounter = 0;
    public ArrayList controlArray = new ArrayList();
    bool destringAloneBall;
    public bool dropingDown;
    public float dropDownTime = 0f;
    public bool isPaused;
    public bool noSound;
    public bool gameOver;
    public bool arcadeMode;
    public float bottomBorder;
    public float topBorder;
    public float leftBorder;
    public float rightBorder;
    public float gameOverBorder;
    public float ArcadedropDownTime;
    public bool hd;
    public GameObject Fade;
    public int highScore;
    public AudioClip pops;
    public AudioClip click;
    public AudioClip levelBells;
    float appearLevelTime;
    public GameObject boxCatapult;
    public GameObject boxFirst;
    public GameObject boxSecond;
    public GameObject ElectricLiana;
    public GameObject BonusLiana;
    public GameObject BonusScore;
    public static bool ElectricBoost;
    bool BonusLianaCounter;
    bool gameOverShown;
    public static bool StopControl;
    public GameObject finger;

    public GameObject BoostChanging;

    public creatorBall creatorBall;

    public GameObject GameOverBorderObject;
    public GameObject TopBorder;
    public Transform Balls;
    public Hashtable animTable = new Hashtable();
    public static Vector3 lastBall;
    public GameObject FireEffect;
    public static int doubleScore = 1;

    public int TotalTargets;

    public int countOfPreparedToDestroy;

    public int bugSounds;
    public int potSounds;

    public static Dictionary<int, BallColor> colorsDict = new Dictionary<int, BallColor>();

    private int _ComboCount;

    public int ComboCount
    {
        get { return _ComboCount; }
        set
        {
            _ComboCount = value;
            if (value > 0)
            {
                SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().combo[Mathf.Clamp(value - 1, 0, 5)]);
                creatorBall.Instance.CreateBug(lastBall, value);
                if (value >= 6)
                {
                    SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().combo[5]);
                    FireEffect.SetActive(true);
                    doubleScore = 2;
                }
            }
            else
            {
                DestroyBugs();
                FireEffect.SetActive(false);
                doubleScore = 1;
            }
        }
    }

    public GameObject popupScore;

    private int TargetCounter;

    public int TargetCounter1
    {
        get { return TargetCounter; }
        set
        {
            TargetCounter = value;
        }
    }

    public GameObject[] starsObject;
    public int stars = 0;

    public GameObject perfect;

    public GameObject[] boosts;
    public GameObject[] locksBoosts;

    public GameObject arrows;
    int stageTemp;
    public GameObject newBall2;
    private int maxCols;
    private int maxRows;
    private LIMIT limitType;
    private int limit;
    private int colorLimit;

    //	public int[][] meshMatrix = new int[15][17];
    // Use this for initialization

    void Awake()
    {
        if (InitScript.Instance == null) gameObject.AddComponent<InitScript>();


        currentLevel = PlayerPrefs.GetInt("OpenLevel", 1);
        stage = 1;
        MainScript.StopControl = false;
        animTable.Clear();
        //		arcadeMode = InitScript.Arcade;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            //SwitchLianaBoost();
            //arcadeMode = true;
        }


        creatorBall = GameObject.Find("Creator").GetComponent<creatorBall>();

        // StartCoroutine( ShowArrows() );

        StartCoroutine(CheckColors());

    }

    IEnumerator CheckColors()
    {
        while (true)
        {
            GetColorsInGame();
            yield return new WaitForEndOfFrame();
            SetColorsForNewBall();
        }

    }

    IEnumerator ShowArrows()
    {
        while (true)
        {

            yield return new WaitForSeconds(30);
            if (GamePlay.Instance.GameStatus == GameState.Playing)
            {
                arrows.SetActive(true);

            }
            yield return new WaitForSeconds(3);
            arrows.SetActive(false);
        }
    }

    private void DestroyBugs()
    {
        Transform spiders = GameObject.Find("Spiders").transform;
        List<Bug> listFreePlaces = new List<Bug>();
        for (int i = 0; i < 2; i++)
        {
            listFreePlaces.Clear();
            foreach (Transform item in spiders)
            {
                if (item.childCount > 0) listFreePlaces.Add(item.GetChild(0).GetComponent<Bug>());
            }
            if (listFreePlaces.Count > 0)
                listFreePlaces[Random.Range(0, listFreePlaces.Count)].MoveOut();

        }

    }


    public void PopupScore(int value, Vector3 pos)
    {
        Score += value;
        Transform parent = GameObject.Find("CanvasScore").transform;
        GameObject poptxt = Instantiate(popupScore, pos, Quaternion.identity) as GameObject;
        poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
        poptxt.transform.SetParent(parent);
        poptxt.transform.localScale = Vector3.one;
        Destroy(poptxt, 1);
    }

    public void SwitchLianaBoost()
    {
        if (!ElectricBoost)
        {
            ElectricBoost = true;
            ElectricLiana.SetActive(true);
        }
        else
        {
            ElectricBoost = false;
            ElectricLiana.SetActive(false);
        }
    }

    void Start()
    {
        Instance = this;

        //	AdmobAd.Instance().LoadInterstitialAd(true);
        //		audio.PlayClipAtPoint(ambient, new Vector3(5, 1, 2));
        //audio.loop = true;
        //		if(DisplayMetricsAndroid.WidthPixels>700){
        //			hd = true;
        //		}
        stageTemp = 1;
        RandomizeWaitTime();
        score = 0;
        if (PlayerPrefs.GetInt("noSound") == 1) noSound = true;
        //		if(PlayerPrefs.GetInt("arcade")==1){ arcadeMode = true; 		highScore = PlayerPrefs.GetInt("scoreArcade");}
        //		if(PlayerPrefs.GetInt("arcade")==0){ arcadeMode = false;		highScore = PlayerPrefs.GetInt("score");}

        GamePlay.Instance.GameStatus = GameState.BlockedGame;
        //		GameObject.Find("GUIHighscore").GetComponent<GUIText>().text = "High Score: " + highScore+"";
    }

    // Update is called once per frame
    void Update()
    {
        if (noSound)
            GetComponent<AudioSource>().volume = 0;
        if (!noSound)
            GetComponent<AudioSource>().volume = 0.5f;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            //			GameObject.Find("PauseButton").GetComponent<clickButton>().OnMouseDown();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // creatorBall.AddMesh();

        }
        if (gameOver && !gameOverShown)
        {
            gameOverShown = true;

            //	return;
        }

        if (GamePlay.Instance.GameStatus == GameState.Win)
        {

            //   return;
        }



        if (checkBall != null && (GamePlay.Instance.GameStatus == GameState.Playing || GamePlay.Instance.GameStatus == GameState.WaitForChicken))
        {
            checkBall.GetComponent<ball>().checkNearestColor();
            Destroy(checkBall.GetComponent<Rigidbody>());
            LevelData.LimitAmount--;
            checkBall = null;
            //connectNearBallsGlobal();
            int missCount = 1;
            if (stage >= 3) missCount = 2;
            if (stage >= 9) missCount = 1;
            //Invoke("destroyAloneBall", 0.5f);
            StartCoroutine(destroyAloneBall());

            if (!arcadeMode)
            {
                if (bounceCounter >= missCount)
                {
                    bounceCounter = 0;
                    dropDownTime = Time.time + 0.5f;
                    //Invoke("dropUp", 0.1f);
                }
                else
                {
                    if (!destringAloneBall && !dropingDown)
                    {
                        //connectNearBallsGlobal();
                        destringAloneBall = true;
                        //StartCoroutine(destroyAloneBall());
                        //	dropingDown = true;

                    }
                }
            }
            //			if(stage > 0){
            //				revertButterFly *=-1;
            //				Invoke("startButterfly",Random.Range(1,3));
            //			}

            //		createBall();
        }

        if (arcadeMode && Time.time > ArcadedropDownTime && GamePlay.Instance.GameStatus == GameState.Playing)
        {
            bounceCounter = 0;
            ArcadedropDownTime = Time.time + 10f;
            dropDownTime = Time.time + 0.2f;
            dropDown();
        }



        if (Time.time > dropDownTime && dropDownTime != 0f)
        {
            //	dropingDown = false;
            CheckLosing();
            dropDownTime = 0;
            StartCoroutine(getBallsForMesh());
        }

        if (LevelData.mode == ModeGame.Vertical && TargetCounter >= 6 && GamePlay.Instance.GameStatus == GameState.Playing)
        {
            GamePlay.Instance.GameStatus = GameState.Win;
        }
        else if (LevelData.mode == ModeGame.Rounded && TargetCounter >= 1 && GamePlay.Instance.GameStatus == GameState.WaitForChicken)
            GamePlay.Instance.GameStatus = GameState.Win;
        else if (LevelData.mode == ModeGame.Animals && TargetCounter >= TotalTargets && GamePlay.Instance.GameStatus == GameState.Playing)
            GamePlay.Instance.GameStatus = GameState.Win;

        else if (LevelData.LimitAmount <= 0 && GamePlay.Instance.GameStatus == GameState.Playing && newBall == null)
        {
            GamePlay.Instance.GameStatus = GameState.GameOver;
        }
        ProgressBarScript.Instance.UpdateDisplay((float)score * 100f / ((float)LevelData.star1 / ((LevelData.star1 * 100f / LevelData.star3)) * 100f) / 100f);

        if (score >= LevelData.star1 && stars <= 0)
        {
            stars = 1;
        }
        if (score >= LevelData.star2 && stars <= 1)
        {
            stars = 2;
        }
        if (score >= LevelData.star3 && stars <= 2)
        {
            stars = 3;
        }

        if (score >= LevelData.star1)
        {
            starsObject[0].SetActive(true);
        }
        if (score >= LevelData.star2)
        {
            starsObject[1].SetActive(true);
        }
        if (score >= LevelData.star3)
        {
            starsObject[2].SetActive(true);
        }

    }

    public void CheckLosing()
    {
        fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9)
            {
                //if(transform.position.y <= gameOverBorder  ){
                //    if(!MainScript.ElectricBoost)
                //        Camera.main.GetComponent<MainScript>().gameOver = true;
                //    else
                //    {
                //        MainScript.Instance.BurnRows(3);
                //    }
                //}
            }
        }
    }

    IEnumerator startBonusLiana()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(30, 120));
            Instantiate(BonusLiana);
        }
    }

    IEnumerator startBonusScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 20));
            Instantiate(BonusScore);
        }
    }

    IEnumerator startButterfly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            GameObject gm = GameObject.Find("Creator");
            revertButterFly *= -1;
            gm.GetComponent<creatorButterFly>().createButterFly(revertButterFly);
        }

    }

    IEnumerator getBallsForMesh()
    {
        GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
        foreach (GameObject obj1 in meshes)
        {
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(obj1.transform.position, 0.1f, 1 << 9);  //balls
            foreach (Collider2D obj in fixedBalls)
            {
                obj1.GetComponent<Grid>().Busy = obj.gameObject;
                obj.GetComponent<bouncer>().offset = obj1.GetComponent<Grid>().offset;
            }
        }
        yield return new WaitForSeconds(0.2f);
    }


    public GameObject createFirstBall(Vector3 vector3)
    {
        GameObject gm = GameObject.Find("Creator");
        return gm.GetComponent<creatorBall>().createBall(vector3, BallColor.random, true);
    }

    public void connectNearBallsGlobal()
    {
        ///连接附近的泡泡
        fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9)
                obj.GetComponent<ball>().connectNearBalls();
        }

    }

    public void dropUp()
    {
        if (!dropingDown)
        {
            creatorBall.AddMesh();
            dropingDown = true;
            GameObject Meshes = GameObject.Find("-Meshes");
            iTween.MoveAdd(Meshes, iTween.Hash("y", 0.5f, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "OnMoveFinished"));

        }

    }

    void OnMoveFinished()
    {
        dropingDown = false;
    }

    public void dropDown()
    {

        dropingDown = true;
        fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9)
                obj.GetComponent<bouncer>().dropDown();
        }
        GameObject gm = GameObject.Find("Creator");
        gm.GetComponent<creatorBall>().createRow(0);
        //	Invoke("destroyAloneBall", 1f);
        //	destroyAloneBall();
    }

    public void explode(GameObject gameObject)
    {
        //gameObject.GetComponent<Detonator>().Explode();
    }

    void RandomizeWaitTime()
    {
        const float minimumWaitTime = 5f;
        const float maximumWaitTime = 10f;
        waitTime = Time.time + Random.Range(minimumWaitTime, maximumWaitTime);
    }

    public IEnumerator destroyAloneBall()
    {
        MainScript.Instance.newBall2 = null;

        //if( GamePlay.Instance.GameStatus == GameState.Playing )
        //    MainScript.Instance.newBall = null;
        yield return new WaitForSeconds(Mathf.Clamp((float)countOfPreparedToDestroy / 50, 0.6f, (float)countOfPreparedToDestroy / 50));
        //       yield return new WaitForSeconds( 0.6f );
        int i;
        //	while(true){
        connectNearBallsGlobal();
        i = 0;
        int willDestroy = 0;
        destringAloneBall = true;
        Camera.main.GetComponent<MainScript>().arraycounter = 0;
        GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];			// detect alone balls
        Camera.main.GetComponent<MainScript>().controlArray.Clear();
        foreach (GameObject obj in fixedBalls)
        {
            if (obj != null)
            {
                if (obj.layer == 9)
                {

                    if (!findInArray(Camera.main.GetComponent<MainScript>().controlArray, obj.gameObject))
                    {
                        if (obj.GetComponent<ball>().nearBalls.Count < 7 && obj.GetComponent<ball>().nearBalls.Count > 0)
                        {
                            i++;
                            //	if(i>5){ i = 0; yield return new WaitForSeconds(0.5f); yield return new WaitForSeconds(0.5f);}
                            //		if(dropingDown) yield return new WaitForSeconds(1f);
                            yield return new WaitForEndOfFrame();
                            ArrayList b = new ArrayList();
                            obj.GetComponent<ball>().checkNearestBall(b);
                            if (b.Count > 0)
                            {
                                willDestroy++;
                                destroy(b);
                            }
                        }
                    }
                }
            }
        }
        destringAloneBall = false;
        StartCoroutine(getBallsForMesh());
        dropingDown = false;
        //if( willDestroy > 0)
        //    yield return new WaitForSeconds( 0.5f );

        if (LevelData.mode == ModeGame.Vertical)
            creatorBall.Instance.MoveLevelDown();
        else if (LevelData.mode == ModeGame.Animals)
            creatorBall.Instance.MoveLevelDown();
        else if (LevelData.mode == ModeGame.Rounded)
        {
            CheckBallsBorderCross();
        }

        yield return new WaitForSeconds(0.0f);
        GetColorsInGame();
        MainScript.Instance.newBall = null;
        SetColorsForNewBall();

        //	Debug.Log(i);
        //		yield return new WaitForSeconds(2f);
        //	}
    }

    public void SetColorsForNewBall()
    {

        GameObject ball = null;
        if (boxCatapult.GetComponent<Grid>().Busy != null && colorsDict.Count > 0)
        {
            ball = boxCatapult.GetComponent<Grid>().Busy;
            BallColor color = ball.GetComponent<ColorBallScript>().mainColor;
            if (!colorsDict.ContainsValue(color))
            {
                ball.GetComponent<ColorBallScript>().SetColor((BallColor)MainScript.colorsDict[Random.Range(0, MainScript.colorsDict.Count)]);
            }
        }
    }

    public void GetColorsInGame()
    {
        int i = 0;
        colorsDict.Clear();
        foreach (Transform item in Balls)
        {
            if (item.tag == "chicken" || item.tag == "empty" || item.tag == "Ball") continue;
            BallColor col = (BallColor)System.Enum.Parse(typeof(BallColor), item.tag);
            if (!colorsDict.ContainsValue(col) && (int)col <= (int)BallColor.random)
            {
                colorsDict.Add(i, col);
                i++;
            }
        }
        //        print( colorsDict.Count );

        //if( !colorsDict.ContainsValue( (BallColor)System.Enum.Parse( typeof( BallColor ), boxCatapult.GetComponent<Grid>().Busy.tag ) ) )
        //{
        //    boxCatapult.GetComponent<Grid>().Busy.GetComponent<ColorBallScript>().SetColor((BallColor) colorsDict[ Random.Range( 0, colorsDict.Count)]);
        //}
        //if( !colorsDict.ContainsValue( (BallColor)System.Enum.Parse( typeof( BallColor ), boxSecond.GetComponent<Grid>().Busy.tag ) ) )
        //{
        //    boxSecond.GetComponent<Grid>().Busy.GetComponent<ColorBallScript>().SetColor( (BallColor)colorsDict[Random.Range( 0, colorsDict.Count )] );
        //}
    }

    public void CheckFreeChicken()
    {
        if (LevelData.mode != ModeGame.Rounded) return;
        if (GamePlay.Instance.GameStatus == GameState.Playing)
            StartCoroutine(CheckFreeChickenCor());
    }

    IEnumerator CheckFreeChickenCor()
    {
        //  yield return new WaitForSeconds( Mathf.Clamp( (float)countOfPreparedToDestroy / 100, 1.5f, (float)countOfPreparedToDestroy / 100 ) );
        GamePlay.Instance.GameStatus = GameState.WaitForChicken;
        yield return new WaitForSeconds(1.5f);
        bool finishGame = false;
        if (LevelData.mode == ModeGame.Rounded)
        {
            finishGame = true;

            GameObject balls = GameObject.Find("-Ball");
            //            print( "check free chicken " +  balls.transform.childCount);
            foreach (Transform item in balls.transform)
            {
                if (item.tag != "Ball" && item.tag != "chicken")
                {
                    finishGame = false;
                }
            }
        }
        if (!finishGame)
        {
            GetColorsInGame();

            GamePlay.Instance.GameStatus = GameState.Playing;
        }

        else if (finishGame)
        {
            GamePlay.Instance.GameStatus = GameState.WaitForChicken;

            GameObject chicken = GameObject.FindGameObjectWithTag("chicken");
            chicken.GetComponent<SpriteRenderer>().sortingLayerName = "UI layer";
            Vector3 targetPos = new Vector3(2.3f, 6, 0);
            MainScript.Instance.TargetCounter++;
            AnimationCurve curveX = new AnimationCurve(new Keyframe(0, chicken.transform.position.x), new Keyframe(0.5f, targetPos.x));
            AnimationCurve curveY = new AnimationCurve(new Keyframe(0, chicken.transform.position.y), new Keyframe(0.5f, targetPos.y));
            curveY.AddKey(0.2f, chicken.transform.position.y - 1);
            float startTime = Time.time;
            Vector3 startPos = chicken.transform.position;
            float speed = 0.2f;
            float distCovered = 0;
            while (distCovered < 0.6f)
            {
                distCovered = (Time.time - startTime);
                chicken.transform.position = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
                chicken.transform.Rotate(Vector3.back * 10);
                yield return new WaitForEndOfFrame();
            }
            Destroy(chicken);

            //if(chicken.GetComponent<SmoothMove>() == null)
            //    chicken.AddComponent<SmoothMove>();
        }
    }


    void CheckBallsBorderCross()
    {
        foreach (Transform item in Balls)
        {
            item.GetComponent<ball>().CheckBallCrossedBorder();
        }
    }



    public bool findInArray(ArrayList b, GameObject destObj)
    {
        foreach (GameObject obj in b)
        {

            if (obj == destObj) return true;
        }
        return false;
    }

    public void destroy(GameObject obj)
    {
        if (obj.name.IndexOf("ball") == 0) obj.layer = 0;
        Camera.main.GetComponent<MainScript>().bounceCounter = 0;
        //	obj.GetComponent<OTSprite>().collidable = false;
        //	Destroy(obj);
        obj.GetComponent<ball>().Destroyed = true;
        obj.GetComponent<ball>().growUp();
        Camera.main.GetComponent<MainScript>().explode(obj.gameObject);
        //	Score.Instance.addScore( 3);

    }

    void playPop()
    {
        //	if(!Camera.main.GetComponent<MainScript>().noSound) SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.Pops);
        //AudioSource.PlayClipAtPoint(pops, transform.position);
    }

    public void destroy(ArrayList b)
    {
        Camera.main.GetComponent<MainScript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;

        foreach (GameObject obj in b)
        {
            //			obj.GetComponent<OTSprite>().collidable = false;
            if (obj.name.IndexOf("ball") == 0) obj.layer = 0;
            if (!obj.GetComponent<ball>().Destroyed)
            {
                //if(soundPool<5)
                //    obj.GetComponent<ball>().growUpPlaySound();
                //else
                //    obj.GetComponent<ball>().growUp();
                //		soundPool++;
                if (scoreCounter > 3)
                {
                    rate += 3;
                    scoreCounter += rate;
                }
                scoreCounter++;
                obj.GetComponent<ball>().StartFall();
                //	Destroy(obj);
                //	obj.GetComponent<ball>().destroyed = true;
                //	Camera.main.GetComponent<MainScript>().explode(obj.gameObject);
            }
        }
        CheckFreeChicken();
        //	Score.Instance.addScore( scoreCounter);

    }

    public void ChangeBoost()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().swish[0]);
        Grid.waitForAnim = true;
        GameObject ball1 = boxSecond.GetComponent<Grid>().Busy;
        boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>().newBall = false;
        iTween.MoveTo(boxSecond.GetComponent<Grid>().Busy, iTween.Hash("position", boxCatapult.transform.position, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
        iTween.MoveTo(boxCatapult.GetComponent<Grid>().Busy, iTween.Hash("position", boxSecond.transform.position, "time", 0.3, "easetype", iTween.EaseType.linear));
        boxSecond.GetComponent<Grid>().Busy = boxCatapult.GetComponent<Grid>().Busy;
        boxCatapult.GetComponent<Grid>().Busy = ball1;
        //		boxFirst.GetComponent<Grid>().Busy = boxSecond.GetComponent<Grid>().Busy;
        //boxCatapult.GetComponent<Grid>().BounceFrom(boxFirst);
    }


    public void destroyAllballs()
    {
        foreach (Transform item in Balls)
        {
            if (item.tag != "chicken")
            {

                destroy(item.gameObject);
            }
        }
        CheckFreeChicken();
    }


}


