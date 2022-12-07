using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class AdMobBanner : MonoBehaviour
{
    //やること
    //1.バナー広告IDを入力
    //2.バナーの表示位置　(現状表示位置は下になっています。)
    //3.バナー表示のタイミング (現状 起動直後になっています。)

    private BannerView bannerView;//BannerView型の変数bannerViewを宣言　この中にバナー広告の情報が入る


    //シーン読み込み時からバナーを表示する
    //最初からバナーを表示したくない場合はこの関数を消してください。
    private void Start()
    {
        RequestBanner();//アダプティブバナーを表示する関数 呼び出し
    }

    //ボタン等に割り付けて使用
    //バナーを表示する関数
    public void BannerStart()
    {
        RequestBanner();//アダプティブバナーを表示する関数 呼び出し       
    }

    //ボタン等に割り付けて使用
    //バナーを削除する関数
    public void BannerDestroy()
    {
        bannerView.Hide();
        bannerView.Destroy();//バナー削除
    }

    //アダプティブバナーを表示する関数
    public void RequestBanner()
    {
        //AndroidとiOSで広告IDが違うのでプラットフォームで処理を分けます。
        // 参考
        //【Unity】AndroidとiOSで処理を分ける方法
        // https://marumaro7.hatenablog.com/entry/platformsyoriwakeru

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3288303606086827/3951259932";//ここにAndroidのバナーIDを入力

#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3288303606086827/1345680841";//ここにiOSのバナーIDを入力

#else
        string adUnitId = "unexpected_platform";
#endif

        // 新しい広告を表示する前にバナーを削除
        if (bannerView != null)//もし変数bannerViewの中にバナーの情報が入っていたら
        {
            bannerView.Destroy();//バナー削除
        }

        //現在の画面の向き横幅を取得しバナーサイズを決定
        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);


        //バナーを生成 new BannerView(バナーID,バナーサイズ,バナー表示位置)
        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);//バナー表示位置は
                                                                               //画面上に表示する場合：AdPosition.Top
                                                                               //画面下に表示する場合：AdPosition.Bottom


        //BannerView型の変数 bannerViewの各種状態 に関数を登録
        bannerView.OnAdLoaded += HandleAdLoaded;//bannerViewの状態が バナー表示完了 となった時に起動する関数(関数名HandleAdLoaded)を登録
        bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;//bannerViewの状態が バナー読み込み失敗 となった時に起動する関数(関数名HandleAdFailedToLoad)を登録


        //リクエストを生成
        AdRequest adRequest = new AdRequest.Builder()
            .Build();

        //広告表示
        bannerView.LoadAd(adRequest);
    }


    #region Banner callback handlers

    //バナー表示完了 となった時に起動する関数
    public void HandleAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("バナー表示完了");
    }

    //バナー読み込み失敗 となった時に起動する関数
    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("バナー読み込み失敗" + args.LoadAdError);//args.Message:エラー内容        
    }

    #endregion

}