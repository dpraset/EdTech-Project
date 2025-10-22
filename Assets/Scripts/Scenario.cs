using UnityEngine;

public class Scenario
{
    public string description;
    public float moneyChange;
    public string feedback;

    public Scenario(string desc, float change, string fb)
    {
        description = desc;
        moneyChange = change;
        feedback = fb;
    }
}
