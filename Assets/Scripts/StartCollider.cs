using System;
using TMPro;
using UnityEngine;

public class StartCollider : MonoBehaviour
{
    public bool repStart;
    public float timeRemaining;
    public bool timerRunning;
    void Start()
    {
        repStart = false;
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
            SuccessRequirements();
            AssignTime();
        }
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
                    GameManagerScript.Instance.RepSuccess();
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
                GameManagerScript.Instance.RepSuccess();
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
            GameManagerScript.Instance.RepSuccess();
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
        if (GameManagerScript.Instance.statChoice == "Strength" || GameManagerScript.Instance.statChoice == "Speed")
        {
            GameObject Timer = GameObject.Find("Timer");
            if (Timer == null)
            {
                return;
            }
            Timer.SetActive(true);
            timeToDisplay += 1; //Adjust so timer doesnt show less than 0
            float seconds = Mathf.FloorToInt(timeToDisplay);
            TMP_Text timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();

            if (timerText != null) // Check if the Text reference is assigned
            {
                timerText.text = seconds.ToString();
            }
        }
        else
        {
            GameObject timerText = GameObject.Find("Timer");
            timerText.SetActive(false);
        }
    }
}
