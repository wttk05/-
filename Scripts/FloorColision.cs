using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;


public class FloorColision : MonoBehaviour
{
    bool fall;
    void Start()
    {
        fall = false;
    }

    void Update()
    {
        if (!fall) { return; }

        if(transform.position.y <= -5)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(0, -1f, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            ScoreManager.instance.AddScore(1);

            //SEÄ¶
            SEManager.Instance.Play(
                audioPath: SEPath.SE_CHAKUCHI02MP3CUTNET,
                volumeRate: 0.2f                          
                                   );

            Invoke("DestroyObject", 1.5f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            // —Ž‰º”»’è
            fall = true;
        }

    }

    void DestroyObject()
    {
        fall = true;
    }
}
