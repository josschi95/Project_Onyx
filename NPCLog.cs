using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLog : MonoBehaviour
{
    [TextArea(1, 20)] [SerializeField] private string activityLog;
    void Start()
    {
        activityLog = "";
    }

    public void AddEntry(string entry)
    {
        activityLog += entry + "\n";
    }
}
