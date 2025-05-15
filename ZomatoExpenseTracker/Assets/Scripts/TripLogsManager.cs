using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class TripLogsManager : MonoBehaviour
{
    public static TripLogsManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject tripItemPrefab;
    [SerializeField] private Transform tripContainer;
    [SerializeField] private GameObject noTripsPanel;

    [Header("Filter UI")]
    [SerializeField] private TMP_Text selectedDateText;
    [SerializeField] private Button selectDateButton;
    [SerializeField] private TMP_Dropdown monthDropdown;
    private List<TripData> allTrips = new List<TripData>();
    private DateTime selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        PopulateDropDown();
        LoadAllTrips();
    }

    private void PopulateDropDown()
    {
        monthDropdown.ClearOptions();
        var months = System.Globalization.DateTimeFormatInfo.InvariantInfo.MonthNames
            .Where(m => !string.IsNullOrEmpty(m))
            .ToList();
        months.Insert(0, "Show All");
        monthDropdown.AddOptions(months);
        int currentMonthIndex = DateTime.Now.Month;
        monthDropdown.value = currentMonthIndex;
        monthDropdown.RefreshShownValue();
    }

    private void Start()
    {
        monthDropdown.onValueChanged.AddListener(OnMonthDropdownValueChanged);
    }

    private void OnMonthDropdownValueChanged(int selectedIndex)
    {
        if (selectedIndex == 0)
        {
            selectedDateText.text = "All Trips";
            GenerateTripTable(true);
            return;
        }
        selectedMonth = new DateTime(DateTime.Now.Year, selectedIndex, 1);
        selectedDateText.text = selectedMonth.ToString("MMMM yyyy");
        GenerateTripTable();
    }

    private void LoadAllTrips()
    {
        UserData userData = JSONHelper.LoadFromJson<UserData>("userdata.json") ?? new UserData();
        allTrips = userData.trips; 
        GenerateTripTable();
    }

    private void GenerateTripTable(bool showAllTrips = false)
    {
        foreach (Transform child in tripContainer)
        {
            Destroy(child.gameObject);
        }

        var filteredTrips = 
        showAllTrips ? allTrips : allTrips.Where(trip => trip.date.Month == selectedMonth.Month && trip.date.Year == selectedMonth.Year).ToList();

        if (filteredTrips.Count == 0)
        {
            noTripsPanel.SetActive(true);
            return;
        }

        noTripsPanel.SetActive(false);

        foreach (var trip in filteredTrips)
        {
            GameObject tripItem = Instantiate(tripItemPrefab, tripContainer);
            var tripItemComponents = tripItem.GetComponent<TripItem>();
            if (tripItemComponents != null)
            {
                tripItemComponents.SetTripData(trip);
            }
        }
    }
    
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
