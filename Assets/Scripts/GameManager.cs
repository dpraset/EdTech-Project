using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public FinanceManager financeManager;

    // Starting money
    public float playerBalance = 1000f;
    public int day = 1;
    public bool isDay = true;

    public float totalEarnings;
    public float totalExpenses;
    
    // Scenario logic
    // Show a scenario every week
    public int daysUntilScenario = 7; 
    private int daysPassed = 0;

    // Income
    //public float dailyIncome = 150f;
    public List<Job> availableJobs = new List<Job>();
    public Job currentJob;
    private int daysSinceLastPay = 0;

    bool overtimeBonusActive = false;
    float overtimeBonusAmount = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game started. Day 1, Morning.");
        uiManager.UpdateDayText(day, isDay);
        uiManager.UpdateBalance(playerBalance);

        if (financeManager == null)
            financeManager = GetComponent<FinanceManager>();
        
        // Jobs List
        availableJobs.Add(new Job("Barista (Part-Time)", 80f, PaySchedule.Daily, true, "Serve coffee."));
        availableJobs.Add(new Job("QA Tester (Part-Time)", 1000f, PaySchedule.BiWeekly, true, "Test software for bugs"));
        availableJobs.Add(new Job("Software Engineer", 2000f, PaySchedule.Monthly, false, "Full-time development role"));

        // Default - No job
        currentJob = null;
        uiManager.UpdateCurrentJob(null);
        //Debug.Log("Current Job: " + currentJob.jobName);
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
            //uiManager.ShowFeedback("Night " + day);

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

            //uiManager.ShowFeedback("Day " + day + " - Start!");
            Debug.Log("Day " + day);
            financeManager.ProcessDailyExpenses(day);

            // Check if it’s time to show a scenario
            if (daysPassed >= daysUntilScenario)
            {
                daysPassed = 0;
                TriggerScenario();
            }
            // Check for game over condition
            CheckGameOver();
        }
    }

    void TriggerScenario()
    {
        ScenarioData scenario = GetRandomScenario();
        uiManager.ShowScenarioPanel(scenario);
    }

    ScenarioData GetRandomScenario()
    {

        // List of financial scenarios
        List<ScenarioData> scenarios = new List<ScenarioData>
        {
            
            // Scenario 1: Night out
            new ScenarioData
            {
                scenarioText = "Your friends invite you for a night out. What will you do?",
                difficulty = ScenarioData.ScenarioDifficulty.Easy,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Go out and splurge",
                        moneyChange = -200f,
                        outcomeText = "You went out and splurged your money, resulting in $200 in costs the next day."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Go out but spend wisely",
                        moneyChange = -40f,
                        outcomeText = "You had a good time with your friends, and only spent $40 on food and drinks."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Politely decline",
                        moneyChange = 0f,
                        outcomeText = "You decided to decline and stay at home."
                    }
                }
            },
            
            
            // Scenario 2: Friend Asks to Borrow Money
            new ScenarioData
            {
                scenarioText = "Your friend needs to borrow $100. They promise to pay back.",
                difficulty = ScenarioData.ScenarioDifficulty.Easy,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Lend the money",
                        moneyChange = -100f,
                        outcomeText = "You lent your friend money. There's a chance you won't get it back."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Say you can't afford it",
                        moneyChange = 0f,
                        outcomeText = "Your friend is saddened by your response and leaves."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Offer $50 instead",
                        moneyChange = -50f,
                        outcomeText = "You compromised with them. Let's hope it was worth it!"
                    }
                }
            },
            
            // Challenge 1: Impulse Purchase
            new ScenarioData
            {
                scenarioText = "Challenge: You see the new gaming console on sale for $400. It's 20% off!",
                difficulty = ScenarioData.ScenarioDifficulty.Medium,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Buy it now! (Impulse)",
                        moneyChange = -400f,
                        outcomeText = "Bought it, but the budget was impacted."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Wait and save up",
                        moneyChange = 0f,
                        outcomeText = ""
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Check your budget first",
                        moneyChange = 0f,
                        outcomeText = ""
                    }
                }
            }
        };
        
        // Return a random scenario
        return scenarios[Random.Range(0, scenarios.Count)];
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
            financeManager.RecordIncome(day, currentJob.jobName, currentJob.payAmount);
            daysSinceLastPay = 0;
            uiManager.ShowFeedback($"Day " + day + " - " + currentJob.jobName + " - Earned $" + currentJob.payAmount);
            Debug.Log($"Day {day}: Received ${currentJob.payAmount} from {currentJob.jobName}");
        }
    }

    void CheckGameOver()
    {
        // Game over if balance is negative for too long or below critical threshold
        if (playerBalance < -100f)
        {
            Debug.LogError("GAME OVER: Debt too high!");
            uiManager.ShowGameOver("You accumulated too much debt and couldn't recover.");
        }
    }

    void PauseBackground()
    {
        // pause bakground menu while other menu (scenario, challenge, pause) is active
    }
}
