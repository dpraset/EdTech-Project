using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FinanceManager : MonoBehaviour
{

    public GameManager gameManager;

    // Expenses
    public float rentAmount = 500f;
    // Pay rent every 30 days (monthly)
    public int rentDayInterval = 30;
    //public float dailyFoodCost = 20f;
    public float weeklyGroceriesCost = 100f;
    public float weeklyUtilitiesCost = 50f;

    // Track finances
    public float monthlyIncome = 0f;
    public float monthlyExpenses = 0f;
    public float totalSavings = 0f;

    private int daysSinceLastRent = 0;
    private int daysSinceLastUtilities = 0;
    private int daysSinceLastGroceries;
    private int currentMonth = 1;


    // Expense history
    [System.Serializable]
    public class ExpenseRecord
    {
        public int day;
        public string expenseType;
        public float amount;
        public string description;
        
        public ExpenseRecord(int d, string type, float amt, string desc)
        {
            day = d;
            expenseType = type;
            amount = amt;
            description = desc;
        }
    }

    [System.Serializable]
    public class IncomeRecord
    {
        public int day;
        public string source;
        public float amount;
        
        public IncomeRecord(int d, string src, float amt)
        {
            day = d;
            source = src;
            amount = amt;
        }
    }

    public List<ExpenseRecord> expenseHistory = new List<ExpenseRecord>();
    public List<IncomeRecord> incomeHistory = new List<IncomeRecord>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    public void ProcessDailyExpenses(int currentDay)
    {
        // Daily food expense
        //ProcessFoodExpense(currentDay);
        daysSinceLastGroceries++;
        if (daysSinceLastGroceries >= 7)
        {
            ProcessGroceriesExpense(currentDay);
            daysSinceLastGroceries = 0;
        }
        
        // Weekly utilities (every 7 days)
        daysSinceLastUtilities++;
        if (daysSinceLastUtilities >= 7)
        {
            ProcessUtilitiesExpense(currentDay);
            daysSinceLastUtilities = 0;
        }
        
        // Monthly rent
        daysSinceLastRent++;
        if (daysSinceLastRent >= rentDayInterval)
        {
            ProcessRentExpense(currentDay);
            daysSinceLastRent = 0;
        }
        
        // Check for new month (every 30 days)
        if (currentDay % 30 == 0 && currentDay > 0)
        {
            ProcessMonthEnd(currentDay);
        }
    }
    /**
    void ProcessFoodExpense(int day)
    {
        if (gameManager.playerBalance >= dailyFoodCost)
        {
            gameManager.AdjustBalance(-dailyFoodCost);
            RecordExpense(day, "Food", dailyFoodCost, "Daily food expenses");
            Debug.Log($"Day {day}: Paid ${dailyFoodCost} for food.");
        }
        else
        {
            // Can't afford food - this could trigger a consequence
            Debug.LogWarning($"Day {day}: Cannot afford food! Balance too low.");
            gameManager.uiManager.ShowFeedback("⚠️ You couldn't afford food today. Your health may suffer.");
        }
    }
    **/
    
    void ProcessGroceriesExpense(int day)
    {
        if (gameManager.playerBalance >= weeklyGroceriesCost)
        {
            gameManager.AdjustBalance(-weeklyGroceriesCost);
            RecordExpense(day, "Groceries", weeklyGroceriesCost, "Weekly groceries");
            Debug.Log($"Day {day}: Paid ${weeklyGroceriesCost} for groceries.");
        }
    }
    
    void ProcessUtilitiesExpense(int day)
    {
        if (gameManager.playerBalance >= weeklyUtilitiesCost)
        {
            gameManager.AdjustBalance(-weeklyUtilitiesCost);
            RecordExpense(day, "Utilities", weeklyUtilitiesCost, "Weekly utilities (electricity, water, internet)");
            Debug.Log($"Day {day}: Paid ${weeklyUtilitiesCost} for utilities.");
        }
        else
        {
            Debug.LogWarning($"Day {day}: Cannot afford utilities!");
            gameManager.uiManager.ShowFeedback("You don't have enough money for Utilities.");
        }
    }
    
    void ProcessRentExpense(int day)
    {
        if (gameManager.playerBalance >= rentAmount)
        {
            gameManager.AdjustBalance(-rentAmount);
            RecordExpense(day, "Rent", rentAmount, "Monthly rent payment");
            gameManager.uiManager.ShowFeedback($"Rent paid: ${rentAmount}");
            Debug.Log($"Day {day}: Paid ${rentAmount} for rent.");
        }
        else
        {
            // Late rent
            Debug.LogError($"Day {day}: Can't afford rent!");
            gameManager.uiManager.ShowFeedback("Rent is overdue!");
            RecordExpense(day, "Rent (missed)", 0f, "Failed to pay rent - OVERDUE");
            // cant pay rent for certain amount of days, fail
        }
    }
    
    void ProcessMonthEnd(int day)
    {
        currentMonth++;
        Debug.Log($"=== Month {currentMonth} Summary ===");
        Debug.Log($"Monthly Income: ${monthlyIncome:F2}");
        Debug.Log($"Monthly Expenses: ${monthlyExpenses:F2}");
        Debug.Log($"Net: ${(monthlyIncome - monthlyExpenses):F2}");
        
        // Reset monthly counters
        monthlyIncome = 0f;
        monthlyExpenses = 0f;
    }
    
    // Record expenses
    public void RecordExpense(int day, string type, float amount, string description)
    {
        expenseHistory.Add(new ExpenseRecord(day, type, amount, description));
        monthlyExpenses += amount;
        gameManager.totalExpenses += amount;
    }
    
    // Record income
    public void RecordIncome(int day, string source, float amount)
    {
        incomeHistory.Add(new IncomeRecord(day, source, amount));
        monthlyIncome += amount;
        gameManager.totalEarnings += amount;
    }
    
    // Calculate budget
    public float GetMonthlyIncome()
    {
        return monthlyIncome;
    }
    
    public float GetMonthlyExpenses()
    {
        return monthlyExpenses;
    }
    
    public float GetBudgetSurplus()
    {
        return monthlyIncome - monthlyExpenses;
    }
    
    public bool IsOverBudget()
    {
        return monthlyExpenses > monthlyIncome;
    }
    
    // Breakdown expenses
    public Dictionary<string, float> GetExpenseBreakdown()
    {
        Dictionary<string, float> breakdown = new Dictionary<string, float>();
        
        foreach (var expense in expenseHistory)
        {
            if (breakdown.ContainsKey(expense.expenseType))
                breakdown[expense.expenseType] += expense.amount;
            else
                breakdown[expense.expenseType] = expense.amount;
        }
        
        return breakdown;
    }
    
    // Savings system
    public void DepositToSavings(float amount)
    {
        if (gameManager.playerBalance >= amount)
        {
            gameManager.AdjustBalance(-amount);
            totalSavings += amount;
            Debug.Log($"Deposited ${amount} to savings. Total savings: ${totalSavings}");
        }
    }
    
    public void WithdrawFromSavings(float amount)
    {
        if (totalSavings >= amount)
        {
            totalSavings -= amount;
            gameManager.AdjustBalance(amount);
            Debug.Log($"Withdrew ${amount} from savings. Remaining: ${totalSavings}");
        }
    }
}
