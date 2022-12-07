using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using DG.Tweening;
using KanKikuchi.AudioManager;


public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    private Tween _tween;

    [SerializeField] GameObject Camera;
    [SerializeField] Vector3 Cameraoffset;

    bool isJump;

    [SerializeField] float jumpMotionTime;
    [SerializeField] Vector3 localGravity;

    public static PlayerManager instance;

    [SerializeField] GameObject ArrowCanvas;

    bool BGM;
    bool fallSound;
    public bool move;


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
        // 数値の初期化
        Init();

        // コンポーネント取得
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.useGravity = false;


        // 設定次第で消す
        if (PlayerPrefs.GetInt("MoveMethod") == 0)
        {
            ArrowCanvas.SetActive(false);
            Cameraoffset.z = -5.0f;
        }
    }

    void Init()
    {
        isJump = false;
        jumpMotionTime = 10f;
        localGravity = new Vector3(0, jumpMotionTime * GameData.Entity.localGravity, 0);

        BGM = false;
        fallSound = false;
        move = false;
    }

    void Update()
    {
        // カメラ移動
        UpdateCameraPosition();

        // キャラ移動
        if (ScoreManager.instance.GetScore() != GameData.Entity.maxScore && move)
        {
            // キーボード操作
            MoveKeyBoard();

            // ０→タッチ操作　1→ボタン操作
            if (PlayerPrefs.GetInt("MoveMethod") == 0)
            {
                // タッチ操作
                MoveTouch();
            }

        }


        // 落下処理
        if (transform.position.y <= GameData.Entity.deadLine)
        {
            //スコアが最大値になっているか
            if (ScoreManager.instance.GetScore() != GameData.Entity.maxScore)
            {
                if (!fallSound)
                {
                    SEManager.Instance.Play(SEPath.GUAA,
                        volumeRate: 0.3f);
                    fallSound = true;

                    ScoreManager.instance.DisplayResultCanvas(false);

                }
            }
            else
            {
                ScoreManager.instance.DisplayResultCanvas(true);
            }
        }
    }

    void MoveKeyBoard()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            MoveLeft();
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            MoveCenter();
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            MoveRight();
        }
    }

    void MoveTouch()
    {
        if (Input.touchCount > 0)
        {
            // タッチ操作取得
            Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                var touchPos = touch.position.x;
                Debug.Log("スクリーンサイズyoko"+Screen.width);
                if(touchPos >= 0 && touchPos < Screen.width / 3)
                {
                    MoveLeft();
                }
                else if(touchPos >= Screen.width / 3 && touchPos <= (Screen.width/3 * 2))
                {
                    MoveCenter();
                }
                else if(touchPos > (Screen.width / 3 * 2) && touchPos <= Screen.width)
                {
                    MoveRight();
                }

            }
        }
    }

    private void FixedUpdate()
    {
        // 重力決定
        SetLocalGravity();
    }

    void JumpDirection(Vector3 direction)
    {
        // BGMを鳴らす
        if (!BGM)
        {
            BGM = true;

            BGMManager.Instance.Play(
            audioPath: BGMPath.BGM_MAIN,
            volumeRate: 0.1f,
            delay: 0f,
            pitch: 1,
            isLoop: true,
            allowsDuplicate: false);
        }

        if (!isJump)
        {
            // 連続で飛ばないようにする
            isJump = true;

            // アニメーション実行
            animator.speed = 0.8f * jumpMotionTime;
            animator.Play("Jump", 0, 0);

            // 最終到達位置を指定
            var endpos = transform.position + new Vector3(0, 0, 1.5f) + direction;

            // その方向を向く
            transform.LookAt(endpos);

            // ジャンプ
            transform.DOJump(endpos,// 最終到達位置
                                 1f,// 到達までの時間
                                  1,// 回数
                                  1f / jumpMotionTime)// アニメーション時間
                                  .SetEase(Ease.Linear)// イーズ種類
                                  .OnComplete(() => EndJumping());// 終わった後どうするか
        }
    }

    void EndJumping()
    {

    }

    void UpdateCameraPosition()
    {
        // カメラの位置を設定する
        Vector3 pos = transform.position;


        Vector3 cameraPos = new Vector3(transform.position.x + Cameraoffset.x,
                                        Cameraoffset.y,
                                        transform.position.z + Cameraoffset.z);


        // カメラ位置の更新
        Camera.transform.position = cameraPos;
    }

    private void SetLocalGravity()
    {
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Plane"))
        {
            // 落下時
            animator.Play("Clap", 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Floor"))
        {
            isJump = false;
        }
    }

    private void OnDisable()
    {
        // Tween
        if (DOTween.instance != null)
        {
            _tween?.Kill();
        }
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public void MoveCenter()
    {
        JumpDirection(new Vector3(0, 0, 0));
    }

    public void MoveLeft()
    {
        JumpDirection(new Vector3(-GameData.Entity.MoveDistance, 0, 0));
    }

    public void MoveRight()
    {
        JumpDirection(new Vector3(GameData.Entity.MoveDistance, 0, 0));
    }


    public void SetActiveMoveArrow(bool f)
    {
        ArrowCanvas.SetActive(f);
    }

    public void MoveFlag()
    {
        move = true;
    }
}
