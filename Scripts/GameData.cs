using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    //ScriptableObject保存してある場所のパス
    public const string PATH = "GameData";

    //MyScriptableObjectの実体
    private static GameData _entity;
    public static GameData Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<GameData>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }

    public int maxScore;
    public float localGravity;
    public float MoveDistance;
    public int deadLine;

}
