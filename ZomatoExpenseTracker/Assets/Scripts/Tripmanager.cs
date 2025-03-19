using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class TripManager : MonoBehaviour {
    public GameObject tripPopup;  // Popup UI
    public TextMeshProUGUI tripButtonText;  // "Start Trip" / "End Trip" button text
    public TMP_InputField startTripReading, endTripReading;  // Input for odometer readings
    public TMP_InputField earningsInput, expensesInput;  // Fields for earnings & other expenses
    public TextMeshProUGUI fuelExpenseText;  // Auto-calculated fuel expense
    public GameObject start, end;

    private bool tripActive = false;
    private float startOdometer;
    private DateTime startTime;
    private List<TripData> trips = new List<TripData>();

    void Start() {
        LoadTrips();
        tripButtonText.text = tripActive ? "End Trip" : "Start Trip";
    }

    void OnEnable() {
        start.SetActive(false);
        end.SetActive(false);
    }

    // Show the popup when clicking "Start Trip" or "End Trip"
    public void ShowTripPopup() {
        tripPopup.SetActive(true);
        
        if (!tripActive) {
            // Starting a trip
            start.SetActive(true);
            tripPopup.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Start Trip";
            startTripReading.text = "";
            endTripReading.gameObject.SetActive(false);
            earningsInput.gameObject.SetActive(false);
            expensesInput.gameObject.SetActive(false);
            fuelExpenseText.gameObject.SetActive(false);
        } else {
            // Ending a trip
            end.SetActive(true);
            tripPopup.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "End Trip";
            startTripReading.interactable = false;
            endTripReading.text = "";
            endTripReading.gameObject.SetActive(true);
            earningsInput.gameObject.SetActive(true);
            expensesInput.gameObject.SetActive(true);
            fuelExpenseText.gameObject.SetActive(true);
        }
    }

    // Confirm button functionality
    public void ConfirmTrip() {
        if (!tripActive) {
            // Start trip
            if (float.TryParse(startTripReading.text, out startOdometer)) {
                startTime = DateTime.Now;
                tripActive = true;
                tripButtonText.text = "End Trip";
                tripPopup.SetActive(false);
            }
        } else {
            // End trip
            if (float.TryParse(endTripReading.text, out float endOdometer) &&
                float.TryParse(earningsInput.text, out float earnings) &&
                float.TryParse(expensesInput.text, out float otherExpenses)) {

                float distance = endOdometer - startOdometer;
                float mileage = PlayerPrefs.GetFloat("Mileage", 40f);  // Default 40km/L if not set
                float fuelPrice = PlayerPrefs.GetFloat("FuelPrice", 100f);  // Default ₹100/L if not fetched
                float fuelExpense = (distance / mileage) * fuelPrice;

                fuelExpenseText.text = "Fuel Expense: ₹" + fuelExpense.ToString("F2");

                // Save Trip Details
                SaveTrip(distance, fuelExpense, earnings, otherExpenses);

                tripActive = false;
                tripButtonText.text = "Start Trip";
                tripPopup.SetActive(false);
            }
        }
    }

    void SaveTrip(float distance, float fuelExpense, float earnings, float otherExpenses) {
        TripData trip = new TripData {
            date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            duration = (System.DateTime.Now - startTime).ToString(@"hh\:mm"),
            distance = distance,
            fuelExpense = fuelExpense,
            earnings = earnings,
            otherExpenses = otherExpenses,
            profit = earnings - (fuelExpense + otherExpenses)
        };

        trips.Add(trip);
        SaveTripsToPrefs();
    }

    void SaveTripsToPrefs() {
        string json = JsonUtility.ToJson(new TripListWrapper { trips = trips });
        PlayerPrefs.SetString("TripData", json);
        PlayerPrefs.Save();
    }

    void LoadTrips() {
        string json = PlayerPrefs.GetString("TripData", "{}");
        TripListWrapper tripWrapper = JsonUtility.FromJson<TripListWrapper>(json);

        if (tripWrapper != null && tripWrapper.trips != null) {
            trips = tripWrapper.trips;
        }
    }
}

[System.Serializable]
public class TripData {
    public string date;
    public string duration;
    public float distance;
    public float fuelExpense;
    public float earnings;
    public float otherExpenses;
    public float profit;
}

[System.Serializable]
public class TripListWrapper {
    public List<TripData> trips;
}
