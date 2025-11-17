using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConceptsPanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject conceptsPanel;
    public TextMeshProUGUI descriptionText;
    
    [Header("Concept Buttons")]
    public Button budgetButton;
    public Button expensesButton;
    public Button incomeButton;
    public Button savingsButton;
    public Button creditButton;
    public Button debtButton;
    public Button investmentsButton;
    
    [Header("Navigation")]
    public Button backButton;
    
    void Start()
    {
        // Setup button listeners
        if (budgetButton != null)
            budgetButton.onClick.AddListener(() => ShowConceptDescription("Budget"));
            
        if (expensesButton != null)
            expensesButton.onClick.AddListener(() => ShowConceptDescription("Expenses"));
            
        if (incomeButton != null)
            incomeButton.onClick.AddListener(() => ShowConceptDescription("Income"));

        if (savingsButton != null)
            savingsButton.onClick.AddListener(() => ShowConceptDescription("Savings"));

        if (creditButton != null)
            creditButton.onClick.AddListener(() => ShowConceptDescription("Credit"));
            
        if (debtButton != null)
            debtButton.onClick.AddListener(() => ShowConceptDescription("Debt"));
            
        if (investmentsButton != null)
            investmentsButton.onClick.AddListener(() => ShowConceptDescription("Investments"));
            
        if (backButton != null)
            backButton.onClick.AddListener(CloseConceptsPanel);
        
        // Start with panel closed
        if (conceptsPanel != null)
            conceptsPanel.SetActive(false);
    }
    
    public void OpenConceptsPanel()
    {
        if (conceptsPanel != null)
        {
            conceptsPanel.SetActive(true);
            // Show default description or leave blank
            if (descriptionText != null)
                descriptionText.text = "Select a concept to learn more about it.";
        }
    }
    
    public void CloseConceptsPanel()
    {
        if (conceptsPanel != null)
            conceptsPanel.SetActive(false);
    }
    
    void ShowConceptDescription(string conceptName)
    {
        if (descriptionText == null) return;
        
        string description = GetConceptDescription(conceptName);
        descriptionText.text = description;
        
        Debug.Log($"Showing description for: {conceptName}");
    }
    
    string GetConceptDescription(string conceptName)
    {
        switch (conceptName)
        {
            case "Budget":
                return "<b>Budgeting Basics</b>\n\n" +
                       "A budget is a plan for your money. It helps you track what comes in (income) and what goes out (expenses).\n\n" +
                       "Budgeting is the practice of creating a plan to manage your money by tracking income and expenses.\n\n" +
    
                       "<b>Why Budget?</b>\n" +
                       "• A budget helps individuals or households ensure they have enough funds to cover essential needs like housing, food, transportation, and savings, while also identifying areas where spending can be reduced.\n" +
                       "• By implementing a budget, people can avoid overspending, increase savings, and work toward financial goals like buying a home, reducing debt, or building an emergency fund.";
            
            case "Expenses":
                return "<b>Expenses & Bills</b>\n\n" +
                       "Expenses are the costs you pay to live and function daily.\n\n" +
                       "<b>Types of Expenses:</b>\n" +
                       "• <b>Fixed:</b> Same amount each month (rent, car payment, insurance)\n" +
                       "• <b>Variable:</b> Changes monthly (groceries, utilities, gas)\n" +
                       "• <b>Discretionary:</b> Optional spending (entertainment, dining out)\n\n" +
                       "<b>Common Bills:</b>\n" +
                       "• Rent/Mortgage (usually biggest expense)\n" +
                       "• Utilities (electricity, water, internet)\n" +
                       "• Groceries\n" +
                       "• Transportation\n" +
                       "• Insurance\n\n" +
                       "💡 Tip: Pay essential bills first, then allocate money for wants!";
            
            case "Income":
                return "<b>Income</b>\n\n" +
                       "An emergency fund is money saved specifically for unexpected expenses. It's your financial safety net!\n\n" +
                       "Income is the money earned or received by an individual or household, typically from sources such as employment wages, freelance work, investments, or government assistance.\n" +
                       "• It determines how much can be allocated to spending, saving, and investing.\n" +
                       "• Managing income effectively involves ensuring that spending does not exceed earnings and that some portion is allocated toward savings and debt repayment";

            case "Savings":
                return "<b>Savings</b>\n\n" +
                       "Savings refers to the money that is set aside for future use instead of being spent.\n\n" +
                       "<b>How Much to Save:</b>\n" +
                       "• These funds are often kept in a savings account, emergency fund, or other low-risk financial instrument.\n" +
                       "• Saving is important for achieving financial security, funding future goals, and protecting against unexpected expenses like medical bills or car repairs\n" +
                       "• Consistent saving habits help individuals avoid debt and work toward long-term financial independence.";

            case "Credit":
                return "<b>Credit</b>\n\n" +
                       "Credit is the ability to borrow money or access goods or services with the promise to repay later, often with interest.\n\n" +
                       "• Credit can come in the form of loans, credit cards, or lines of credit.\n" +
                       "• It is a valuable financial tool when used responsibly, allowing people to make large purchases or handle emergencies.\n" +
                       "• However, misuse of credit can lead to high-interest debt and poor credit scores, making it harder to borrow in the future.";
            
            case "Debt":
                return "<b>Debt</b>\n\n" +
                       "Debt is the amount of money that is owed to lenders or creditors. It can include credit card balances, student loans, mortgages, or personal loans.\n\n" +
                       "<b>Credit Cards:</b>\n" +
                       "• While some types of debt (like mortgages or student loans) can be helpful investments toward building wealth or skills, high-interest debt (like credit card debt) can quickly become harmful\n" +
                       "• Managing debt wisely involves paying bills on time, prioritizing high-interest debts, and avoiding excessive borrowing.";
            
            case "Investments":
                return "<b>Investing</b>\n\n" +
                       "Investing is putting your money to work to grow over time.\n\n" +
                       "• It is used to purchase assets (like stocks, bonds, real estate, or mutual funds) with the goal of generating a return over time.\n" +
                       "• Effective investing requires an understanding of higher risk vs reward, diversification, and long-term planning.";
            
            default:
                return "Select a concept to learn more about it.";
        }
    }
}