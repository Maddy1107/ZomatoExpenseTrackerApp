using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public enum FilterType { Daily, Weekly, Monthly }

public class TripLogsManager : MonoBehaviour
{
    public static TripLogsManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject tripItemPrefab;
    [SerializeField] private Transform tripContainer;
    [SerializeField] private GameObject noTripsPanel;

    [Header("Filter UI")]
    [SerializeField] private TMP_Text selectedDateText, headerText;
    [SerializeField] private Button dailyButton, weeklyButton, monthlyButton, selectDateButton;

    [Header("UI Colors")]
    [SerializeField] private Color selectedColor, defaultColor;

    private List<TripData> allTrips = new List<TripData>();
    private DateTime selectedDate = DateTime.Now;
    private FilterType filterType = FilterType.Daily;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        CalenderScript.OnDateSelected += LoadTripsForDateRange;
        LoadAllTrips();
    }

    private void OnDisable()
    {
        CalenderScript.OnDateSelected -= LoadTripsForDateRange;
    }

    private void Start()
    {
        dailyButton.onClick.AddListener(() => SetFilter(FilterType.Daily));
        weeklyButton.onClick.AddListener(() => SetFilter(FilterType.Weekly));
        monthlyButton.onClick.AddListener(() => SetFilter(FilterType.Monthly));
        selectDateButton.onClick.AddListener(OpenSelectionPanel);

        SetFilter(FilterType.Daily);
    }

    private void LoadAllTrips()
    {
        UserData userData = JSONHelper.LoadFromJson<UserData>("userdata.json") ?? new UserData();
        allTrips = userData.trips;  // Load trips from `UserData`
    }

    private void OpenSelectionPanel()
    {
        switch (filterType)
        {
            case FilterType.Daily:
                CalenderScript.Instance.GenerateDailyCalendar(selectedDate.Year, selectedDate.Month);
                headerText.text = selectedDate.ToString("MMMM yyyy");
                break;
            case FilterType.Weekly:
                CalenderScript.Instance.GenerateWeeklyRanges(selectedDate.Year);
                headerText.text = "Select a week";
                break;
            case FilterType.Monthly:
                CalenderScript.Instance.GenerateMonthlySelection();
                headerText.text = "Select a month";
                break;
        }
    }

    private void LoadTripsForDateRange(DateTime startDate, DateTime endDate)
    {
        selectedDateText.text = (startDate == endDate) ? startDate.ToString("dd MMMM yyyy") :
            (startDate.Day == 1 && endDate.Day == DateTime.DaysInMonth(startDate.Year, startDate.Month)) ?
            startDate.ToString("MMMM yyyy") : $"{startDate:dd MMM} - {endDate:dd MMM yyyy}";

        foreach (Transform child in tripContainer)
            Destroy(child.gameObject);

        List<TripData> filteredTrips = allTrips
            .Where(trip => trip.date.Date >= startDate.Date && trip.date.Date <= endDate.Date)
            .ToList();

        noTripsPanel.SetActive(filteredTrips.Count == 0);
        tripContainer.gameObject.SetActive(filteredTrips.Count > 0);

        if (filteredTrips.Count > 0)
        {
            GameObject tripItemObj = Instantiate(tripItemPrefab, tripContainer);
            tripItemObj.GetComponent<TripItem>().SetTripData(filteredTrips, filterType);
        }
    }

    public void SetFilter(FilterType filterType)
    {
        this.filterType = filterType;
        DateTime today = DateTime.Now;

        switch (filterType)
        {
            case FilterType.Daily:
                LoadTripsForDateRange(today, today);
                break;
            case FilterType.Weekly:
                DateTime weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                LoadTripsForDateRange(weekStart, weekStart.AddDays(6));
                break;
            case FilterType.Monthly:
                DateTime monthStart = new DateTime(today.Year, today.Month, 1);
                LoadTripsForDateRange(monthStart, monthStart.AddMonths(1).AddDays(-1));
                break;
        }

        UpdateButtonUI();
    }

    private void UpdateButtonUI()
    {
        dailyButton.image.color = (filterType == FilterType.Daily) ? selectedColor : defaultColor;
        weeklyButton.image.color = (filterType == FilterType.Weekly) ? selectedColor : defaultColor;
        monthlyButton.image.color = (filterType == FilterType.Monthly) ? selectedColor : defaultColor;
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
