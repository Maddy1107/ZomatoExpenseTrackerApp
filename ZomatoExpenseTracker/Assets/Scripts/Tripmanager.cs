using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;

public class TripManager : MonoBehaviour
{
    public GameObject startTrip, endTrip;
    private bool tripActive = false;
    public Button startConfirmbtn, endConfirmbtn;

    public TMP_InputField startTripReading, endTripReading, earningsInput;

    private List<TripData> trips = new List<TripData>();

    private TripData currentTrip;

    public static TripManager instance;

    private string filePath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        filePath = Path.Combine(Application.persistentDataPath, "trips.json");

        //LoadTrips();
    }

    public void OnEnable()
    {
        currentTrip = new TripData();

        if (!tripActive)
        {
            startTrip.SetActive(true);
            endTrip.SetActive(false);
        }
        else
        {
            startTrip.SetActive(false);
            endTrip.SetActive(true);
        }

        startConfirmbtn.onClick.AddListener(StartTrip);
        endConfirmbtn.onClick.AddListener(EndTrip);
    }

    public void ShowPopup(bool tripActive)
    {
        this.tripActive = tripActive;
        this.gameObject.SetActive(true);
    }

    public void StartTrip()
    {
        currentTrip.date = DateTime.Now;
        currentTrip.startOdometer = float.Parse(startTripReading.text);
        currentTrip.startTime = DateTime.Now.TimeOfDay;

        tripActive = true;
        SaveTrips();

        this.gameObject.SetActive(false);
    }

    public void EndTrip()
    {
        currentTrip.endTime = DateTime.Now.TimeOfDay;
        currentTrip.endOdometer = float.Parse(endTripReading.text);
        currentTrip.distance = currentTrip.endOdometer - currentTrip.startOdometer;
        currentTrip.earnings = float.Parse(earningsInput.text);
        currentTrip.fuelExpense = currentTrip.distance / PlayerPrefs.GetFloat("Mileage", 40f) * PlayerPrefs.GetFloat("FuelPrice", 100f);
        currentTrip.profit = currentTrip.earnings - currentTrip.fuelExpense;

        trips.Add(currentTrip);
        tripActive = false;

        SaveTrips();
        this.gameObject.SetActive(false);
    }

    private void SaveTrips()
    {
        string json = JsonConvert.SerializeObject(trips, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Debug.Log("Trips saved to " + filePath);
    }

    private void LoadTrips()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            trips = JsonConvert.DeserializeObject<List<TripData>>(json) ?? new List<TripData>();
            Debug.Log("Trips loaded from " + filePath);
        }
        else
        {
            trips = new List<TripData>();
        }
    }
}

[System.Serializable]
public class TripData
{
    public DateTime date;
    public TimeSpan startTime;
    public TimeSpan endTime;
    public float startOdometer;
    public float endOdometer;
    public string duration;
    public float distance;
    public float fuelPrice;
    public float fuelExpense;
    public float earnings;
    public float profit;
}
