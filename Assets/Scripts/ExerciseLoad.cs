using System;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseLoad : MonoBehaviour
{
    public enum ExerciseType
    {
        BicepCurl,
        Squat,
        ShoulderPress,
        None
    }
    public ExerciseType workout;
    private void Start()
    {
        Button UIButton = GetComponent<Button>();
        switch (workout)
        {
            case ExerciseType.BicepCurl: UIButton.onClick.AddListener(GameManagerScript.Instance.ChoseBicepCurl);
            break;
            case ExerciseType.Squat: UIButton.onClick.AddListener(GameManagerScript.Instance.ChoseSquat);
            break;
            case ExerciseType.ShoulderPress: UIButton.onClick.AddListener(GameManagerScript.Instance.ChoseShoulder);
            break;
            case ExerciseType.None: UIButton.onClick.AddListener(GameManagerScript.Instance.ResetExerciseChoice);
            break;
        }
    }
}
