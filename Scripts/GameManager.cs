using UnityEngine;
using Unity.Collections;


/*
 * �Q�[���S�̂̊Ǘ�
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
