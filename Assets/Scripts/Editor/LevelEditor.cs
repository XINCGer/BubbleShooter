using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

/// <summary>
/// 关卡编辑器类
/// </summary>
public class LevelEditor : EditorWindow
{
    private static LevelEditor window;
    private int maxRows;
    private int maxCols = 11;
    public static BallColor[] levelSquares = new BallColor[81];
    private Texture[] ballTex;
    int levelNumber = 1;
    private Vector2 scrollViewVector;
    private Target target;
    private LIMIT limitType;
    private int limit;
    private int colorLimit;
    private int star1;
    private int star2;
    private int star3;
    private string fileName = "1.txt";
    private BallColor brush;
    private int selected;
    private static GameObject levelEditorBaseObj = null;

    [MenuItem("Window/Level Editor")]
    static void Init()
    {
        //获取打开的窗口，如果不存在则创建一个
        CreateLevelBaseObj();
        window = GetWindow<LevelEditor>(); ;
        window.Show();
    }

    public static void ShowWindow()
    {
        GetWindow(typeof(LevelEditor));
    }

    private static void CreateLevelBaseObj()
    {
        levelEditorBaseObj = GameObject.FindWithTag("LevelEditor");
        if (null == levelEditorBaseObj)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets/Editor/EditorRes/LevelEditorBase.prefab");
            if (null != prefab)
            {
                levelEditorBaseObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                levelEditorBaseObj.name = prefab.name;
            }
            else
            {
                Debug.LogWarning("找不到编辑器所依赖的预制体!无法打开关卡编辑器!");
            }
        }
    }

    void OnFocus()
    {
        if (maxRows <= 0)
            maxRows = 30;
        if (maxCols <= 0)
            maxCols = 11;

        Initialize();
        LoadDataFromLocal(levelNumber);
        LevelEditorBase lm = null;
        if (null != levelEditorBaseObj)
        {
            lm = levelEditorBaseObj.GetComponent<LevelEditorBase>();
        }
        //解决打开错误场景后窗口卡住的问题
        if (lm == null)
        {
            Debug.LogError("Can't find LevelEditorBase组件！");
            window.Close();
        }
        ballTex = new Texture[lm.sprites.Length];
        for (int i = 0; i < lm.sprites.Length; i++)
        {
            ballTex[i] = lm.sprites[i].texture;
        }

    }

    void Initialize()
    {
        levelSquares = new BallColor[maxCols * maxRows];
        for (int i = 0; i < levelSquares.Length; i++)
        {
            levelSquares[i] = 0;
        }

    }

    void OnGUI()
    {
        if (levelNumber < 1)
            levelNumber = 1;


        scrollViewVector = GUI.BeginScrollView(new Rect(25, 45, position.width - 30, position.height), scrollViewVector, new Rect(0, 0, 400, 2000));
        //      GUILayout.Space(-30);



        GUILevelSelector();
        GUILayout.Space(10);

        GUILimit();
        GUILayout.Space(10);


        GUIColorLimit();
        GUILayout.Space(10);

        GUIStars();
        GUILayout.Space(10);

        //GUITarget();
        //GUILayout.Space(10);

        GUIBlocks();
        GUILayout.Space(20);


        GUIGameField();

        GUI.EndScrollView();

    }


    #region leveleditor

    void GUILevelSelector()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level editor", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        //if (GUILayout.Button("Test level", new GUILayoutOption[] { GUILayout.Width(150) }))
        //{
        //    PlayerPrefs.SetInt("OpenLevelTest", levelNumber);
        //    PlayerPrefs.SetInt("OpenLevel", levelNumber);
        //    PlayerPrefs.Save();

        //    EditorApplication.isPlaying = true;


        //}
        GUILayout.EndHorizontal();

        //     myString = EditorGUILayout.TextField("Text Field", myString);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(50) });
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if (GUILayout.Button("<<", new GUILayoutOption[] { GUILayout.Width(50) }))
        {
            PreviousLevel();
        }
        string changeLvl = GUILayout.TextField(" " + levelNumber, new GUILayoutOption[] { GUILayout.Width(50) });
        try
        {
            if (int.Parse(changeLvl) != levelNumber)
            {
                if (LoadDataFromLocal(int.Parse(changeLvl)))
                    levelNumber = int.Parse(changeLvl);

            }
        }
        catch (Exception)
        {

            throw;
        }

        if (GUILayout.Button(">>", new GUILayoutOption[] { GUILayout.Width(50) }))
        {
            NextLevel();
        }

        if (GUILayout.Button("New level", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            AddLevel();
        }


        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

    }

    void AddLevel()
    {
        SaveLevel();
        levelNumber = GetLastLevel() + 1;
        Initialize();
        SaveLevel();
    }

    void NextLevel()
    {
        levelNumber++;
        if (!LoadDataFromLocal(levelNumber))
            levelNumber--;
    }

    void PreviousLevel()
    {
        levelNumber--;
        if (levelNumber < 1)
            levelNumber = 1;
        if (!LoadDataFromLocal(levelNumber))
            levelNumber++;


    }


    #endregion


    void GUILimit()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        GUILayout.Label("Limit:", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(50) });
        LIMIT limitTypeSave = limitType;
        int oldLimit = limit;
        limitType = (LIMIT)EditorGUILayout.EnumPopup(limitType, GUILayout.Width(93));
        if (limitType == LIMIT.MOVES)
            limit = EditorGUILayout.IntField(limit, new GUILayoutOption[] { GUILayout.Width(50) });
        else
        {
            GUILayout.BeginHorizontal();
            int limitMin = EditorGUILayout.IntField(limit / 60, new GUILayoutOption[] { GUILayout.Width(30) });
            GUILayout.Label(":", new GUILayoutOption[] { GUILayout.Width(10) });
            int limitSec = EditorGUILayout.IntField(limit - (limit / 60) * 60, new GUILayoutOption[] { GUILayout.Width(30) });
            limit = limitMin * 60 + limitSec;
            GUILayout.EndHorizontal();
        }
        if (limit <= 0)
            limit = 1;
        GUILayout.EndHorizontal();

        if (limitTypeSave != limitType || oldLimit != limit)
            SaveLevel();

    }

    void GUIColorLimit()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        int saveInt = colorLimit;
        GUILayout.Label("Color limit:", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(100) });
        colorLimit = (int)GUILayout.HorizontalSlider(colorLimit, 3, 5, new GUILayoutOption[] { GUILayout.Width(100) });
        colorLimit = EditorGUILayout.IntField("", colorLimit, new GUILayoutOption[] { GUILayout.Width(50) });
        if (colorLimit < 3)
            colorLimit = 3;
        if (colorLimit > 5)
            colorLimit = 5;

        GUILayout.EndHorizontal();

        if (saveInt != colorLimit)
        {
            SaveLevel();
        }

    }


    void GUIStars()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();

        GUILayout.Label("Stars:", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.Label("Star1", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Star2", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Star3", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        int s = 0;
        s = EditorGUILayout.IntField("", star1, new GUILayoutOption[] { GUILayout.Width(100) });
        if (s != star1)
        {
            star1 = s;
            SaveLevel();
        }
        if (star1 < 0)
            star1 = 10;
        s = EditorGUILayout.IntField("", star2, new GUILayoutOption[] { GUILayout.Width(100) });
        if (s != star2)
        {
            star2 = s;
            SaveLevel();
        }
        if (star2 < star1)
            star2 = star1 + 10;
        s = EditorGUILayout.IntField("", star3, new GUILayoutOption[] { GUILayout.Width(100) });
        if (s != star3)
        {
            star3 = s;
            SaveLevel();
        }
        if (star3 < star2)
            star3 = star2 + 10;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    [MenuItem("Window/ClearCache")]
    public static void ClearCache()
    {
        Process proc = null;
        try
        {
            proc = new Process();
            proc.StartInfo.FileName = @"./Assets/Prefabs/ClearCache.bat";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
        }
        catch (Exception ex)
        {
            Debug.Log("Exception Occurred" + ex.Message + ex.StackTrace.ToString());
        }
    }

    void GUITarget()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.Label("Target:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        Target saveTarget = target;
        target = (Target)EditorGUILayout.EnumPopup(target, GUILayout.Width(100));
        if (target == Target.Top)
        {
        }
        GUILayout.EndVertical();
        if (saveTarget != target)
        {
            SaveLevel();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.EndHorizontal();
    }


    void GUIBlocks()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("Tools:", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
        {
            for (int i = 0; i < levelSquares.Length; i++)
            {
                levelSquares[i] = 0;
            }
            SaveLevel();
        }
        GUILayout.EndVertical();


        GUILayout.Label("Balls:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        for (int i = 1; i <= System.Enum.GetValues(typeof(BallColor)).Length; i++)
        {
            if ((BallColor)i != 0)
            {
                if (GUILayout.Button(ballTex[i - 1], new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
                {
                    if ((BallColor)i != BallColor.chicken)
                        brush = (BallColor)i;
                    else
                    {
                        target = Target.Chicken;
                        levelSquares[5 * maxCols + 5] = BallColor.chicken;
                        SaveLevel();
                    }
                }
            }


        }
        if (GUILayout.Button("  ", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
        {
            brush = 0;
        }
        //   GUILayout.Label(" - empty", EditorStyles.boldLabel);


        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    void GUIGameField()
    {
        GUILayout.BeginVertical();
        bool offset = false;
        for (int row = 0; row < maxRows; row++)
        {
            GUILayout.BeginHorizontal();
            if (offset)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);

            }
            for (int col = 0; col < maxCols; col++)
            {

                var imageButton = new object();
                if (levelSquares[row * maxCols + col] == 0)
                {
                    imageButton = "X";
                }
                else if (levelSquares[row * maxCols + col] != 0)
                {
                    if (levelSquares[row * maxCols + col] == BallColor.blue)
                    {
                        imageButton = ballTex[0];
                    }
                    else if (levelSquares[row * maxCols + col] == BallColor.green)
                    {
                        imageButton = ballTex[1];
                    }
                    else if (levelSquares[row * maxCols + col] == BallColor.red)
                    {
                        imageButton = ballTex[2];
                    }
                    else if (levelSquares[row * maxCols + col] == BallColor.violet)
                    {
                        imageButton = ballTex[3];
                    }
                    else if (levelSquares[row * maxCols + col] == BallColor.yellow)
                    {
                        imageButton = ballTex[4];
                    }
                    else if (levelSquares[row * maxCols + col] == BallColor.random)
                    {
                        imageButton = ballTex[5];
                    }
                    else if (levelSquares[row * maxCols + col] == BallColor.chicken)
                    {
                        imageButton = ballTex[6];
                    }

                }

                if (GUILayout.Button(imageButton as Texture, new GUILayoutOption[] {
                    GUILayout.Width (50),
                    GUILayout.Height (50)
                }))
                {
                    SetType(col, row);
                }
            }
            GUILayout.EndHorizontal();
            if (offset)
            {
                GUILayout.EndHorizontal();

            }


            offset = !offset;
        }
        GUILayout.EndVertical();
    }

    void SetType(int col, int row)
    {
        bool chickenExist = false;
        levelSquares[row * maxCols + col] = brush;
        foreach (BallColor item in levelSquares)
        {
            if (item == BallColor.chicken)
                chickenExist = true;
        }
        if (chickenExist) target = Target.Chicken;
        else target = Target.Top;
        SaveLevel();
        // GetSquare(col, row).type = (int) squareType;
    }


    int GetLastLevel()
    {
        TextAsset mapText = null;
        for (int i = levelNumber; i < 50000; i++)
        {
            mapText = Resources.Load("Levels/" + i) as TextAsset;
            if (mapText == null)
            {
                return i - 1;
            }
        }
        return 0;
    }

    void SaveLevel()
    {
        if (!fileName.Contains(".txt"))
            fileName += ".txt";
        SaveMap(fileName);
    }

    public void SaveMap(string fileName)
    {
        string saveString = "";
        //配置写入字符串
        saveString += "MODE " + (int)target;
        saveString += "\r\n";
        saveString += "SIZE " + maxCols + "/" + maxRows;
        saveString += "\r\n";
        saveString += "LIMIT " + (int)limitType + "/" + limit;
        saveString += "\r\n";
        saveString += "COLOR LIMIT " + colorLimit;
        saveString += "\r\n";
        saveString += "STARS " + star1 + "/" + star2 + "/" + star3;
        saveString += "\r\n";

        //配置Map数据
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                saveString += (int)levelSquares[row * maxCols + col];
                //如果此列不是当前行的最后一列，那么就添加space分隔符
                if (col < (maxCols - 1))
                    saveString += " ";
            }
            //如果不是最后一行，则换行
            if (row < (maxRows - 1))
                saveString += "\r\n";
        }
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            //写入文件
            string activeDir = Application.dataPath + @"/Resources/Levels/";
            string newPath = System.IO.Path.Combine(activeDir, levelNumber + ".txt");
            StreamWriter sw = new StreamWriter(newPath);
            sw.Write(saveString);
            sw.Close();
        }
        AssetDatabase.Refresh();
    }

    public bool LoadDataFromLocal(int currentLevel)
    {
        //从txt文本读取
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            return false;
            SaveLevel();
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
        return true;
    }

    void ProcessGameDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE "))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                target = (Target)int.Parse(modeString);
            }
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                maxCols = int.Parse(sizes[0]);
                maxRows = int.Parse(sizes[1]);
                Initialize();
            }
            else if (line.StartsWith("LIMIT "))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                limitType = (LIMIT)int.Parse(sizes[0]);
                limit = int.Parse(sizes[1]);

            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                colorLimit = int.Parse(blocksString);
            }
            else if (line.StartsWith("STARS "))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                star1 = int.Parse(blocksNumbers[0]);
                star2 = int.Parse(blocksNumbers[1]);
                star3 = int.Parse(blocksNumbers[2]);
            }
            else
            {
                //分割lines获取map数量
                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    levelSquares[mapLine * maxCols + i] = (BallColor)int.Parse(st[i][0].ToString());
                }
                mapLine++;
            }
        }
    }



}


public class MySprite : ScriptableObject
{
    public Sprite background;
}