using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalenderScript : MonoBehaviour
{
    public static CalenderScript Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject calendarPanel;
    [SerializeField] private Transform dateGrid, monthWeekGrid;
    [SerializeField] private Button dateButtonPrefab, monthWeekButtonPrefab;

    public static event Action<DateTime, DateTime> OnDateSelected;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        calendarPanel.SetActive(false);
        dateGrid.gameObject.SetActive(false);
        monthWeekGrid.gameObject.SetActive(false);
    }

    public void GenerateDailyCalendar(int year, int month)
    {
        OpenCalendar(dateGrid);

        int daysInMonth = DateTime.DaysInMonth(year, month);
        
        for (int day = 1; day <= daysInMonth; day++) // ✅ Fix: Include the last day
        {
            int selectedDay = day; // ✅ Capture correct value for each button

            CreateButton(dateButtonPrefab, dateGrid, selectedDay.ToString(), () =>
            {
                DateTime selectedDate = new DateTime(year, month, selectedDay);
                SelectDateRange(selectedDate, selectedDate);
            });
        }
    }

    public void GenerateWeeklyRanges(int year)
    {
        OpenCalendar(monthWeekGrid);

        DateTime today = DateTime.Today;
        DateTime lastMonday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        for (int weekOffset = 0; weekOffset < 52; weekOffset++)
        {
            DateTime weekStart = lastMonday.AddDays(-weekOffset * 7);
            DateTime weekEnd = weekStart.AddDays(6);

            if (weekStart.Year != year || weekStart > today) break;

            CreateButton(monthWeekButtonPrefab, monthWeekGrid, $"{weekStart:dd MMM} - {weekEnd:dd MMM}", () =>
            {
                SelectDateRange(weekStart, weekEnd);
            });
        }
    }

    public void GenerateMonthlySelection()
    {
        OpenCalendar(monthWeekGrid);

        DateTime today = DateTime.Today;
        int currentYear = today.Year;

        for (int month = today.Month; month >= 1; month--)
        {
            DateTime startOfMonth = new DateTime(currentYear, month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            CreateButton(monthWeekButtonPrefab, monthWeekGrid, startOfMonth.ToString("MMMM"), () =>
            {
                SelectDateRange(startOfMonth, endOfMonth);
            });
        }
    }

    private void SelectDateRange(DateTime startDate, DateTime endDate)
    {
        OnDateSelected?.Invoke(startDate, endDate);
        CloseCalendar();
    }

    private void OpenCalendar(Transform grid)
    {
        calendarPanel.SetActive(true);
        grid.gameObject.SetActive(true);
    }

    private void CloseCalendar()
    {
        calendarPanel.SetActive(false);
        ClearGrid(dateGrid);
        ClearGrid(monthWeekGrid);
    }

    private void CreateButton(Button prefab, Transform parent, string text, Action onClickAction)
    {
        Button button = Instantiate(prefab, parent);
        button.GetComponentInChildren<TMP_Text>().text = text;
        button.onClick.AddListener(() =>
        {
            onClickAction.Invoke();
            button.onClick.RemoveAllListeners();
        });
    }

    private void ClearGrid(Transform grid)
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
        grid.gameObject.SetActive(false);
    }
}
