using UnityEngine;
using Unity.Collections;


/*
 * ゲーム全体の管理
 *  
 */
public class GameManager : MonoBehaviour
{

    float count;

    void Start()
    {
        //NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        Application.targetFrameRate = 60;
    }

}
