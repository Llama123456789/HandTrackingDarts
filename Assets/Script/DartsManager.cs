using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tryDartsBase = new GameObject[3]; // ケースの基準Transform
    [SerializeField] private GameObject[] tryDarts = new GameObject[3]; // ダーツ本体
    [SerializeField] private GameObject[] tryDartsTip = new GameObject[3]; // Tip

    // 各ダーツの初期値
    private Vector3[] tryDartsFirstPos = new Vector3[3];
    private Quaternion[] tryDartsFirstRot = new Quaternion[3];
    private Rigidbody[] tryDartsRb = new Rigidbody[3];

    // ★ 各ダーツの状態マネージャー
    private DartsStateManager[] stateManagers = new DartsStateManager[3];

    void Start()
    {
        for (int i = 0; i < tryDarts.Length; i++)
        {
            // 初期位置・回転はケースBaseから取得
            tryDartsFirstPos[i] = tryDartsBase[i].transform.position;
            tryDartsFirstRot[i] = tryDartsBase[i].transform.rotation;

            // コンポーネント取得
            tryDartsRb[i] = tryDarts[i].GetComponent<Rigidbody>();
            stateManagers[i] = tryDarts[i].GetComponent<DartsStateManager>();

            // ★ 全ダーツをInCase状態で初期化
            ResetSingleDart(i);
        }
    }

    // ─── 全リセット（ハンドジェスチャーから呼ぶ）───────────

    public void DartInitialize()
    {
        for (int i = 0; i < tryDarts.Length; i++)
        {
            ResetSingleDart(i);
        }
    }

    // ─── 1本だけリセット ──────────────────────────────────

    private void ResetSingleDart(int index)
    {
        // 物理リセット
        tryDartsRb[index].velocity = Vector3.zero;
        tryDartsRb[index].angularVelocity = Vector3.zero;

        // ボードの親子関係を解除
        tryDarts[index].transform.SetParent(null);

        // 位置・回転をケース基準に戻す
        tryDarts[index].transform.position = tryDartsFirstPos[index];
        tryDarts[index].transform.rotation = tryDartsFirstRot[index];

        // Tipを復活
        tryDartsTip[index].SetActive(true);

        // ★ 状態をInCaseに戻す
        stateManagers[index].ReturnToCase();
    }
}
