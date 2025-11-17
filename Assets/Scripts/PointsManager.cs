using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointsManager : MonoBehaviour
{

    public TextMeshProUGUI pointsText;

    public GameObject rewardsPanel;
    public Transform rewardsContainer;
    public GameObject rewardItemPrefab;
    public Button openRewardsButton;
    public Button closeRewardsButton;

    public int currentPoints = 0;

    private List<Reward> allRewards;
    private List<Reward> unlockedRewards = new List<Reward>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePointsDisplay();
        InitializeRewards();
        
        if (openRewardsButton != null)
            openRewardsButton.onClick.AddListener(OpenRewardsPanel);
            
        if (closeRewardsButton != null)
            closeRewardsButton.onClick.AddListener(CloseRewardsPanel);
            
        if (rewardsPanel != null)
            rewardsPanel.SetActive(false);
    }

    public void AddPoints(int amount)
    {
        currentPoints += amount;
        UpdatePointsDisplay();
        Debug.Log($"Points added: +{amount}. Total: {currentPoints}");
        
        // Check if any rewards can be unlocked
        CheckUnlockableRewards();
    }
    
    public void SpendPoints(int amount)
    {
        if (currentPoints >= amount)
        {
            currentPoints -= amount;
            UpdatePointsDisplay();
            Debug.Log($"Points spent: -{amount}. Remaining: {currentPoints}");
        }
        else
        {
            Debug.LogWarning("Not enough points!");
        }
    }
    
    void UpdatePointsDisplay()
    {
        if (pointsText != null)
            pointsText.text = $"⭐ Points: {currentPoints}";
    }
    
    void InitializeRewards()
    {
        allRewards = new List<Reward>
        {
            // Avatar Customization Rewards
            new Reward
            {
                rewardName = "Cool Sunglasses",
                description = "Stylish shades for your avatar",
                cost = 50,
                category = RewardCategory.AvatarCustomization,
                icon = "🕶️"
            },
            new Reward
            {
                rewardName = "Fancy Hat",
                description = "A fashionable hat",
                cost = 75,
                category = RewardCategory.AvatarCustomization,
                icon = "🎩"
            },
            new Reward
            {
                rewardName = "Cool Jacket",
                description = "A stylish jacket",
                cost = 100,
                category = RewardCategory.AvatarCustomization,
                icon = "🧥"
            },
            
            // Game Boosts
            new Reward
            {
                rewardName = "Lucky Bonus",
                description = "Get a random cash bonus ($50-$200)",
                cost = 30,
                category = RewardCategory.GameBoost,
                icon = "💵"
            },
            new Reward
            {
                rewardName = "Expense Shield",
                description = "Protect from next negative challenge",
                cost = 40,
                category = RewardCategory.GameBoost,
                icon = "🛡️"
            },
            new Reward
            {
                rewardName = "Double Paycheck",
                description = "Next paycheck is doubled!",
                cost = 60,
                category = RewardCategory.GameBoost,
                icon = "💰"
            },
            
            // Badges/Achievements (auto-unlock)
            new Reward
            {
                rewardName = "Financial Novice Badge",
                description = "Earned 10 points",
                cost = 0,
                category = RewardCategory.Badge,
                icon = "🥉"
            },
            new Reward
            {
                rewardName = "Budget Master Badge",
                description = "Earned 100 points",
                cost = 0,
                category = RewardCategory.Badge,
                icon = "🥈"
            },
            new Reward
            {
                rewardName = "Financial Guru Badge",
                description = "Earned 500 points",
                cost = 0,
                category = RewardCategory.Badge,
                icon = "🥇"
            },
            
            // Special Unlocks
            new Reward
            {
                rewardName = "Budget Dashboard",
                description = "Unlock advanced budget tracking",
                cost = 150,
                category = RewardCategory.Feature,
                icon = "📊"
            },
            new Reward
            {
                rewardName = "Investment Simulator",
                description = "Unlock stock market feature",
                cost = 200,
                category = RewardCategory.Feature,
                icon = "📈"
            }
        };
        
        LoadRewardsUI();
    }
    
    void LoadRewardsUI()
    {
        if (rewardsContainer == null || rewardItemPrefab == null)
        {
            Debug.LogWarning("Rewards container or prefab not assigned!");
            return;
        }
        
        // Clear existing
        foreach (Transform child in rewardsContainer)
            Destroy(child.gameObject);
        
        foreach (var reward in allRewards)
        {
            GameObject item = Instantiate(rewardItemPrefab, rewardsContainer);
            
            // Find UI elements in prefab
            TextMeshProUGUI nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = item.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
            Button unlockButton = item.transform.Find("UnlockButton")?.GetComponent<Button>();
            
            bool isUnlocked = unlockedRewards.Contains(reward);
            
            if (nameText != null)
                nameText.text = $"{reward.icon} {reward.rewardName}\n<size=10>{reward.description}</size>";
            
            if (isUnlocked)
            {
                if (costText != null)
                    costText.text = "✓ UNLOCKED";
                if (unlockButton != null)
                    unlockButton.interactable = false;
            }
            else if (reward.cost == 0)
            {
                if (costText != null)
                    costText.text = "Achievement";
                if (unlockButton != null)
                    unlockButton.gameObject.SetActive(false);
            }
            else
            {
                if (costText != null)
                    costText.text = $"{reward.cost} pts";
                    
                if (unlockButton != null)
                {
                    unlockButton.interactable = currentPoints >= reward.cost;
                    Reward rewardCopy = reward; // Capture for lambda
                    unlockButton.onClick.RemoveAllListeners();
                    unlockButton.onClick.AddListener(() => UnlockReward(rewardCopy));
                }
            }
        }
    }
    
    void UnlockReward(Reward reward)
    {
        if (currentPoints >= reward.cost && !unlockedRewards.Contains(reward))
        {
            SpendPoints(reward.cost);
            unlockedRewards.Add(reward);
            
            // Apply reward effect
            ApplyRewardEffect(reward);
            
            // Refresh UI
            LoadRewardsUI();
            
            Debug.Log($"Unlocked: {reward.rewardName}");
        }
    }
    
    void ApplyRewardEffect(Reward reward)
    {
        GameManager gm = FindObjectOfType<GameManager>();
        
        switch (reward.category)
        {
            case RewardCategory.GameBoost:
                ApplyGameBoost(reward, gm);
                break;
            case RewardCategory.AvatarCustomization:
                Debug.Log($"Avatar updated with: {reward.rewardName}");
                if (gm != null && gm.uiManager != null)
                    gm.uiManager.ShowFeedback($"✨ Unlocked: {reward.rewardName}!");
                break;
            case RewardCategory.Feature:
                Debug.Log($"Feature unlocked: {reward.rewardName}");
                if (gm != null && gm.uiManager != null)
                    gm.uiManager.ShowFeedback($"🎉 Feature Unlocked: {reward.rewardName}!");
                break;
            case RewardCategory.Badge:
                Debug.Log($"Badge earned: {reward.rewardName}");
                break;
        }
    }
    
    void ApplyGameBoost(Reward reward, GameManager gm)
    {
        if (gm == null)
        {
            Debug.LogWarning("GameManager not found!");
            return;
        }
        
        if (reward.rewardName == "Lucky Bonus")
        {
            float bonus = Random.Range(50f, 200f);
            gm.AdjustBalance(bonus);
            if (gm.uiManager != null)
                gm.uiManager.ShowFeedback($"🎉 Lucky Bonus! +${bonus:F0}");
        }
        else if (reward.rewardName == "Double Paycheck")
        {
            // You could set a flag here for next paycheck to be doubled
            Debug.Log("Next paycheck will be doubled!");
            if (gm.uiManager != null)
                gm.uiManager.ShowFeedback("💰 Next paycheck will be DOUBLED!");
        }
        else if (reward.rewardName == "Expense Shield")
        {
            Debug.Log("Protected from next negative challenge!");
            if (gm.uiManager != null)
                gm.uiManager.ShowFeedback("🛡️ Protected from the next negative event!");
        }
    }
    
    void CheckUnlockableRewards()
    {
        // Auto-unlock badges based on points
        foreach (var reward in allRewards)
        {
            if (reward.category == RewardCategory.Badge && !unlockedRewards.Contains(reward))
            {
                if (reward.rewardName.Contains("Novice") && currentPoints >= 10)
                {
                    unlockedRewards.Add(reward);
                    Debug.Log($"🏆 Badge Unlocked: {reward.rewardName}");
                    
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (gm != null && gm.uiManager != null)
                        gm.uiManager.ShowFeedback($"🏆 Badge Unlocked: {reward.rewardName}");
                }
                else if (reward.rewardName.Contains("Master") && currentPoints >= 100)
                {
                    unlockedRewards.Add(reward);
                    Debug.Log($"🏆 Badge Unlocked: {reward.rewardName}");
                    
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (gm != null && gm.uiManager != null)
                        gm.uiManager.ShowFeedback($"🏆 Badge Unlocked: {reward.rewardName}");
                }
                else if (reward.rewardName.Contains("Guru") && currentPoints >= 500)
                {
                    unlockedRewards.Add(reward);
                    Debug.Log($"🏆 Badge Unlocked: {reward.rewardName}");
                    
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (gm != null && gm.uiManager != null)
                        gm.uiManager.ShowFeedback($"🏆 Badge Unlocked: {reward.rewardName}");
                }
            }
        }
        
        LoadRewardsUI();
    }
    
    public void OpenRewardsPanel()
    {
        if (rewardsPanel != null)
        {
            rewardsPanel.SetActive(true);
            LoadRewardsUI(); // Refresh
        }
    }
    
    public void CloseRewardsPanel()
    {
        if (rewardsPanel != null)
            rewardsPanel.SetActive(false);
    }
    
    public int GetCurrentPoints()
    {
        return currentPoints;
    }
    
    public List<Reward> GetUnlockedRewards()
    {
        return unlockedRewards;
    }
}

[System.Serializable]
public class Reward
{
    public string rewardName;
    public string description;
    public int cost;
    public RewardCategory category;
    public string icon;
}

public enum RewardCategory
{
    AvatarCustomization,
    GameBoost,
    Badge,
    Feature
}
