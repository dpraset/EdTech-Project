using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject educationPanel;

    public Button gameTabButton;
    public Button educationTabButton;

    private MenuState currentState = MenuState.Game;

    // Blue for active
    public Color activeTabColor = new Color();
    // Gray for inactive
    public Color inactiveTabColor = new Color();

    public enum MenuState
    {
        Game,
        Education
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTabButton.onClick.AddListener(() => SwitchToMenu(MenuState.Game));
        educationTabButton.onClick.AddListener(() => SwitchToMenu(MenuState.Education));

        // Start on game menu - make home menu later
        SwitchToMenu(MenuState.Game);
    }

    public void SwitchToMenu(MenuState newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case MenuState.Game:
                ShowGamePanel();
                break;
            case MenuState.Education:
                ShowEducationPanel();
                break;
        }
        
        UpdateButtonVisuals();
    }
    
    // Game panel
    void ShowGamePanel()
    {
        gamePanel.SetActive(true);
        educationPanel.SetActive(false);
        Debug.Log("Game Menu");
    }
    
    // Education panel
    void ShowEducationPanel()
    {
        gamePanel.SetActive(false);
        educationPanel.SetActive(true);
        Debug.Log("Education Menu");
    }
    
    void UpdateButtonVisuals()
    {
        // Update button colors to show active tab
        Image gameButtonImage = gameTabButton.GetComponent<Image>();
        Image educationButtonImage = educationTabButton.GetComponent<Image>();
        
        if (currentState == MenuState.Game)
        {
            gameButtonImage.color = activeTabColor;
            educationButtonImage.color = inactiveTabColor;
        }
        else
        {
            gameButtonImage.color = inactiveTabColor;
            educationButtonImage.color = activeTabColor;
        }
    }
    
    public MenuState GetCurrentState()
    {
        return currentState;
    }
}
