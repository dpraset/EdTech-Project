using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public FinanceManager financeManager;
    public ConceptProgressionManager progressionManager;
    public ConceptsPanelManager conceptsPanelManager; // NEW

    // Starting money
    public float playerBalance = 1000f;
    public int day = 1;

    public float totalEarnings;
    public float totalExpenses;
    
    // Scenario tracking
    public int daysUntilScenario = 7; 
    private int daysPassed = 0;
    public int completedScenariosCount = 0;

    // Income
    public List<Job> availableJobs = new List<Job>();
    public Job currentJob;
    private int daysSinceLastPay = 0;

    void Start()
    {
        Debug.Log("Game started. Day 1.");
        uiManager.UpdateDayText(day);
        uiManager.UpdateBalance(playerBalance);

        if (financeManager == null)
            financeManager = GetComponent<FinanceManager>();
            
        if (progressionManager == null)
            progressionManager = GetComponent<ConceptProgressionManager>();
            
        if (conceptsPanelManager == null)
            conceptsPanelManager = FindObjectOfType<ConceptsPanelManager>();
        
        // Jobs List
        availableJobs.Add(new Job("Barista (Part-Time)", 80f, PaySchedule.Daily, true, "Serve coffee."));
        availableJobs.Add(new Job("QA Tester (Part-Time)", 1000f, PaySchedule.BiWeekly, true, "Test software for bugs"));
        availableJobs.Add(new Job("Software Engineer", 2000f, PaySchedule.Monthly, false, "Full-time development role"));

        // Default - No job
        currentJob = null;
        uiManager.UpdateCurrentJob(null);
        
        // Check for initial unlocks (Day 1)
        if (progressionManager != null)
            progressionManager.CheckUnlocks(day);
    }

    public void OnNextButtonPressed()
    {
        // Go directly to next day (no night cycle)
        day++;
        daysPassed++;
        daysSinceLastPay++;
        
        uiManager.UpdateDayText(day);
        ProcessPaycheck();

        Debug.Log($"Day {day}");
        financeManager.ProcessDailyExpenses(day);
        
        // Check for concept unlocks each day
        if (progressionManager != null)
            progressionManager.CheckUnlocks(day);

        // Check if it's time to show a scenario
        if (daysPassed >= daysUntilScenario)
        {
            daysPassed = 0;
            TriggerScenario();
        }
        
        // Check for game over condition
        CheckGameOver();
    }

    void TriggerScenario()
    {
        ScenarioData scenario = GetRandomScenario();
        uiManager.ShowScenarioPanel(scenario);
    }

    ScenarioData GetRandomScenario()
    {
        List<ScenarioData> scenarios = new List<ScenarioData>
        {
            new ScenarioData
            {
                scenarioText = "Your friends invite you for a night out. What will you do?",
                difficulty = ScenarioData.ScenarioDifficulty.Easy,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Go out and enjoy",
                        moneyChange = -200f,
                        outcomeText = "You went out, spending $200."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Go out but spend wisely",
                        moneyChange = -40f,
                        outcomeText = "You had fun with friends and set a budget beforehand."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Politely decline",
                        moneyChange = 0f,
                        outcomeText = "You declined and stayed home."
                    }
                }
            },
            
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
                        outcomeText = "You lent $100. Hope they pay you back!"
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Say you can't afford it",
                        moneyChange = 0f,
                        outcomeText = "You refused to lend money to your friend."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Compromise with them",
                        moneyChange = -50f,
                        outcomeText = "You compromised and lent $50."
                    }
                }
            },
            
            new ScenarioData
            {
                scenarioText = "You see a new TV on sale for $400. It's 20% off!",
                difficulty = ScenarioData.ScenarioDifficulty.Easy,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Buy it now!",
                        moneyChange = -400f,
                        outcomeText = "You bought it, though your wallet took a big hit."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Trade-in an older TV.",
                        moneyChange = -200f,
                        outcomeText = "You traded in an older TV for a newer one."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Wait for a better deal.",
                        moneyChange = 0f,
                        outcomeText = "You decide to wait for a better deal."
                    }
                }
            },

            new ScenarioData
            {
                scenarioText = "Your sink has a leak! This could lead to bigger problems in the future.",
                difficulty = ScenarioData.ScenarioDifficulty.Medium,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Try to repair it yourself.",
                        moneyChange = -40f,
                        outcomeText = "You did some DIY repairs, stopping the issue, for now."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Call for a handyman.",
                        moneyChange = -100f,
                        outcomeText = "The handyman fixed your sink without any further issues."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Ignore the issue.",
                        moneyChange = -400f,
                        outcomeText = "The leak got worse, and lead to a burst pipe! You have to get it replaced now."
                    }
                }
            },

            new ScenarioData
            {
                scenarioText = "A student asks you, 'Which of these is considered a 'fixed' expense?'",
                difficulty = ScenarioData.ScenarioDifficulty.Easy,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Groceries!",
                        moneyChange = 0f,
                        outcomeText = "Groceries are not a fixed expense. The student is sad and walks away."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Gas",
                        moneyChange = 0f,
                        outcomeText = "Gas prices are not actually a fixed expense. The student is sad and walks away."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Rent",
                        moneyChange = 20f,
                        outcomeText = "Rent is a fixed expense. The student is happy and gives you $20."
                    }
                }
            },

            new ScenarioData
            {
                scenarioText = "<b>FINANCIAL CHALLENGE</b>\n" + 
                "Your car suddenly breaks down and requires urgent repairs.",
                difficulty = ScenarioData.ScenarioDifficulty.Hard,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Buy a new car",
                        moneyChange = -40000f,
                        outcomeText = "Bought it, but your budget took a hit."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Buy an old car",
                        moneyChange = -25000f,
                        outcomeText = "Smart! Delayed gratification prevents financial stress."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Repair the car",
                        moneyChange = -800f,
                        outcomeText = "Good thinking! Always budget for wants."
                    }
                }
            },

            new ScenarioData
            {
                scenarioText = "There's a large retailer that has recently dropped in stock price. Shares are $80 each.",
                difficulty = ScenarioData.ScenarioDifficulty.Medium,
                choices = new ScenarioChoice[]
                {
                    new ScenarioChoice
                    {
                        choiceText = "Buy 25 shares!",
                        moneyChange = -2000f,
                        outcomeText = "Bought it, but your budget took a hit."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Buy 10 shares",
                        moneyChange = -800f,
                        outcomeText = "Smart! Delayed gratification prevents financial stress."
                    },
                    new ScenarioChoice
                    {
                        choiceText = "Ignore",
                        moneyChange = 0f,
                        outcomeText = "Good thinking! Always budget for wants."
                    }
                }
            }
        };
        
        return scenarios[Random.Range(0, scenarios.Count)];
    }

    public void AdjustBalance(float amount)
    {
        playerBalance += amount;
        uiManager.UpdateBalance(playerBalance);
        
        // Check balance-based unlocks
        if (progressionManager != null)
            progressionManager.CheckUnlocks(day);
    }

    public void OpenJobPanel()
    {
        uiManager.ShowJobSelection(availableJobs);
    }
    
    public void OpenConceptsPanel()
    {
        if (conceptsPanelManager != null)
        {
            conceptsPanelManager.OpenConceptsPanel();
        }
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
            uiManager.ShowFeedback($"Day {day} - {currentJob.jobName} - Earned ${currentJob.payAmount}");
            Debug.Log($"Day {day}: Received ${currentJob.payAmount} from {currentJob.jobName}");
        }
    }

    void CheckGameOver()
    {
        if (playerBalance < -100f)
        {
            Debug.LogError("GAME OVER: Debt too high!");
            uiManager.ShowGameOver("You accumulated too much debt and couldn't recover.");
        }
    }
    
    public void OnScenarioCompleted()
    {
        completedScenariosCount++;
        
        if (progressionManager != null)
            progressionManager.CheckUnlocks(day);
    }
}