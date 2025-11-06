using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DartboardScore : MonoBehaviour
{
    [SerializeField] 
    private Transform boardCenter; // 的の中心

    [SerializeField]
    private TextMeshProUGUI dartsScore;

    // セグメントの順番（一般的なダーツの並び）
    private int[] scoreOrder = {6,13,4,18,1,20,5,12,9,14,11,
                                8,16,7,19,3,17,2,15,10,6};

    public int gameModeNumber = 0;
    public int initialScore = 0;
    public int maxScore = 0;
    public int totalScore = 0;

    private float bullRadius = 0.00635f; // インブル
    private float outerBullRadius = 0.0159f;  // アウターブル

    private float tripleRingInner = 0.0725f; //0.0725f 0.099f
    private float tripleRingOuter = 0.0815f; //0.0815f 0.107f

    private float doubleRingInner = 0.1235f; //0.1235f 0.162f
    private float doubleRingOuter = 0.131f; //0.131f 0.170f

    private float boardRadius = 0.17f;   // ボード外周

    void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPos = collision.gameObject.transform.position;
        int score = GetScore(hitPos);
        Debug.Log("Score: " + score);
        int result = ScoreConversion(gameModeNumber, score);
        dartsScore.SetText(""+result);
        Debug.Log(hitPos);
        Debug.Log(collision.gameObject.name);
    }
    int GetScore(Vector3 hitPos)
    {
        // ワールド座標 → 的のローカル座標へ
        Vector3 localHit = boardCenter.InverseTransformPoint(hitPos);

        // XY平面で距離を計算（Zは無視、的の厚み方向）
        float distance = new Vector2(localHit.x, localHit.y).magnitude;
        Debug.Log(distance+"distance");

        // ブル判定
        if (distance <= bullRadius) return 50; // インブル
        if (distance <= outerBullRadius) return 25; // アウターブル

        // 角度（Y軸を上、X軸を右と仮定）
        float angle = Mathf.Atan2(localHit.y, localHit.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        Debug.Log(localHit.x+","+ localHit.y);
        Debug.Log(angle+"angle");

        // 1セクターの角度 = 18度
        int sectorIndex = Mathf.FloorToInt((angle+9) / 18f);
        Debug.Log(sectorIndex + "セクター");
        int baseScore = scoreOrder[sectorIndex];

        // リング判定
        if (distance >= tripleRingInner && distance <= tripleRingOuter) return baseScore * 3;
        if (distance >= doubleRingInner && distance <= doubleRingOuter) return baseScore * 2;
        if (distance > boardRadius) return 0; // 外れ
    
        return baseScore;
    }

    int ScoreConversion(int gameModeNum,int score)
    {
        switch (gameModeNum)
        {
            case 0://フリースロー
                totalScore += score;
                break;
            case 1://カウントアップ
                totalScore += score; 
                break;
            case 2://ゼロワン
                if (initialScore - (maxScore - score) == 301) Debug.Log("GameClear"); //ゼロワンクリア時
                else if(initialScore - (maxScore-score) <= initialScore)　　　　　　　//0点を超えてしまう時
                { 
                    maxScore -= score;
                    totalScore = maxScore;
                }
                break;
            default:
                Debug.Log("存在しないゲームモードです。");
                break;

        }
        return totalScore;
    }
}
