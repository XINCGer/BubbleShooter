using UnityEngine;
using System.Collections;

public class bouncer : MonoBehaviour
{
    Vector3 tempPosition;
    Vector3 targetPrepare;
    bool isPaused;
    public bool startBounce;
    float startTime;
    public float offset;
    public ArrayList nearBalls = new ArrayList();
    //	private OTSpriteBatch spriteBatch = null;  
    GameObject Meshes;
    public int countNEarBalls;
    float gameOverBorder;

    // Use this for initialization
    void Start()
    {
        isPaused = Camera.main.GetComponent<MainScript>().isPaused;
        gameOverBorder = Camera.main.GetComponent<MainScript>().gameOverBorder;
        targetPrepare = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator bonceCoroutine()
    {

        while (Vector3.Distance(transform.position, targetPrepare) > 1 && !isPaused && !GetComponent<ball>().setTarget)
        {
            //transform.position  += targetPrepare * Time.deltaTime;
            transform.position = Vector3.Lerp(tempPosition, targetPrepare, (Time.time - startTime) * 2f);
            //	transform.position  = targetPrepare ;
            yield return new WaitForSeconds(1f / 30f);
        }

    }

    IEnumerator bonceToCatapultCoroutine()
    {

        /*	while (Vector3.Distance(transform.position, targetPrepare)>1 && !isPaused && !GetComponent<ball>().setTarget ){
                //transform.position  += targetPrepare * Time.deltaTime;
                transform.position = Vector3.Lerp(tempPosition, targetPrepare,  (Time.time - startTime)*2);
                //	transform.position  = targetPrepare ;
                yield return new WaitForSeconds(1f/5f);
            }
            if(!isPaused)*/
        Invoke("delayedBonceToCatapultCoroutine", 0.5f);
        yield return new WaitForSeconds(1f / 5f);
    }

    void delayedBonceToCatapultCoroutine()
    {
        transform.position = targetPrepare;
        GetComponent<ball>().newBall = true;

    }

    void newBall()
    {
        GetComponent<ball>().newBall = true;
        Grid.waitForAnim = false;
    }

    public void bounceToCatapult(Vector3 vector3)
    {
        vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
        tempPosition = transform.position;
        targetPrepare = vector3;
        startBounce = true;
        startTime = Time.time;
        iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
        //		StartCoroutine(bonceToCatapultCoroutine());
        //transform.position = vector3;
        Grid.waitForAnim = false;

    }

    public void bounceTo(Vector3 vector3)
    {
        vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
        tempPosition = transform.position;
        targetPrepare = vector3;
        startBounce = true;
        startTime = Time.time;
        if( GamePlay.Instance.GameStatus == GameState.Playing )
            iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear));
        else if( GamePlay.Instance.GameStatus == GameState.Win )
            iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.00001, "easetype", iTween.EaseType.linear));
        //StartCoroutine(bonceCoroutine());
        //transform.position = vector3;
    }

    public void dropDown()
    {
        Vector3 v;

        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer("Mesh");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        foreach (Collider2D obj in fixedBalls)
        {
            float distTemp = Vector3.Distance(new Vector3(transform.position.x - offset, transform.position.y, transform.position.z), obj.transform.position);
            if (distTemp <= 0.9f && obj.transform.position.y + 0.1f < transform.position.y)
            {
                if (obj.GetComponent<Grid>().offset > 0)
                {
                    v = new Vector3(transform.position.x + obj.GetComponent<Grid>().offset, obj.transform.position.y, transform.position.z);
                }
                else
                {
                    v = new Vector3(obj.transform.position.x, obj.transform.position.y, transform.position.z);
                }
                bounceTo(v);
                //	transform.position = v;
                return;
            }
        }

    }

    public bool checkNearestBall(ArrayList b)
    {
        if (transform.position.y >= 530f / 640f * Camera.main.orthographicSize)
        {
            Camera.main.GetComponent<MainScript>().controlArray = addFrom(b, Camera.main.GetComponent<MainScript>().controlArray);
            b.Clear();
            return true;    /// don't destroy
        }
        if (findInArray(Camera.main.GetComponent<MainScript>().controlArray, gameObject)) { b.Clear(); return true; } /// don't destroy
        b.Add(gameObject);
        foreach (GameObject obj in nearBalls)
        {
            if (obj.gameObject.layer == 9 && obj != gameObject)
            {
                //	if(findInArray(Camera.main.GetComponent<MainScript>().controlArray, obj.gameObject)){b.Clear(); return true;} /// don't destroy
                //	else{
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 0.8f && distTemp > 0)
                {
                    if (!findInArray(b, obj.gameObject))
                    {
                        Camera.main.GetComponent<MainScript>().arraycounter++;
                        if (obj.GetComponent<bouncer>().checkNearestBall(b))
                            return true;
                    }
                }
                //		}
            }
        }
        return false;

    }

    public bool findInArray(ArrayList b, GameObject destObj)
    {
        foreach (GameObject obj in b)
        {

            if (obj == destObj) return true;
        }
        return false;
    }

    public ArrayList addFrom(ArrayList b, ArrayList b2)
    {
        foreach (GameObject obj in b)
        {
            if (!findInArray(b2, obj))
            {
                b2.Add(obj);
            }
        }
        return b2;
    }

    public void connectNearBalls()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        nearBalls.Clear();
        foreach (Collider2D obj in fixedBalls)
        {
            if (nearBalls.Count <= 7)
                nearBalls.Add(obj.gameObject);
        }
        countNEarBalls = nearBalls.Count;
    }

    public void checkNextNearestColor(ArrayList b, int counter)
    {
        //		Debug.Log(b.Count);
        Vector3 distEtalon = transform.localScale;
        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1f, layerMask);
        foreach (Collider2D obj1 in meshes)
        {
            if (obj1.gameObject.tag == tag)
            {
                GameObject obj = obj1.gameObject;
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 1f)
                {
                    if (!findInArray(b, obj))
                    {
                        counter++;
                        b.Add(obj);
                        obj.GetComponent<ball>().checkNextNearestColor(b, counter);
                        //		destroy();
                        //obj.GetComponent<mesh>().checkNextNearestColor();
                        //		obj.GetComponent<mesh>().destroy();
                    }
                }
            }
        }
    }

}
