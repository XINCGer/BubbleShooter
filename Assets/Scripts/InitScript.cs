using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
public enum BoostType
{
    FiveBallsBoost = 0,
    ColorBallBoost,
    FireBallBoost
}
public enum Target
{
    Top = 0,
    Chicken
}

namespace InitScriptName
{


    public class InitScript : MonoBehaviour
    {
        public static InitScript Instance;
        private int _levelNumber = 1;
        private int _starsCount = 1;
        private bool _isShow;
        public static int openLevel;

        public static bool boostJelly;
        public static bool boostMix;
        public static bool boostChoco;

        public static bool sound = false;
        public static bool music = false;

        public static int waitedPurchaseGems;

        public static List<string> selectedFriends;
        public static bool Lauched;
        public static int scoresForLeadboardSharing;
        public static int lastPlace;
        public static int savelastPlace;
        public static bool beaten;
        public static List<string> Beatedfriends;
        int messCount;
        public static bool loggedIn;
        public GameObject EMAIL;
        public GameObject MessagesBox;


        public static bool FirstTime;
        public static int Lifes;
        /// <summary>
        /// 生命的最大值
        /// </summary>
        public static int CapOfLife = 5;
        public static int Gems;

        public static float RestLifeTimer;
        public static string DateOfExit;
        public static DateTime today;
        public static DateTime DateOfRestLife;
        public static string timeForReps;

        public static bool openNext;
        public static bool openAgain;

        public int FiveBallsBoost;
        public int ColorBallBoost;
        public int FireBallBoost;
        public bool BoostActivated;

        public Target currentTarget;

        public void Awake()
        {
            Instance = this;
            if (Application.loadedLevelName == "Map")
            {
                if (GameObject.Find("Canvas").transform.Find("MenuPlay").gameObject.activeSelf) GameObject.Find("Canvas").transform.Find("MenuPlay").gameObject.SetActive(false);

            }
            RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");

            //			if(InitScript.DateOfExit == "")
            //			print(InitScript.DateOfExit );
            DateOfExit = PlayerPrefs.GetString("DateOfExit", "");

            Gems = PlayerPrefs.GetInt("Gems");
            Lifes = PlayerPrefs.GetInt("Lifes");

            //当游戏第一次运行时候
            if (PlayerPrefs.GetInt("Lauched") == 0)
            {
                FirstTime = true;
                Lifes = CapOfLife;
                Gems = 5;
                //利用PlayerPrefs存储数据
                PlayerPrefs.SetInt("Gems", Gems);
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.SetInt("Lauched", 1);
                PlayerPrefs.SetInt("Music", 1);
                PlayerPrefs.SetInt("Sound", 1);
                PlayerPrefs.Save();
            }

            GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
            SoundBase.GetInstance().GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");

            ReloadBoosts();

            boostPurchased = false;

        }

        void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Application.targetFrameRate = 60;
                //	PlayerPrefs.DeleteAll();

                //			RestScores();
            }

        }


        public static bool boostPurchased;


        public void AddGems(int count)
        {
            Gems += count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
        }

        public void SpendGems(int count)
        {
            Gems -= count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
        }

        public void RestoreLifes()
        {
            Lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", Lifes);
            PlayerPrefs.Save();
        }


        public void AddLife(int count)
        {
            Lifes += count;
            if (Lifes > CapOfLife) Lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", Lifes);
            PlayerPrefs.Save();
        }

        public int GetLife()
        {
            if (Lifes > CapOfLife)
            {
                Lifes = CapOfLife;
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.Save();
            }
            return Lifes;
        }

        public void PurchaseSucceded()
        {
            AddGems(waitedPurchaseGems);
            waitedPurchaseGems = 0;
        }
        public void SpendLife(int count)
        {
            if (Lifes > 0)
            {
                Lifes -= count;
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.Save();
            }
        }

        public void BuyBoost(BoostType boostType, int count, int price)
        {
            SpendGems(price);
            if (boostType != BoostType.FiveBallsBoost)
            {
                PlayerPrefs.SetInt("" + boostType, count);
                PlayerPrefs.Save();
            }
            else
            {
                //                LevelData.LimitAmount += 5;
            }
            ReloadBoosts();
        }

        public void SpendBoost(BoostType boostType)
        {
            InitScript.Instance.BoostActivated = true;
            if (boostType != BoostType.FiveBallsBoost)
                MainScript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>().SetBoost(boostType);
            else
                LevelData.LimitAmount += 5;
            PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) - 1);
            PlayerPrefs.Save();
            ReloadBoosts();
        }

        public void ReloadBoosts()
        {
            FiveBallsBoost = PlayerPrefs.GetInt("" + BoostType.FiveBallsBoost);
            ColorBallBoost = PlayerPrefs.GetInt("" + BoostType.ColorBallBoost);
            FireBallBoost = PlayerPrefs.GetInt("" + BoostType.FireBallBoost);

        }


        #region 关卡选择相关函数
        public int LoadLevelStarsCount(int level)
        {
            return level > 10 ? 0 : (level % 3 + 1);
        }

        public void SaveLevelStarsCount(int level, int starsCount)
        {
            Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        }

        public void ClearLevelProgress(int level)
        {

        }


        public void OnLevelClicked(int number)
        {
            if (!GameObject.Find("Canvas").transform.Find("MenuPlay").gameObject.activeSelf)
            {
                PlayerPrefs.SetInt("OpenLevel", number);
                PlayerPrefs.Save();
                openLevel = number;
                currentTarget = LevelData.GetTarget(number);
                GameObject.Find("Canvas").transform.Find("MenuPlay").gameObject.SetActive(true);
            }
        }
        #endregion


        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (RestLifeTimer > 0)
                {
                    PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
                }
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
                PlayerPrefs.SetInt("Gems", Gems);
                PlayerPrefs.Save();
            }
        }

        void OnDisable()
        {
            PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            PlayerPrefs.SetInt("Lifes", Lifes);
            if (Application.loadedLevel != 2)
                PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
        }

    }


}