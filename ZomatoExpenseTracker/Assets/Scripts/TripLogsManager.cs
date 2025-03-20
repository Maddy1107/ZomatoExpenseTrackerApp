using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class TripLogsManager : MonoBehaviour
{
    private static TripLogsManager instance;
    public GameObject tripItemPrefab; // Prefab for individual trip entries
    public GameObject monthHeaderPrefab; // Prefab for expandable month sections
    public Transform scrollViewContent; // Parent for instantiated objects

    private string tripFilePath;
    private Dictionary<string, List<TripData>> tripsByMonth = new Dictionary<string, List<TripData>>();
    
    public static void OpenTripLogs()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<TripLogsManager>(true);
            if (instance == null)
            {
                Debug.LogError("TripManager is missing in the scene!");
                return;
            }
        }
        instance.gameObject.SetActive(true);
    }

    private void Awake()
    {
        tripFilePath = Path.Combine(Application.persistentDataPath, "trips.json");
        LoadTrips();
    }

    private void LoadTrips()
    {
        if (!File.Exists(tripFilePath))
        {
            Debug.LogError("Trip file not found: " + tripFilePath);
            return;
        }

        string json = File.ReadAllText(tripFilePath);
        List<TripData> allTrips = JsonConvert.DeserializeObject<List<TripData>>(json) ?? new List<TripData>();

        tripsByMonth.Clear();
        foreach (TripData trip in allTrips)
        {
            string monthKey = trip.date.ToString("MMMM yyyy"); // Example: "March 2025"

            if (!tripsByMonth.ContainsKey(monthKey))
            {
                tripsByMonth[monthKey] = new List<TripData>();
            }

            tripsByMonth[monthKey].Add(trip);
        }

        PopulateTripLogs();
    }

    private void PopulateTripLogs()
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var monthEntry in tripsByMonth)
        {
            string monthName = monthEntry.Key;
            List<TripData> monthTrips = monthEntry.Value;

            GameObject monthHeaderObj = Instantiate(monthHeaderPrefab, scrollViewContent);
            TMP_Text monthHeaderText = monthHeaderObj.GetComponentInChildren<TMP_Text>();
            monthHeaderText.text = monthName;

            Transform tripListContainer = monthHeaderObj.transform.Find("TripListContainer");
            Button monthButton = monthHeaderObj.GetComponent<Button>();
            monthButton.onClick.AddListener(() => ToggleMonthSection(tripListContainer));

            tripListContainer.gameObject.SetActive(false); // Start collapsed

            // Instantiate trip entries under this month
            foreach (TripData trip in monthTrips)
            {
                GameObject tripItemObj = Instantiate(tripItemPrefab, tripListContainer);
                TripItem tripUI = tripItemObj.GetComponent<TripItem>();
                tripUI.SetTripData(trip);
            }
        }
    }

    private void ToggleMonthSection(Transform section)
    {
        section.gameObject.SetActive(!section.gameObject.activeSelf);
    }
}
