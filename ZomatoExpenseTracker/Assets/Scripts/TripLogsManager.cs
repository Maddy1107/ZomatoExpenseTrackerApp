using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using System.Linq;

public class TripLogsManager : MonoBehaviour
{
    public static TripLogsManager instance;

    public GameObject tripItemPrefab;       // Prefab for individual trip entries
    public GameObject tripContainer;        // Parent container for trip logs
    public GameObject noTripsFoundPanel;    // UI to show when no trips are found
    public Transform scrollViewContent;     // Parent for instantiated objects
    public TMP_Text selectedDateText;       // UI element to display the selected date

    private string tripFilePath;
    private List<TripData> allTrips = new List<TripData>();
    
    private DateTime selectedStartDate;
    private DateTime selectedEndDate;

    public static void OpenTripLogs()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<TripLogsManager>(true);
            if (instance == null)
            {
                Debug.LogError("TripLogsManager is missing in the scene!");
                return;
            }
        }
        instance.gameObject.SetActive(true);
    }

    void OnEnable()
    {
        CalendarDropDown.OnDateSelected += LoadTripsForDateRange;
    }

    void OnDisable()
    {
        CalendarDropDown.OnDateSelected -= LoadTripsForDateRange;
    }

    private void Awake()
    {
        tripFilePath = Path.Combine(Application.persistentDataPath, "trips.json");
        LoadAllTrips();
    }

    private void LoadAllTrips()
    {
        if (!File.Exists(tripFilePath))
        {
            Debug.LogError("Trip file not found: " + tripFilePath);
            return;
        }

        string json = File.ReadAllText(tripFilePath);
        allTrips = JsonConvert.DeserializeObject<List<TripData>>(json) ?? new List<TripData>();
    }

    public void LoadTripsForDateRange(DateTime startDate, DateTime endDate)
    {
        selectedStartDate = startDate;
        selectedEndDate = endDate;

        selectedDateText.text = startDate == endDate
            ? startDate.ToString("dd MMMM yyyy") // Daily
            : $"{startDate:dd MMM} - {endDate:dd MMM yyyy}"; // Weekly or Monthly

        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Filter trips in selected range
        List<TripData> filteredTrips = allTrips.Where(trip =>
            trip.date.Date >= selectedStartDate.Date &&
            trip.date.Date <= selectedEndDate.Date).ToList();

        if (filteredTrips.Count == 0)
        {
            // No trips found
            tripContainer.SetActive(false);
            noTripsFoundPanel.SetActive(true);
        }
        else
        {
            // Populate trips
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
}
