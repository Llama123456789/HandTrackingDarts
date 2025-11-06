using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroOneModeSelector : MonoBehaviour
{
    [SerializeField]
    private DartboardScore dartBoardScore;
    [SerializeField]
    private int changeModeScore = 0;

    public void ModeChangeButton()
    {
        dartBoardScore.initialScore = changeModeScore;
        dartBoardScore.maxScore = changeModeScore;
    }
}
