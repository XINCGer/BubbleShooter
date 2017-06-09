using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Text label;
    public Image picture;

    public Sprite[] pictures;
    public string[] labels;

    // Use this for initialization
    void Start()
    {
        int level = MainScript.Instance.currentLevel;
        if (level <= 8)
        {
            label.text = labels[level - 1];
            picture.sprite = pictures[level - 1];
        }
        else if (level == 11)
        {
            label.text = labels[8];
            picture.sprite = pictures[8];
        }
        else if (level == 15)
        {
            label.text = labels[9];
            picture.sprite = pictures[9];
        }
        else if (level == 17)
        {
            label.text = labels[10];
            picture.sprite = pictures[10];
        }
        else if (level == 26)
        {
            label.text = labels[11];
            picture.sprite = pictures[11];
        }
        else if (level == 41)
        {
            label.text = labels[12];
            picture.sprite = pictures[12];
        }
        else
        {
            GamePlay.Instance.GameStatus = GameState.Playing;
            gameObject.SetActive(false);

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
