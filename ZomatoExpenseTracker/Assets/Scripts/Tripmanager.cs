using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;

public class TripManager : MonoBehaviour
{
    public static event Action OnTripStatusChanged;

    [Header("UI Elements")]
    [SerializeField] private GameObject startTripPopup, endTripPopup;
    [SerializeField] private Button startConfirmButton, endConfirmButton;
    [SerializeField] private TMP_InputField startTripReading, endTripReading, earningsInput;
    [SerializeField] private TMP_Text fuelCostText;

    private bool tripActive;
    private TripData currentTrip;
    float mileage;
    float fuelPrice;

    private List<TripData> trips = new List<TripData>();

    private static TripManager instance;
    private string tripFilePath;
    private string activeTripFilePath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        tripFilePath = Path.Combine(Application.persistentDataPath, "trips.json");
        activeTripFilePath = Path.Combine(Application.persistentDataPath, "active_trip.json");

        LoadTrips();
    }

    void Start()
    {
        endTripReading.onValueChanged.RemoveAllListeners();
        endTripReading.onValueChanged.AddListener(delegate { UpdateFuelCost(); });
    }

    public static void OpenTripPopup()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<TripManager>(true);
            if (instance == null)
            {
                Debug.LogError("TripManager is missing in the scene!");
                return;
            }
        }
        instance.ShowPopup();
    }

    private void ShowPopup()
    {
        tripActive = PlayerPrefs.GetInt("TripActive", 0) == 1;

        startTripPopup.SetActive(!tripActive);
        endTripPopup.SetActive(tripActive);
        gameObject.SetActive(true);

        if (tripActive) LoadActiveTrip();

        mileage = PlayerPrefs.GetFloat("Mileage", 40f);
        fuelPrice = PlayerPrefs.GetFloat("FuelPrice", 103.93f);

        startConfirmButton.onClick.RemoveAllListeners();
        startConfirmButton.onClick.AddListener(StartTrip);

        endConfirmButton.onClick.RemoveAllListeners();
        endConfirmButton.onClick.AddListener(EndTrip);
    }

    private void StartTrip()
    {
        if (!float.TryParse(startTripReading.text, out float startOdo))
        {
            Debug.LogError("Invalid odometer input!");
            return;
        }

        currentTrip = new TripData
        {
            date = DateTime.Now,
            startOdometer = startOdo,
            startTime = DateTime.Now.TimeOfDay
        };

        tripActive = true;
        SaveActiveTrip();
        ClosePopup();
    }

    private void EndTrip()
    {
        if (!tripActive || !File.Exists(activeTripFilePath))
        {
            Debug.LogError("No active trip to end.");
            return;
        }

        LoadActiveTrip();

        if (!float.TryParse(endTripReading.text, out float endOdo) ||
            !float.TryParse(earningsInput.text, out float earnings))
        {
            Debug.LogError("Invalid input in end trip fields.");
            return;
        }

        currentTrip.endTime = DateTime.Now.TimeOfDay;
        currentTrip.endOdometer = endOdo;
        currentTrip.distance = currentTrip.endOdometer - currentTrip.startOdometer;
        currentTrip.earnings = earnings;

        currentTrip.fuelExpense = (currentTrip.distance / mileage) * fuelPrice;
        currentTrip.profit = currentTrip.earnings - currentTrip.fuelExpense;

        trips.Add(currentTrip);
        SaveTrips();

        if (File.Exists(activeTripFilePath)) File.Delete(activeTripFilePath);

        tripActive = false;
        ClosePopup();
    }

    private void UpdateFuelCost()
    {
        float startOdo =currentTrip.startOdometer;
        if (!float.TryParse(endTripReading.text, out float endOdo))
        {
            fuelCostText.text = "₹";
            return;
        }

        float distance = endOdo - startOdo;
        float fuelExpense = (distance / mileage) * fuelPrice;

        fuelCostText.text = $"Fuel Cost: ₹{fuelExpense:F2}";
    }

    private void SaveTrips()
    {
        try
        {
            string json = JsonConvert.SerializeObject(trips, Formatting.Indented);
            File.WriteAllText(tripFilePath, json);
            Debug.Log("Trips saved successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving trips: " + ex.Message);
        }
    }

    private void LoadTrips()
    {
        if (!File.Exists(tripFilePath)) return;

        try
        {
            string json = File.ReadAllText(tripFilePath);
            trips = JsonConvert.DeserializeObject<List<TripData>>(json) ?? new List<TripData>();
            Debug.Log("Trips loaded successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading trips: " + ex.Message);
            trips = new List<TripData>();
        }
    }

    private void SaveActiveTrip()
    {
        try
        {
            string json = JsonConvert.SerializeObject(currentTrip, Formatting.Indented);
            File.WriteAllText(activeTripFilePath, json);
            Debug.Log("Active trip saved.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving active trip: " + ex.Message);
        }
    }

    private void LoadActiveTrip()
    {
        if (!File.Exists(activeTripFilePath)) return;

        try
        {
            string json = File.ReadAllText(activeTripFilePath);
            currentTrip = JsonConvert.DeserializeObject<TripData>(json);
            Debug.Log("Active trip loaded.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading active trip: " + ex.Message);
            currentTrip = new TripData();
        }
    }

    private void ClosePopup()
    {
        startTripReading.text = "";
        endTripReading.text = "";
        earningsInput.text = "";
        fuelCostText.text = "₹";

        PlayerPrefs.SetInt("TripActive", tripActive ? 1 : 0);

        OnTripStatusChanged?.Invoke();

        gameObject.SetActive(false);
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
    public float distance;
    public float fuelExpense;
    public float earnings;
    public float profit;
    public string Duration
    {
        get
        {
            TimeSpan duration = endTime - startTime;
            return $"{(int)duration.TotalHours:D2}h:{duration.Minutes:D2}m";
        }
    }
}
