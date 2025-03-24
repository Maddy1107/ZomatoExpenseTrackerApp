using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class TripLogsManager : MonoBehaviour
{
    public static TripLogsManager instance;

    [Header("UI Elements")]
    public GameObject tripItemPrefab;       // Prefab for individual trip entries
    public GameObject tripContainer;        // Parent container for trip logs
    public GameObject noTripsFoundPanel;    // UI to show when no trips are found
    public Transform scrollViewContent;     // Parent for instantiated objects

    private List<TripData> allTrips = new List<TripData>();
    private DateTime selectedStartDate;
    private DateTime selectedEndDate;
    
    public TMP_Text selectedDateText, headerText;
    public Button dailyButton, weeklyButton, monthlyButton, selectDateButton;

    public Color selectedColor, defaultColor;

    private DateTime selectedDate = DateTime.Now;
    private Filtertype filterType = Filtertype.Daily;

    void OnEnable()
    {
        CalenderScript.OnDateSelected += LoadTripsForDateRange;
    }

    void OnDisable()
    {
        CalenderScript.OnDateSelected -= LoadTripsForDateRange;
    }   
    
    public static void OpenTripLogs()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<TripLogsManager>(true);
            if (instance == null)
            {
                Debug.LogError("❌ TripLogsManager is missing in the scene!");
                return;
            }
        }
        instance.gameObject.SetActive(true);
    }

    private void Start()
    {
        dailyButton.onClick.AddListener(() => SetFilter(Filtertype.Daily));
        weeklyButton.onClick.AddListener(() => SetFilter(Filtertype.Weekly));
        monthlyButton.onClick.AddListener(() => SetFilter(Filtertype.Monthly));
        selectDateButton.onClick.AddListener(OpenSelectionPanel);

        SetDefaultSelection();
    }

    private void SetDefaultSelection()
    {
        SetFilter(Filtertype.Daily);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LoadAllTrips();
    }

    public void LoadAllTrips()
    {
        allTrips = JSONHelper.LoadFromJson<List<TripData>>("trips.json") ?? new List<TripData>();
    }

    private void OpenSelectionPanel()
    {
        switch (filterType)
        {
            case Filtertype.Daily:
                CalenderScript.instance.GenerateDailyCalendar(selectedDate.Year, selectedDate.Month);
                headerText.text = selectedDate.ToString("MMMM yyyy");
                break;
            case Filtertype.Weekly:
                CalenderScript.instance.GenerateWeeklyRanges(selectedDate.Year);
                headerText.text = "Select a week";
                break;
            case Filtertype.Monthly:
                CalenderScript.instance.GenerateMonthlySelection();
                headerText.text = "Select a month";
                break;
        }
    }

    private void LoadTripsForDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == endDate)
        {
            selectedDateText.text = startDate.ToString("dd MMMM yyyy");
        }
        else
        {
            selectedDateText.text = $"{startDate:dd MMM} - {endDate:dd MMM yyyy}"; 
        }

        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        List<TripData> filteredTrips = allTrips.Where(trip =>
            trip.date.Date >= startDate.Date &&
            trip.date.Date <= endDate.Date).ToList();

        if (filteredTrips.Count == 0)
        {
            tripContainer.SetActive(false);
            noTripsFoundPanel.SetActive(true);
        }
        else
        {
            tripContainer.SetActive(true);
            noTripsFoundPanel.SetActive(false);

            foreach (TripData trip in filteredTrips)
            {
                GameObject tripItemObj = Instantiate(tripItemPrefab, scrollViewContent);
                TripItem tripUI = tripItemObj.GetComponent<TripItem>();
                tripUI.SetTripData(trip);
            }
        }
    }

    public void SetFilter(Filtertype filtertype)
    {
        filterType = filtertype;
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

        UpdateButtonUI();
    }

    private void UpdateButtonUI()
    {
        dailyButton.image.color = (filterType == Filtertype.Daily) ? selectedColor : defaultColor;
        weeklyButton.image.color = (filterType == Filtertype.Weekly) ? selectedColor : defaultColor;
        monthlyButton.image.color = (filterType == Filtertype.Monthly) ? selectedColor : defaultColor;
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
