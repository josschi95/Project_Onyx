using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DateTimeManager : MonoBehaviour
{
    #region - Singleton -
    public static DateTimeManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DateTimeManager found");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnNewDay();
    public OnNewDay onNewDay;

    public bool timeIsPaused = false;
    public float timeRate = 1;
    //0.05f, 1 real-life minute = 3 hours in-game, making an in-game day 8 minutes //Note this was using Time.deltaTime, not fixedDeltaTime
    //0.01f, 1 real-life minute = 1 hour1 in-game, making an in-game day 24 minutes

    [Range(0, 24)] public float timeOfDay;
    public DayOfWeek day;
    public int dayOfMonth;
    public Month month;
    public int totalDayCount;
    public int year;

    private int weekLength;
    private int yearLength;
    private int daysInAMonth = 30;

    private void Start()
    {
        weekLength = System.Enum.GetValues(typeof(DayOfWeek)).Length;
        yearLength = System.Enum.GetValues(typeof(Month)).Length;
    }

    private void FixedUpdate()
    {
        if (timeIsPaused == true) return;

        timeOfDay += Time.fixedDeltaTime * timeRate;
        if (timeOfDay >= 24) NewDay();
    }

    private void NewDay()
    {
        timeOfDay = 0;
        day++;
        dayOfMonth++;
        totalDayCount++;
        if (onNewDay != null) onNewDay.Invoke();

        if ((int)day == weekLength) NewWeek();
        //Debug.Log("New Day");
    }

    private void NewWeek()
    {
        day = 0;
        dayOfMonth++;
        if (dayOfMonth >= daysInAMonth) NewMonth();
        //Debug.Log("New Week");
    }

    private void NewMonth()
    {
        month++;
        dayOfMonth = 0;
        if ((int)month >= yearLength) NewYear();
        //Debug.Log("New Month");
    }

    private void NewYear()
    {
        month = 0;
        year++;
        //Debug.Log("New Year");
    }

    public void SkipToNextDay()
    {
        NewDay();
        timeOfDay = 6;
    }

    public string GetTimeOfDay()
    {
        if (timeOfDay >= 6 && timeOfDay < 12) return "morning";
        else if (timeOfDay >= 12 && timeOfDay < 17) return "afternoon";
        else return "evening";
    }

    public void SetSavedDateTime(float timeOfDay, int dayOfWeek, int dayOfMonth, int currentMonth, int year, int totalDayCount)
    {
        this.timeOfDay = timeOfDay;
        day = (DayOfWeek)dayOfWeek;
        this.dayOfMonth = dayOfMonth;
        month = (Month)currentMonth;
        this.year = year;
        this.totalDayCount = totalDayCount;
    }
}
public enum DayOfWeek { Day00, Day01, Day02, Day03, Day04, Day05, Day06, Day07, Day08, Day09 }
public enum Month { Month00, Month01, Month02, Month03, Month04, Month05, Month06, Month07, Month08, Month09 }
