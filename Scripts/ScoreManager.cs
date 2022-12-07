using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using KanKikuchi.AudioManager;
using UnityEngine.InputSystem;
using UnityEditor;
using DigitalSalmon.Fade;




public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private Tween _tween;


    int score;
    [SerializeField] Canvas scoreCanvas;
    [SerializeField] SuperTextMesh speedUpText;


    [SerializeField] SuperTextMesh scoreText;


    float time;
    [SerializeField] SuperTextMesh timeText;


    [SerializeField] Canvas resultCanvas;
    [SerializeField] GameObject resultPanel;
    [SerializeField] SuperTextMesh resultText;

    [SerializeField] GameObject SendScoreButton;

    [SerializeField] AdMobInterstitial admob;


    [SerializeField] GameObject viewHighScorePanel;
    [SerializeField] SuperTextMesh MyHighScoreText;

    [SerializeField] Tweets tweets;

    [SerializeField]
    protected FadePostProcess fadePostProcess;


    bool DON;
    bool ue;
    public bool goTitle;
    bool viewAds;
    bool save;
    bool fade;

    private void Awake()
    {
        // インスタンス生成
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _tween = null;

    }


    void Start()
    {
        score = 0;
        time = 0;
        DON = false;
        ue = false;
        goTitle = false;
        viewAds = false;
        fade = false;
        resultText.baseOffset.x = 0.25f;

        resultCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(true);
        viewHighScorePanel.SetActive(false);
    }

    void Update()
    {
        scoreText.text = score.ToString();

        // 開始
        if (score >= 1 && score <= GameData.Entity.maxScore - 1)
        {
            time += Time.deltaTime;
            if (time >= 99.999f)
                time = 99.999f;

            timeText.text = "time : "+time.ToString("F3");
        }

        if (score == GameData.Entity.maxScore)
        {
            //インターセクシャル広告表示
            ViewAdMobInterstitial();
            // 
            DisplayResultCanvas(true);

            resultText.text = "あなたのタイムは\r\n<c=red>" + time.ToString("F3") + "</c>秒\r\nです！";
            resultText.baseOffset.x = 0f;
            resultText.baseOffset.y = 1.5f;
            resultText.lineSpacing = 2;
            resultText.size = 130;

            float t = float.Parse(time.ToString("F3"));

            // データ保存するか確認
            if (!save)
            {
                SaveScore(t);
                save = true;
            }
        }



        //広告が消えていて、タイトルボタンが押されていた場合
        if (!admob.displayAds && goTitle)
        {
            //タイトルに戻る
            SceneManager.LoadScene("title");
        }

        if (ue)
        {
            if (Keyboard.current.upArrowKey.isPressed)
            {
                Reset();
            }
        }

    }

    public void AddScore(int s)
    {
        score = score + s;
        FloorManager.instance.CreateFloor();
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTimer()
    {
        return time;
    }

    public void DisplayResultCanvas(bool b)
    {
        BGMManager.Instance.Stop();

        //矢印の設定だったら矢印を消す
        if(PlayerPrefs.GetInt("MoveMethod") == 1)
        {
            PlayerManager.instance.SetActiveMoveArrow(false);
        }

        // クリアしないとランキングボタンを表示させない
        SendScoreButton.SetActive(b);

        if (!DON)
        {
            SEManager.Instance.Play(SEPath.DON,
                volumeRate: 0.3f);
            DON = true;

            resultCanvas.gameObject.SetActive(true);
            resultPanel.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.OutExpo).OnComplete(() => Debug.Log("パネル移動おわり"));
            scoreCanvas.gameObject.SetActive(false);
        }

        ue = true;
    }

    public void CreateScoreText(int score)
    {
        Instantiate(scoreText, PlayerManager.instance.GetPos(), Quaternion.identity);
    }

    public void Reset()
    {
        if (!fade)
        {
            SEManager.Instance.Play(SEPath.BUTTON
                , volumeRate: 0.3f);

            DOTween.Clear(true);

            Fade();
            fade = true;
        }
    }

    public void GoTitle()
    {
        if(!admob.viewAds)
        {
            //インターセクシャル広告表示
            ViewAdMobInterstitial();
        }
        goTitle = true;
    }

    public void ViewRanking()
    {
        // ハイスコアランキング用パネルを表示する
        viewHighScorePanel.SetActive(true);

        //SE
        SEManager.Instance.Play(SEPath.BUTTON
            , volumeRate: 0.3f);

        if (PlayerPrefs.GetFloat("highScore") == 0)
        {
            MyHighScoreText.text = "ありません";
        }
        else
        {
            // ハイスコアを表示
            MyHighScoreText.text = PlayerPrefs.GetFloat("highScore").ToString("F3") + "秒";
        }

    }

    private void SaveScore(float t)
    {
        //　現在のスコアがハイスコアより小さかった場合
        if (PlayerPrefs.GetFloat("highScore") == 0)
        {
            // ハイスコアを保存
            PlayerPrefs.SetFloat("highScore", t);
            PlayerPrefs.Save();
        }

        // ハイスコア更新
        if (t < PlayerPrefs.GetFloat("highScore"))
        {
            // ハイスコアを保存
            PlayerPrefs.SetFloat("highScore", t);
            PlayerPrefs.Save();
        }
    }

    private void OnDisable()
    {
        DOTween.Clear(true);

        // Tween
        if (DOTween.instance != null)
        {
            _tween?.Kill();
        }

        //メモリ解放
        MemoryRelease();
    }

    void ViewAdMobInterstitial()
    {
        if (!viewAds)
        {
            // 広告表示
            admob.ShowAdMobInterstitial();
            viewAds = true;
        }
    }

    void MemoryRelease()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        Debug.Log("かいほう");

    }


    public void closeRanking()
    {
        viewHighScorePanel.SetActive(false);

        // SE
        SEManager.Instance.Play(SEPath.BACK
            , volumeRate: 0.8f);
    }


    public void SendTwitter()
    {
        // Twitterに送信する
        tweets.PushShareButton();

        Debug.Log("ナイスツイート👌");
    }

    void Fade()
    {
        fadePostProcess.FadeDown(false, () => SceneManager.LoadScene("main"));
    }
}
