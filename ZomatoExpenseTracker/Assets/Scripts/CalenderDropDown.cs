using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public enum Filtertype
{
    Daily,
    Weekly,
    Monthly
}

public class CalendarDropDown : MonoBehaviour
{
    public GameObject calendarPanel;   // The calendar UI panel
    public Transform dateGrid, weekGrid, monthGrid; // Grids to hold date buttons   
    public Button dateButtonPrefab;    // Prefab for calendar date buttons
    public Button monthweekPrefab;
    public TMP_Text selectedDateText;  // Displays selected date

    public Button dailyButton, weeklyButton, monthlyButton, selectDateButton; // UI buttons

    public static Action<DateTime, DateTime> OnDateSelected;

    private DateTime selectedDate = DateTime.Now;
    private Filtertype filterType = Filtertype.Daily;

    private void Start()
    {
        calendarPanel.SetActive(false);

        dailyButton.onClick.AddListener(() => SetFilter(Filtertype.Daily));
        weeklyButton.onClick.AddListener(() => SetFilter(Filtertype.Weekly));
        monthlyButton.onClick.AddListener(() => SetFilter(Filtertype.Monthly));
        selectDateButton.onClick.AddListener(OpenSelectionPanel);

        SetDefaultSelection();
    }

    private void SetDefaultSelection()
    {
        selectedDate = DateTime.Now;
        filterType = Filtertype.Daily;
        OnDateSelected?.Invoke(selectedDate, selectedDate);
    }

    private void SetFilter(Filtertype type)
    {
        filterType = type;
        DateTime today = DateTime.Now;

        switch (filterType)
        {
            case Filtertype.Daily:
                selectedDate = today;
                selectedDateText.text = selectedDate.ToString("dd MMMM yyyy");
                break;

            case Filtertype.Weekly:
                DateTime weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                DateTime weekEnd = weekStart.AddDays(6);
                selectedDateText.text = $"{weekStart:dd MMM} - {weekEnd:dd MMM yyyy}";
                break;

            case Filtertype.Monthly:
                selectedDate = new DateTime(today.Year, today.Month, 1);
                selectedDateText.text = selectedDate.ToString("MMMM yyyy");
                break;
        }
    }


    private void OpenSelectionPanel()
    {
        calendarPanel.SetActive(true);

        switch (filterType)
        {
            case Filtertype.Daily:
                GenerateDailyCalendar(selectedDate.Year, selectedDate.Month);
                break;
            case Filtertype.Weekly:
                GenerateWeeklyRanges(selectedDate.Year);
                break;
            case Filtertype.Monthly:
                GenerateMonthlySelection();
                break;
        }
    }

    private void GenerateDailyCalendar(int year, int month)
    {
        ClearGrid();
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

    private void GenerateWeeklyRanges(int year)
    {
        ClearGrid();
        DateTime firstMonday = FirstMondayOfYear(year);

        for (int i = 0; i < 52; i++) // Generate 52 weeks
        {
            DateTime weekStart = firstMonday.AddDays(i * 7);
            DateTime weekEnd = weekStart.AddDays(6);

            Button weekButton = Instantiate(monthweekPrefab, weekGrid);
            weekButton.GetComponentInChildren<TMP_Text>().text = $"{weekStart:dd MMM} - {weekEnd:dd MMM}";

            DateTime selectedStart = weekStart;
            DateTime selectedEnd = weekEnd;

            weekButton.onClick.AddListener(() =>
            {
                SelectDateRange(selectedStart, selectedEnd);
            });
        }
    }

    private void GenerateMonthlySelection()
    {
        ClearGrid();

        for (int month = 1; month <= 12; month++)
        {
            Button monthButton = Instantiate(monthweekPrefab, monthGrid);
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
        selectedDateText.text = startDate == endDate
            ? startDate.ToString("dd MMMM yyyy")  // Daily
            : $"{startDate:dd MMM} - {endDate:dd MMM yyyy}";  // Weekly or Monthly

        OnDateSelected?.Invoke(startDate, endDate);
        calendarPanel.SetActive(false);
    }

    private void ClearGrid()
    {
        Transform grid = null;
        switch (filterType)
        {
            case Filtertype.Daily:
                grid = dateGrid;
                break;
            case Filtertype.Weekly:
                grid = weekGrid;
                break;
            case Filtertype.Monthly:
                grid = monthGrid;
                break;
        }

        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
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
