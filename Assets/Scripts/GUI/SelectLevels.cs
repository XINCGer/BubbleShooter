using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectLevels : MonoBehaviour
{
    int latestFile;
    public GameObject levelPrefab;
    public Vector3 startPosition;
    public Vector2 offset;
    public int countInRow = 4;
    public int countInColumn = 4;
    public Button backButton;
    public Button nextButton;
    int firstShownLevelInGrid;

    // Use this for initialization
    void Start()
    {
        GenerateGrid();
    }


    /// <summary>
    /// 生成网格布局的关卡
    /// </summary>
    /// <param name="genfrom"></param>从哪里开始生成的index游标
    void GenerateGrid(int genfrom = 0)
    {
        int l = 0;
        int posCOunter = 0;
        ClearLevels();
        firstShownLevelInGrid = genfrom;
        latestFile = GetLastLevel();
        for (l = genfrom; l < latestFile; l++)
        {
            GameObject level = Instantiate(levelPrefab) as GameObject;
            level.GetComponent<Level>().number = l+1;
            level.transform.SetParent(transform);
            level.transform.localPosition = startPosition + Vector3.right * (posCOunter % countInRow) * offset.x + Vector3.down * (posCOunter / countInColumn) * offset.y;
            level.transform.localScale = Vector3.one;
            if (posCOunter + 1 >= countInRow * countInColumn) break;
            posCOunter++;
        }
        if (genfrom == 0) backButton.gameObject.SetActive(false);
        else if (genfrom > 0) backButton.gameObject.SetActive(true);
        if (l + 1 >= latestFile) nextButton.gameObject.SetActive(false);
        else nextButton.gameObject.SetActive(true);

    }

    /// <summary>
    /// 清除之前缓存的关卡数据
    /// </summary>
    void ClearLevels()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void Next()
    {
        GenerateGrid(firstShownLevelInGrid + countInRow * countInColumn);
    }

    public void Back()
    {
        GenerateGrid(firstShownLevelInGrid - countInRow * countInColumn);

    }

    /// <summary>
    /// 获取最后一关，即关卡总数
    /// </summary>
    /// <returns></returns>关卡数量
    int GetLastLevel()
    {
        TextAsset mapText = null;
        for (int i = 1; i < 50000; i++)
        {
            mapText = Resources.Load("Levels/" + i) as TextAsset;
            if (mapText == null)
            {
                return i - 1;
            }
        }
        return 0;
    }
}
