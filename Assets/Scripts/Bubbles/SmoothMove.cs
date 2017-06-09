using UnityEngine;
using System.Collections;

public class SmoothMove : MonoBehaviour
{
    public Vector3 targetPos;

    // Use this for initialization
    void Start()
    {
        transform.parent.SetParent(GameObject.Find("-Meshes").transform);
        if (GamePlay.Instance.GameStatus == GameState.Playing)
            StartCoroutine(StartMove());
        else
        {
            MainScript.Instance.TargetCounter1++;
        }
    }

    // Update is called once per frame
    IEnumerator StartMove()
    {
        GetComponent<SpriteRenderer>().sortingLayerName = "UI layer";

        MainScript.Instance.TargetCounter1++;
        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, transform.position.x), new Keyframe(0.5f, targetPos.x));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, transform.position.y), new Keyframe(0.5f, targetPos.y));
        curveY.AddKey(0.2f, transform.position.y - 4);
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = Random.Range(0.4f, 0.6f);
        float distCovered = 0;
        while (distCovered < 1)
        {
            distCovered = (Time.time - startTime) * speed;
            transform.position = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);

            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
