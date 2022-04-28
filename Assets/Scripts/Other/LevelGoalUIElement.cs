using Level.Goals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalUIElement : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text textDisplay;
    [SerializeField] UnityEngine.UI.Image successImage, failedImage;

    internal void UpdateUI(LevelGoal.GoalState state)
    {
        if (state == LevelGoal.GoalState.Success)
            successImage.enabled = true;
        else
            successImage.enabled = false;

        if (state == LevelGoal.GoalState.Failed)
            failedImage.enabled = true;
        else
            failedImage.enabled = false;
    }
}
