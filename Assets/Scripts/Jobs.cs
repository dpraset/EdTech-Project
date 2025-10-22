using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PaySchedule
{
    Daily,
    Weekly,
    BiWeekly,
    Monthly
}

public class Job
{
    public string jobName;          // Name of the job
    public float payAmount;         // Amount paid each pay cycle
    public PaySchedule paySchedule; // Payment frequency
    public bool isPartTime;         // Job type flag
    public string description;      // Job description

    // Constructor
    public Job(string jobName, float payAmount, PaySchedule paySchedule, bool isPartTime, string description = "")
    {
        this.jobName = jobName;
        this.payAmount = payAmount;
        this.paySchedule = paySchedule;
        this.isPartTime = isPartTime;
        this.description = description;
    }
}
