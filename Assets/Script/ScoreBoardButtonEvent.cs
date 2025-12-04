using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardButtonEvent : MonoBehaviour
{
    [SerializeField]
    private int gameModeNum = 0;　　　//インスペクター上で数値を変更しモードを変更する
    [SerializeField]
    private int initializeScore = 0;　//表示されるスコアを初期化
    [SerializeField] 
    private TextMeshProUGUI dartsScore;
    [SerializeField]
    private DartboardScore dartBoardScore;

    private void Start()
    {
        //OnButtonPush();//テスト用
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == ("IndexFinger"))
        {
            OnButtonPush();

        }
    }
    public void OnButtonPush()
    {
        Debug.Log("ボタンが押された"+ gameModeNum);
        dartsScore.SetText(""+initializeScore);
        dartBoardScore.gameModeNumber = gameModeNum;
        dartBoardScore.initialScore = initializeScore;
        dartBoardScore.maxScore = initializeScore;
    }

    public void ScoreResetButton()
    {
        initializeScore = dartBoardScore.initialScore;
        dartBoardScore.maxScore = initializeScore;
        dartBoardScore.totalScore = 0;
        dartsScore.SetText("" + initializeScore);
    }//リセットボタン用
}
