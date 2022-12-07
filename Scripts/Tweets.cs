using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Tweets : MonoBehaviour
{
    public void PushShareButton()
    {

        StartCoroutine(_Share());
    }

    // SNS
    public IEnumerator _Share()
    {
        string imgPath = Application.persistentDataPath + "/image.png";



        File.Delete(imgPath);


        while (true)
        {
            if (!File.Exists(imgPath)) break;
            yield return null;
        }

        ScreenCapture.CaptureScreenshot("image.png");

        while (true)
        {
            if (File.Exists(imgPath)) break;
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        string tweetText;

        if (PlayerPrefs.GetFloat("highScore") == 0)
        {
            tweetText = "スコアはありません。\n";
        }
        else
        {
            tweetText = "あなたのハイスコアは" + PlayerPrefs.GetFloat("highScore") + "秒です！ \n #太陽に向かって\n\n";
        }


#if UNITY_IPHONE
        string tweetURL = "https://itunes.apple.com/jp/app/id1588551861?mt=8";
#elif UNITY_ANDROID
        string tweetURL = "https://play.google.com/store/apps/details?id=com.WTTKApps&hl=ja&gl=US";
#endif


        SocialConnector.PostMessage(SocialConnector.ServiceType.Twitter, tweetText, tweetURL, imgPath);
    }

}
