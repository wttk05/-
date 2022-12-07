using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DigitalSalmon.Fade;



public class TitleManager : MonoBehaviour
{
    bool se;
    bool fade;
    [SerializeField] GameObject ViewHighScorePanel;
    [SerializeField] SuperTextMesh MyHighScore;



   // [SerializeField] PlayFabManager PlayFabManager;

    [SerializeField] AdMobBanner adMobBanner;

    [SerializeField]  Tweets tweets;

    [SerializeField] Sprite yubi;
    [SerializeField] Sprite button;

    // 画像を動的に変えたいボタンの宣言
    [SerializeField] GameObject moveModeButton;
    private Image spriteRenderer;

    //フェード用
    [SerializeField]
    protected FadePostProcess fadePostProcess;



    private void Start()
    {
        ViewHighScorePanel.SetActive(false);
        se = false;
        fade = false;

        BGMManager.Instance.Play(
        audioPath: BGMPath.BGM_TITLE,
        volumeRate: 0.05f,            
        delay: 0f,                    
        pitch: 1,                     
        isLoop: true,                 
        allowsDuplicate: false       
        );

        // SpriteRendererを所得する
        spriteRenderer = moveModeButton.GetComponent<Image>();


        // 値がなかった場合、０を登録する
        if (!PlayerPrefs.HasKey("MoveMethod"))
        {
            PlayerPrefs.SetInt("MoveMethod", 1);
            PlayerPrefs.Save();
            spriteRenderer.sprite = button;

        }
        else
        {
            if (PlayerPrefs.GetInt("MoveMethod") == 0)
            {
                spriteRenderer.sprite = yubi;
            }
            else
            {
                spriteRenderer.sprite = button;
            }
        }
       
    }

    public void GoMain()
    {
        if (se) { return; }

        SEManager.Instance.Play(SEPath.KETTEI
                 , volumeRate: 0.3f);

        se = true;
    }

    private void Update()
    {

        if (se && !SEManager.Instance.IsPlaying())
        {
            //　ゲームシーンへ移動
            if(!fade)
            {
                //　バナー広告を消す
                adMobBanner.BannerDestroy();

                // BGM停止
                BGMManager.Instance.Stop();

                //フェード開始
                Fade();
                fade = true;
            }
        }

    }
    public void ViewRanking()
    {
        // パネルを表示する
        ViewHighScorePanel.SetActive(true);

        // SE再生
        SEManager.Instance.Play(SEPath.BUTTON
            , volumeRate: 0.3f);

        // ランキング
        if (PlayerPrefs.GetFloat("highScore") == 0)
        {
            MyHighScore.text = "ありません";
        }
        else
        {
            // ハイスコアを表示
            MyHighScore.text = PlayerPrefs.GetFloat("highScore").ToString("F3") + "秒";
        }

        //バナー広告を消す
        adMobBanner.BannerDestroy();
    }

    public void SendTwitter()
    {
        // Twitterに送信する
        tweets.PushShareButton();

        Debug.Log("ナイスツイート👌");
    }

    public void closeRanking()
    {
        ViewHighScorePanel.SetActive(false);

        SEManager.Instance.Play(SEPath.BACK
        , volumeRate: 0.8f);

        //再度広告を表示
        adMobBanner.RequestBanner();
    }

    public void ChangeMoveMode()
    {
        if(PlayerPrefs.GetInt("MoveMethod") == 0)
        {
            PlayerPrefs.SetInt("MoveMethod", 1);
            PlayerPrefs.Save();
            spriteRenderer.sprite = button;
            SEManager.Instance.Play(SEPath.BUTTON
         , volumeRate: 0.3f);


        }
        else
        {
            PlayerPrefs.SetInt("MoveMethod", 0);
            PlayerPrefs.Save();
            spriteRenderer.sprite = yubi;
            SEManager.Instance.Play(SEPath.BUTTON
, volumeRate: 0.3f);
        }
    }

    void Fade()
    {
        fadePostProcess.FadeDown(false, () => SceneManager.LoadScene("main"));
    }

}
