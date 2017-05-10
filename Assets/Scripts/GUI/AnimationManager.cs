using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using InitScriptName;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class AnimationManager : MonoBehaviour
{
    public bool PlayOnEnable = true;
    Dictionary<string, string> parameters;

    void OnEnable()
    {
        //这里对平台做个判断，Windows平台单独处理一下分辨率
#if UNITY_STANDALONE_WIN
        Screen.SetResolution(360, 540, false);
#endif

        if (PlayOnEnable)
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().swish[0]);
        }
        if (name == "MenuPlay")
        {
            for (int i = 1; i <= 3; i++)
            {
                transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
            }
            int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", PlayerPrefs.GetInt("OpenLevel")), 0);
            if (stars > 0)
            {
                for (int i = 1; i <= stars; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
                }

            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
                }

            }

        }

        if (name == "Settings" || name == "MenuPause")
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(true);
            else
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(false);

            if (PlayerPrefs.GetInt("Music") == 0)
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(true);
            else
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(false);

        }

    }
    void OnDisable()
    {
        //if( PlayOnEnable )
        //{
        //    if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
        //        GetComponent<SequencePlayer>().sequenceArray[0].Play
        //}
    }




    public void OnFinished()
    {
        if (name == "MenuComplete")
        {
            StartCoroutine(MenuComplete());
            StartCoroutine(MenuCompleteScoring());
        }
        if (name == "MenuPlay")
        {
            InitScript.Instance.currentTarget = LevelData.GetTarget(PlayerPrefs.GetInt("OpenLevel"));

        }

    }



    IEnumerator MenuComplete()
    {
        for (int i = 1; i <= MainScript.Instance.stars; i++)
        {
            //  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
            transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().hit);
        }
    }
    IEnumerator MenuCompleteScoring()
    {
        Text scores = transform.Find("Image").Find("Scores").GetComponent<Text>();
        for (int i = 0; i <= MainScript.Score; i += 500)
        {
            scores.text = "" + i;
            // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
            yield return new WaitForSeconds(0.00001f);
        }
        scores.text = "" + MainScript.Score;
    }


    public void PlaySoundButton()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);

    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void CloseMenu()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (gameObject.name == "MenuPreGameOver")
        {
            ShowGameOver();
        }
        if (gameObject.name == "MenuComplete")
        {
            Application.LoadLevel("Map");
        }
        if (gameObject.name == "MenuGameOver")
        {
            Application.LoadLevel("Map");
        }

        if (Application.loadedLevelName == "Game" || Application.loadedLevelName == "game")
        {
            if (GamePlay.Instance.GameStatus == GameState.Pause)
            {
                GamePlay.Instance.GameStatus = GameState.WaitAfterClose;

            }
        }
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().swish[1]);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 当各种界面上的和Play相关的按钮按下时调用的方法
    /// </summary>
    public void Play()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        //根据绑定的不同GameObejct有不同的功能
        if (gameObject.name == "MenuPreGameOver")
        {
            if (InitScript.Gems >= 12)
            {
                InitScript.Instance.SpendGems(12);
                LevelData.LimitAmount += 12;
                GamePlay.Instance.GameStatus = GameState.WaitAfterClose;
                gameObject.SetActive(false);

            }
            else
            {
                BuyGames();
            }
        }
        else if (gameObject.name == "MenuGameOver")
        {
            Application.LoadLevel("Map");
        }
        else if (gameObject.name == "MenuPlay")
        {
            Application.LoadLevel("Game");
            if (InitScript.Lifes > 0)
            {
                InitScript.Instance.SpendLife(1);

                Application.LoadLevel("Game");
            }
            else
            {
                BuyLifeShop();
            }

        }
        //如果按下Play按钮，则加载Map场景
        else if (gameObject.name == "PlayMain")
        {
            Application.LoadLevel("Map");
        }
    }

    public void PlayTutorial()
    {
        //        SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.click );
        GamePlay.Instance.GameStatus = GameState.Playing;
        //    MainScript.Instance.dropDownTime = Time.time + 0.5f;
        //        CloseMenu();
    }

    public void Next()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        CloseMenu();
    }
    public void BuyGames()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        GameObject.Find("Canvas").transform.Find("GemsShop").gameObject.SetActive(true);
    }

    public void Buy(GameObject pack)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (pack.name == "Pack1")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
        }

        if (pack.name == "Pack2")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
        }
        if (pack.name == "Pack3")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
        }
        if (pack.name == "Pack4")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
        }
        InitScript.Instance.PurchaseSucceded();
        CloseMenu();

    }
    public void BuyLifeShop()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Lifes < InitScript.CapOfLife)
            GameObject.Find("Canvas").transform.Find("LiveShop").gameObject.SetActive(true);

    }
    public void BuyLife(GameObject button)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text.Replace("￥", "")))
        {
            InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text.Replace("￥", "")));
            InitScript.Instance.RestoreLifes();
            CloseMenu();
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("GemsShop").gameObject.SetActive(true);
        }

    }



    void ShowGameOver()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().gameOver);

        GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        gameObject.SetActive(false);

    }

    #region 设置相关
    public void ShowSettings(GameObject menuSettings)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (!menuSettings.activeSelf)
        {
            menuSettings.SetActive(true);
            //           menuSettings.GetComponent<SequencePlayer>().Play();
        }
        else menuSettings.SetActive(false);
    }

    public void SoundOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().volume = 0;
            InitScript.sound = false;

            Off.SetActive(true);
        }
        else
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().volume = 1;
            InitScript.sound = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Sound", (int)SoundBase.GetInstance().GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }
    public void MusicOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
            InitScript.music = false;

            Off.SetActive(true);
        }
        else
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
            InitScript.music = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Music", (int)GameObject.Find("Music").GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    public void Info()
    {
        if (Application.loadedLevelName == "Map" || Application.loadedLevelName == "menu")
            GameObject.Find("Canvas").transform.Find("Tutorial").gameObject.SetActive(true);
        else
            GameObject.Find("Canvas").transform.Find("PreTutorial").gameObject.SetActive(true);
    }

    public void Quit()
    {
        if (Application.loadedLevelName == "Game" || Application.loadedLevelName == "game")
            Application.LoadLevel("Map");
        else
            Application.Quit();
    }



    #endregion

    #region 道具物品

    public void FiveBallsBoost()
    {
        if (GamePlay.Instance.GameStatus != GameState.Playing) return;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Instance.FiveBallsBoost > 0)
        {
            if (GamePlay.Instance.GameStatus == GameState.Playing)
                InitScript.Instance.SpendBoost(BoostType.FiveBallsBoost);
        }
        else
        {
            OpenBoostShop(BoostType.FiveBallsBoost);
        }
    }
    public void ColorBallBoost()
    {
        if (GamePlay.Instance.GameStatus != GameState.Playing) return;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Instance.ColorBallBoost > 0)
        {
            if (GamePlay.Instance.GameStatus == GameState.Playing)
                InitScript.Instance.SpendBoost(BoostType.ColorBallBoost);
        }
        else
        {
            OpenBoostShop(BoostType.ColorBallBoost);
        }

    }
    public void FireBallBoost()
    {
        if (GamePlay.Instance.GameStatus != GameState.Playing) return;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Instance.FireBallBoost > 0)
        {
            if (GamePlay.Instance.GameStatus == GameState.Playing)
                InitScript.Instance.SpendBoost(BoostType.FireBallBoost);
        }
        else
        {
            OpenBoostShop(BoostType.FireBallBoost);
        }

    }

    public void OpenBoostShop(BoostType boosType)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        GameObject.Find("Canvas").transform.Find("BoostShop").gameObject.GetComponent<BoostShop>().SetBoost(boosType);
    }

    public void BuyBoost(BoostType boostType, int price)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Gems >= price)
        {
            InitScript.Instance.BuyBoost(boostType, 1, price);
            InitScript.Instance.SpendBoost(boostType);
            CloseMenu();
        }
        else
        {
            BuyGames();
        }
    }

    #endregion

}
