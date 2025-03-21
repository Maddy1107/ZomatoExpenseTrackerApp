using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

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

    private void OnEnable()
    {
        CalendarDropDown.OnDateSelected += LoadTripsForDateRange;
    }

    private void OnDisable()
    {
        CalendarDropDown.OnDateSelected -= LoadTripsForDateRange;
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

    private void LoadTripsForDateRange(DateTime startDate, DateTime endDate)
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Filter trips based on date range
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

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
