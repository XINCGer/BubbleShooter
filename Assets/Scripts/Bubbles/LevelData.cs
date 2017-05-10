using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Xml;

public enum ModeGame
{
    Vertical=0,
    Rounded,
    Animals,
}
public class LevelData
{
    public static LevelData Instance;

    public static int[] map = new int[11 * 70];

    //List of mission in this map
    public static List<Mission> requestMissions = new List<Mission>();
    public static ModeGame mode = ModeGame.Vertical;
    private static float limitAmount = 40;

    public static float LimitAmount
    {
        get { return limitAmount; }
        set 
        { 
            limitAmount = value;
            if( value < 0 ) limitAmount = 0;
        }
    }
    private static bool startReadData;
    public static Dictionary<int, BallColor> colorsDict = new Dictionary<int, BallColor>();
    static int key;
    public static int colors;
    public static int star1;
    public static int star2;
    public static int star3;
    public static void LoadDataFromXML(int currentLevel)
    {
        requestMissions.Clear();
       // TextAsset textAsset = (TextAsset)Resources.Load(  "4.txt", typeof( TextAsset ) ) ;
        TextAsset textReader = Resources.Load( "Levels/" + currentLevel ) as TextAsset;
      //  TextReader textReader = new StreamReader(Application.dataPath + "/Resources/Levels/" + currentLevel + ".tmx");
        //TextReader textReader = new StreamReader( textAsset );
        //TextReader textReader = new TextReader( textAsset );
        ProcessGameDataFromXML( textReader );
       // textReader.Close();

    }

    public static void LoadDataFromLocal(int currentLevel)
    {
        requestMissions.Clear();
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        ProcessGameDataFromString(mapText.text);
    }
    public static void LoadDataFromURL(int currentLevel)
    {
        //Read data from your server, if you want
    }
    static void ProcessGameDataFromString(string mapText)
    {
        //Structure of text file like this:
        //1st: Line start with "GM". This is game mode line (0-Move Limit, 1-Time Limit)
        //2nd: Line start with "LMT" is limit amount of play time (time of move or seconds depend on game mode)
        //Ex: LMT 20  mean player can move 20 times or 20 seconds, depend on game mode
        //3rd: Line start with "MNS" is missions line. This is amount ofScore/Block/Ring/... 
        //Ex: MNS 10000/24/0' mean user need get 1000 points, 24 block, and not need to get rings.
        //4th:Map lines: This is an array of square types.
        //First thing is split text to get all in arrays of text
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        foreach (string line in lines)
        {
            //check if line is game mode line
            if (line.StartsWith("GM="))
            {
                //Replace GM to get mode number, 
                string modeString = line.Replace("GM=", string.Empty).Trim();
                //then parse it to interger
                int modeNum = int.Parse(modeString);
                //Assign game mode
                mode = (ModeGame)modeNum;
            }
            else if (line.StartsWith("LMT="))
            {
                //Replace LTM to get limit number, 
                string amountString = line.Replace("LMT=", string.Empty).Trim();
                //then parse it to interger and assign to limitAmount
                limitAmount = int.Parse(amountString);
            }
            //check third line to get missions
            else if (line.StartsWith("MNS"))
            {
                //Replace 'MNS' to get mission numbers
                string missionString = line.Replace("MNS", string.Empty).Trim();
                //Split again to get mission numbers
                string[] missionNumbers = missionString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < missionNumbers.Length; i++)
                {
                    //Set scores of mission and mission type
                    int amount = int.Parse(missionNumbers[i].Trim());
                    MissionType type = (MissionType)i;
                    if (amount > 0)
                        requestMissions.Add(new Mission(amount, type));
                }
            }
            else if (line.StartsWith("data="))
            {
                startReadData = true;
            }
            else if (startReadData)//Maps
            {
                //Split lines again to get map numbers
                string[] squareTypes = line.Replace("\r", string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < squareTypes.Length; i++)
                {
                    int value = int.Parse(squareTypes[i].Trim());
                    //if (!colorsDict.ContainsValue((BallColor)mapValue) && mapValue != 9)
                    //    colorsDict.Add(key++, (BallColor)mapValue);

                    map[mapLine * creatorBall.columns + i] = value;
                }
                mapLine++;
            }
        }
    }

    static void ProcessGameDataFromXML( TextAsset xmlString )
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml( xmlString.text );
        XmlNodeList elemList = doc.GetElementsByTagName("property");
        foreach (XmlElement element in elemList)
        {
            if (element.GetAttribute("name") == "GM") mode = (ModeGame)int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "LMT") limitAmount = int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "COLORS") colors = int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "STAR1") star1 = int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "STAR2") star2 = int.Parse(element.GetAttribute("value"));
            if (element.GetAttribute("name") == "STAR3") star3 = int.Parse(element.GetAttribute("value"));
            //    Debug.Log(element.GetAttribute("value"));
        }

        elemList = doc.GetElementsByTagName("tile");
        colorsDict.Clear();
        key = 0;

        BallColor exceptedColor = BallColor.violet;

        for (int i = 0; i < creatorBall.rows; i++)
        {
            for (int j = 0; j < creatorBall.columns; j++)
            {
                XmlElement element = (XmlElement)elemList[i * creatorBall.columns + j];
                int value = int.Parse(element.GetAttribute("gid"));

                if (!colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                {
                        colorsDict.Add(key, (BallColor)value);
                        key++;

                }

                map[i * creatorBall.columns + j] = value;
            }

        }


        //random colors
        if (colorsDict.Count == 0) 
        {
            //add constant colors 
            colorsDict.Add(0, BallColor.yellow);
            colorsDict.Add(1, BallColor.red);

            //add random colors
            List<BallColor> randomList = new List<BallColor>();
            randomList.Add(BallColor.blue);
            randomList.Add(BallColor.green);
            if( mode != ModeGame.Rounded ) 
                randomList.Add(BallColor.violet);
            for (int i = 0; i < colors-2; i++)
            {
                BallColor randCol = BallColor.yellow;
                while (colorsDict.ContainsValue(randCol))
                {
                    randCol = randomList[UnityEngine.Random.RandomRange(0, randomList.Count)];
                }
                colorsDict.Add(2 + i, randCol);
                
            }

        }
        //foreach (XmlElement element in elemList)
        //{
        //    Debug.Log(element.GetAttribute("gid"));
        //}

    }

    public static int GetScoreTarget(int currentLevel)
    {
        LoadDataFromLocal(currentLevel);
        return GetMission(MissionType.Stars).amount;
    }

    public static Mission GetMission(MissionType type)
    {
        return requestMissions.Find(obj => obj.type == type);
    }

    public static Target GetTarget(int levelNumber)
    {
        LoadLevel(levelNumber);
        return (Target)LevelData.mode;
    }

    public static bool LoadLevel(int currentLevel)
    {
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcesDataFromString(mapText.text);
        return true;
    }

    static void ProcesDataFromString(string mapText)
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
        }
    }
}


