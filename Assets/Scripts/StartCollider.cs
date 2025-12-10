using System;
using TMPro;
using UnityEngine;

public class StartCollider : MonoBehaviour
{
    public bool repStart;
    [SerializeField]
    public int repsDone;
    public float timeRemaining;
    public bool timerRunning;
    void Start()
    {
        repStart = false;
        repsDone = 0;
        timerRunning = false;
        AssignTime();
    }
    void Update()
    {
        if (timerRunning)
        {
            DisplayTime(timeRemaining);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; //Decrease time 
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (repStart && other.gameObject.CompareTag("MainControl"))
        {
            Debug.Log("Rep recorded!");
            SuccessRequirements();
            AssignTime();
        }
    }
    public void SetSuccess()
    {
        repsDone = 0;//Needs more input here
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MainControl"))
        {
            timerRunning = true;
        }
    }
    private void SuccessRequirements()
    {
        if (GameManagerScript.Instance.statChoice == "Strength")
            {
                if (timeRemaining == 0)
                {
                    Debug.Log("Success");
                    repsDone++;
                    TMP_Text changeRepsDone = GameObject.Find("repNumber").GetComponent<TMP_Text>();
                    changeRepsDone.text = "Reps: " + repsDone;
                    repStart = false;
                }
                else
                {
                    repStart = false;
                    timerRunning = false;
                    AssignTime();
                }
            }
        else if (GameManagerScript.Instance.statChoice == "Speed")
        {
            if (timeRemaining > 0)
            {
                repsDone++;
                TMP_Text changeRepsDone = GameObject.Find("repNumber").GetComponent<TMP_Text>();
                changeRepsDone.text = "Reps: " + repsDone;
                repStart = false;
            }
            else
            {
                repStart = false;
                timerRunning = false;
                AssignTime();
            }
        }
        else if (GameManagerScript.Instance.statChoice == "Endurance")
        {
            repsDone++;
            TMP_Text changeRepsDone = GameObject.Find("repNumber").GetComponent<TMP_Text>();
            changeRepsDone.text = "Reps: " + repsDone;
            repStart = false;
        }
        else
        {
            Debug.Log("No stat chosen!");
        }
    }
    private void AssignTime()
    {
        if (GameManagerScript.Instance.statChoice == "Strength")
        {
            timeRemaining = 3.0f;
        }
        else if (GameManagerScript.Instance.statChoice == "Speed")
        {
            timeRemaining = 2.0f;
        }
        else if (GameManagerScript.Instance.statChoice == "Endurance")
        {
            timeRemaining = 0f;
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; //Adjust so timer doesnt show less than 0
        float seconds = Mathf.FloorToInt(timeToDisplay);
        TMP_Text timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();

        if (timerText != null) // Check if the Text reference is assigned
        {
            timerText.text = seconds.ToString();
        }
    }
}
