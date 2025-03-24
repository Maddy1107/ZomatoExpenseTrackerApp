using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalenderScript : MonoBehaviour
{
    public static CalenderScript instance;

    public GameObject calendarPanel;
    public Transform dateGrid, monthWeekGrid; 
    public Button dateButtonPrefab;    // Prefab for calendar date buttons
    public Button monthweekPrefab;

    public static Action<DateTime, DateTime> OnDateSelected;
    public static Action<DateTime> OnDateSelectedSingle;


    void Start()
    {
        calendarPanel.SetActive(false);
        dateGrid.gameObject.SetActive(false);
        monthWeekGrid.gameObject.SetActive(false);
    }

    public void GenerateDailyCalendar(int year, int month)
    {
        calendarPanel.SetActive(true);
        dateGrid.gameObject.SetActive(true);

        int daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            Button dateButton = Instantiate(dateButtonPrefab, dateGrid);
            dateButton.GetComponentInChildren<TMP_Text>().text = day.ToString();
            int selectedDay = day;

            dateButton.onClick.AddListener(() =>
            {
                DateTime selected = new DateTime(year, month, selectedDay);
                SelectDateRange(selected, selected); // Single day selection
            });
        }
    }

    public void GenerateWeeklyRanges(int year)
    {
        calendarPanel.SetActive(true);
        monthWeekGrid.gameObject.SetActive(true);

        DateTime firstMonday = FirstMondayOfYear(year);

        for (int i = 0; i < 52; i++) // Generate 52 weeks
        {
            DateTime weekStart = firstMonday.AddDays(i * 7);
            DateTime weekEnd = weekStart.AddDays(6);

            Button weekButton = Instantiate(monthweekPrefab, monthWeekGrid);
            weekButton.GetComponentInChildren<TMP_Text>().text = $"{weekStart:dd MMM} - {weekEnd:dd MMM}";

            DateTime selectedStart = weekStart;
            DateTime selectedEnd = weekEnd;

            weekButton.onClick.AddListener(() =>
            {
                SelectDateRange(selectedStart, selectedEnd);
            });
        }
    }

    public void GenerateMonthlySelection()
    {
        calendarPanel.SetActive(true);
        monthWeekGrid.gameObject.SetActive(true);
        
        for (int month = 1; month <= 12; month++)
        {
            Button monthButton = Instantiate(monthweekPrefab, monthWeekGrid);
            monthButton.GetComponentInChildren<TMP_Text>().text = new DateTime(2025, month, 1).ToString("MMMM");

            int selectedMonth = month;

            monthButton.onClick.AddListener(() =>
            {
                DateTime start = new DateTime(DateTime.Now.Year, selectedMonth, 1);
                DateTime end = start.AddMonths(1).AddDays(-1); // Last day of the month
                SelectDateRange(start, end);
            });
        }
    }

    private void SelectDateRange(DateTime startDate, DateTime endDate)
    {
        OnDateSelected?.Invoke(startDate, endDate);
        calendarPanel.SetActive(false);

        ClearGrid();
    }

    private void ClearGrid()
    {
        foreach (Transform child in dateGrid)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in monthWeekGrid)
        {
            Destroy(child.gameObject);
        }

        dateGrid.gameObject.SetActive(false);
        monthWeekGrid.gameObject.SetActive(false);
    }

    private DateTime FirstMondayOfYear(int year)
    {
        DateTime firstDay = new DateTime(year, 1, 1);
        while (firstDay.DayOfWeek != DayOfWeek.Monday)
        {
            firstDay = firstDay.AddDays(1);
        }
        return firstDay;
    }
}
