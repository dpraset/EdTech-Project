using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

    // Starting money
    public float playerBalance = 1000f;
    public int day = 1;
    public bool isDay = true;

    public float totalEarnings;
    public float totalExpenses;
    
    // Scenario logic
    // Show a scenario every 3 days
    public int daysUntilScenario = 3; 
    private int daysPassed = 0;

    // Income
    //public float dailyIncome = 150f;
    public List<Job> availableJobs = new List<Job>();
    public Job currentJob;
    private int daysSinceLastPay = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game started. Day 1, Morning.");
        uiManager.UpdateDayText(day, isDay);
        uiManager.UpdateBalance(playerBalance);
        
        // Jobs List
        availableJobs.Add(new Job("Barista (Part-Time)", 80f, PaySchedule.Daily, true));
        availableJobs.Add(new Job("QA Tester (Part-Time)", 1000f, PaySchedule.BiWeekly, true));
        availableJobs.Add(new Job("Software Engineer", 2000f, PaySchedule.Monthly, false));

        // Default - No job
        currentJob = null;
        uiManager.UpdateCurrentJob(null);
        Debug.Log("Current Job: " + currentJob.jobName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNextButtonPressed()
    {
        if (isDay)
        {
            // Transition from Day to Night
            isDay = false;
            uiManager.UpdateDayText(day, isDay);

            Debug.Log("Evening to Day " + day);
        }
        else
        {
            // Transition from Night to Next Day
            isDay = true;
            day++;
            daysPassed++;
            daysSinceLastPay++;
            uiManager.UpdateDayText(day, isDay);
            ProcessPaycheck();

            Debug.Log("Day " + day);

            // Check if it’s time to show a scenario
            if (daysPassed >= daysUntilScenario)
            {
                daysPassed = 0;
                TriggerScenario();
            }
        }
    }

    void TriggerScenario()
    {
        //Debug.Log("A random financial scenario appears!");
        uiManager.ShowScenarioPanel("You need to pay rent this week. What will you do?");
    }

    public void AdjustBalance(float amount)
    {
        playerBalance += amount;
        uiManager.UpdateBalance(playerBalance);
    }

    
    /**void ProcessWork()
    {
        if (isDay)
        {
            Debug.Log("You worked your day shift!");
            AdjustBalance(dailyIncome);
        }
        else
        {
            Debug.Log("You chose to rest tonight.");
        }
    } **/

    public void OpenJobPanel()
    {
        uiManager.ShowJobSelection(availableJobs);
    }

    void ProcessPaycheck()
    {
        bool shouldPay = false;
        if (currentJob == null) return;

        switch (currentJob.paySchedule)
        {
            case PaySchedule.Daily:
                shouldPay = true;
                break;
            case PaySchedule.Weekly:
                if (daysSinceLastPay >= 7) shouldPay = true;
                break;
            case PaySchedule.BiWeekly:
                if (daysSinceLastPay >= 14) shouldPay = true;
                break;
            case PaySchedule.Monthly:
                if (daysSinceLastPay >= 30) shouldPay = true;
                break;
        }

        if (shouldPay)
        {
            AdjustBalance(currentJob.payAmount);
            daysSinceLastPay = 0;
            //uiManager.ShowNotification($"You received ${currentJob.payAmount} from your {currentJob.paySchedule} job!");
            Debug.Log($"You received your {currentJob.paySchedule} paycheck of ${currentJob.payAmount} from your job as a {currentJob.jobName}!");
        }
    }
}
