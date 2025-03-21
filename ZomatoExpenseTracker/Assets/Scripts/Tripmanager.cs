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
    private float mileage;
    private float fuelPrice;

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

        trips = JSONHelper.LoadFromJson<List<TripData>>("trips.json") ?? new List<TripData>();
    }

    private void Start()
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
                Debug.LogError("❌ TripManager is missing in the scene!");
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
            Debug.LogError("❌ Invalid odometer input!");
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
            Debug.LogError("❌ No active trip to end.");
            return;
        }

        LoadActiveTrip();

        if (!float.TryParse(endTripReading.text, out float endOdo) ||
            !float.TryParse(earningsInput.text, out float earnings))
        {
            Debug.LogError("❌ Invalid input in end trip fields.");
            return;
        }

        currentTrip.endTime = DateTime.Now.TimeOfDay;
        currentTrip.endOdometer = endOdo;
        currentTrip.distance = currentTrip.endOdometer - currentTrip.startOdometer;
        currentTrip.earnings = earnings;

        currentTrip.fuelExpense = (currentTrip.distance / mileage) * fuelPrice;
        currentTrip.profit = currentTrip.earnings - currentTrip.fuelExpense;

        trips.Add(currentTrip);
        JSONHelper.SaveToJson("trips.json", trips);

        if (File.Exists(activeTripFilePath)) File.Delete(activeTripFilePath);

        tripActive = false;
        ClosePopup();
    }

    private void UpdateFuelCost()
    {
        if (!float.TryParse(endTripReading.text, out float endOdo))
        {
            fuelCostText.text = "₹";
            return;
        }

        float distance = endOdo - currentTrip.startOdometer;
        float fuelExpense = (distance / mileage) * fuelPrice;
        fuelCostText.text = $"Fuel Cost: ₹{fuelExpense:F2}";
    }

    private void SaveActiveTrip()
    {
        JSONHelper.SaveToJson("active_trip.json", currentTrip);
        Debug.Log("✅ Active trip saved.");
    }

    private void LoadActiveTrip()
    {
        currentTrip = JSONHelper.LoadFromJson<TripData>("active_trip.json") ?? new TripData();
        Debug.Log("✅ Active trip loaded.");
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

