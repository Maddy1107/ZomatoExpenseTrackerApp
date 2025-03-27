using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

public class TripManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button confirmButton, dateSelectorButton, closeButton;
    [SerializeField] private TMP_InputField startTripReading, endTripReading, earningsInput;
    [SerializeField] private TMP_Text fuelCostText, dateText, distanceText;

    [SerializeField] private TMP_Text profitTitle;
    [SerializeField] private TMP_Text profitText;
    [SerializeField] private Image profitBG;

    private TripData currentTrip;
    private float mileage;
    private float fuelPrice;

    private List<TripData> trips = new List<TripData>();

    private static TripManager instance;

    void OnEnable()
    {
        CalenderScript.OnDateSelected += SetDate;
    }

    private void SetDate(DateTime time1, DateTime time2)
    {
        if (time1 == time2)
        {
            dateText.text = time1.ToString("dd MMM yyyy");
        }
    }

    void OnDisable()
    {
        CalenderScript.OnDateSelected -= SetDate;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        trips = JSONHelper.LoadFromJson<List<TripData>>("trips.json") ?? new List<TripData>();
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(AddTrip);
        closeButton.onClick.AddListener(ClosePopup);
        dateSelectorButton.onClick.AddListener(() => CalenderScript.instance.GenerateDailyCalendar(DateTime.Now.Year, DateTime.Now.Month));

        startTripReading.onValueChanged.AddListener(delegate { UpdateDynamicValues(); });
        endTripReading.onValueChanged.AddListener(delegate { UpdateDynamicValues(); });
        earningsInput.onValueChanged.AddListener(delegate { UpdateDynamicValues(); });
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
        gameObject.SetActive(true);

        currentTrip = new TripData();

        dateText.text = DateTime.Now.ToString("dd MMM yyyy");

        mileage = PlayerPrefs.GetFloat("Mileage", 40f);
        fuelPrice = PlayerPrefs.GetFloat("FuelPrice", 103.93f);
    }

    private void AddTrip()
    {
        if (!float.TryParse(startTripReading.text, out currentTrip.startOdometer) ||
            !float.TryParse(endTripReading.text, out currentTrip.endOdometer) ||
            !float.TryParse(earningsInput.text, out currentTrip.earnings))
        {
            Debug.LogError("❌ Invalid input in trip fields.");
            return;
        }

        currentTrip.date = DateTime.Parse(dateText.text);
        currentTrip.fuelExpense = (currentTrip.distance / mileage) * fuelPrice;
        currentTrip.profit = currentTrip.earnings - currentTrip.fuelExpense;

        trips.Add(currentTrip);

        JSONHelper.SaveToJson("trips.json", trips);

        ClosePopup();
    }

    private void UpdateDynamicValues()
    {
        if (!float.TryParse(startTripReading.text, out float startOdo)) startOdo = 0;
        if (!float.TryParse(endTripReading.text, out float endOdo)) endOdo = startOdo; // Default to startOdo to avoid negative distance
        if (!float.TryParse(earningsInput.text, out float earnings)) earnings = 0;

        // // Ensure end odometer is never smaller than start odometer
        // if (endOdo < startOdo)
        // {
        //     endOdo = startOdo;
        //     endTripReading.text = startOdo.ToString();
        //     Debug.LogError("End reading cannot be smaller than start reading");
        // }

        currentTrip.startOdometer = startOdo;
        currentTrip.endOdometer = endOdo;
        currentTrip.earnings = earnings;

        float distance = endOdo - startOdo;
        float fuelExpense = (distance / mileage) * fuelPrice;
        float profit = earnings - fuelExpense;

        distanceText.text = $"{distance:F2} km";
        fuelCostText.text = $"Rs. {fuelExpense:F2}";
        profitText.text = $"Rs. {profit:F2}";

        if (profit >= 0)
        {
            profitText.color = Color.green;
            profitBG.color = new Color(0.5f, 1f, 0.5f, 0.5f);
            profitTitle.text = "Profit";
        }
        else
        {
            profitText.color = Color.red;
            profitBG.color = new Color(1f, 0.5f, 0.5f, 0.5f);
            profitTitle.text = "Loss";
        }
    }

        private void ClosePopup()
        {
            startTripReading.text = string.Empty;
            endTripReading.text = string.Empty;
            earningsInput.text = string.Empty;
            fuelCostText.text = "Rs. 0.00";
            distanceText.text = "0 km";
            gameObject.SetActive(false);
        }
    }

[System.Serializable]
public class TripData
{
    public DateTime date;
    public float startOdometer;
    public float endOdometer;
    public float distance => Mathf.Max(endOdometer - startOdometer, 0);
    public float fuelExpense;
    public float earnings;
    public float profit;
}
