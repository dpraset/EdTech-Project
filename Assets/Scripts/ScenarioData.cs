using UnityEngine;

public class ScenarioData
{
    public string scenarioText;

    // Scale difficulty by progression
    public ScenarioDifficulty difficulty;

    public ScenarioChoice[] choices;

    public enum ScenarioDifficulty
    {
        // Month 1 - 2
        Easy,
        // Month 3 - 4
        Medium,
        // Month 5+
        Hard       
    }
    
}

public class ScenarioChoice
{
    public string choiceText;
    public float moneyChange;
    public string outcomeText;
}
