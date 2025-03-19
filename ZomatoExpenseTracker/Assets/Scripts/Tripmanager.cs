using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class TripManager : MonoBehaviour {

    public GameObject startTrip, endTrip;
    private bool tripActive = false;
    public Button startConfirmbtn, endConfirmbtn;

    public TMP_InputField startTripReading, endTripReading, earningsInput;

    private List<TripData> trips = new List<TripData>();

    TripData currentTrip;

    public void OnEnable() 
    {
        currentTrip = new TripData();

        if(!tripActive) {
            startTrip.SetActive(true);
            endTrip.SetActive(false);
        } else {
            startTrip.SetActive(false);
            endTrip.SetActive(true);
        }

        startConfirmbtn.onClick.AddListener(StartTrip);
        endConfirmbtn.onClick.AddListener(EndTrip);
    }

    private void StartTrip()
    {
        currentTrip.date = DateTime.Now;
        currentTrip.startOdometer = float.Parse(startTripReading.text);
        currentTrip.startTime = DateTime.Now.TimeOfDay;
    }

    private void EndTrip()
    {
        currentTrip.endTime = DateTime.Now.TimeOfDay;
        currentTrip.endOdometer = float.Parse(endTripReading.text);
        currentTrip.distance = float.Parse(endTripReading.text) - float.Parse(startTripReading.text);
        currentTrip.earnings = float.Parse(earningsInput.text);
        currentTrip.fuelExpense = currentTrip.distance / PlayerPrefs.GetFloat("Mileage", 40f) * PlayerPrefs.GetFloat("FuelPrice", 100f);
        currentTrip.profit = currentTrip.earnings - currentTrip.fuelExpense;
        
        trips.Add(currentTrip);
    }


    // // Show the popup when clicking "Start Trip" or "End Trip"
    // public void ShowTripPopup() {
    //     //tripPopup.SetActive(true);

    //     if (!tripActive) {
    //         // Starting a trip
    //         start.SetActive(true);
    //         //tripPopup.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Start Trip";
    //         startTripReading.text = "";
    //         endTripReading.gameObject.SetActive(false);
    //         earningsInput.gameObject.SetActive(false);
    //         expensesInput.gameObject.SetActive(false);
    //         fuelExpenseText.gameObject.SetActive(false);
    //     } else {
    //         // Ending a trip
    //         end.SetActive(true);
    //         //tripPopup.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "End Trip";
    //         startTripReading.interactable = false;
    //         endTripReading.text = "";
    //         endTripReading.gameObject.SetActive(true);
    //         earningsInput.gameObject.SetActive(true);
    //         expensesInput.gameObject.SetActive(true);
    //         fuelExpenseText.gameObject.SetActive(true);
    //     }
    // }

    // // Confirm button functionality
    // public void ConfirmTrip() {
    //     if (!tripActive) {
    //         // Start trip
    //         if (float.TryParse(startTripReading.text, out startOdometer)) {
    //             startTime = DateTime.Now;
    //             tripActive = true;
    //             tripButtonText.text = "End Trip";
    //             //tripPopup.SetActive(false);
    //         }
    //     } else {
    //         // End trip
    //         if (float.TryParse(endTripReading.text, out float endOdometer) &&
    //             float.TryParse(earningsInput.text, out float earnings) &&
    //             float.TryParse(expensesInput.text, out float otherExpenses)) {

    //             float distance = endOdometer - startOdometer;
    //             float mileage = PlayerPrefs.GetFloat("Mileage", 40f);  // Default 40km/L if not set
    //             float fuelPrice = PlayerPrefs.GetFloat("FuelPrice", 100f);  // Default ₹100/L if not fetched
    //             float fuelExpense = (distance / mileage) * fuelPrice;

    //             fuelExpenseText.text = "Fuel Expense: ₹" + fuelExpense.ToString("F2");

    //             // Save Trip Details
    //             SaveTrip(distance, fuelExpense, earnings, otherExpenses);

    //             tripActive = false;
    //             tripButtonText.text = "Start Trip";
    //             //tripPopup.SetActive(false);
    //         }
    //     }
    // }

    // void SaveTrip(float distance, float fuelExpense, float earnings, float otherExpenses) {
    //     TripData trip = new TripData {
    //         date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
    //         duration = (System.DateTime.Now - startTime).ToString(@"hh\:mm"),
    //         distance = distance,
    //         fuelExpense = fuelExpense,
    //         earnings = earnings,
    //         otherExpenses = otherExpenses,
    //         profit = earnings - (fuelExpense + otherExpenses)
    //     };

    //     trips.Add(trip);
    //     SaveTripsToPrefs();
    // }

    // void SaveTripsToPrefs() {
    //     string json = JsonUtility.ToJson(new TripListWrapper { trips = trips });
    //     PlayerPrefs.SetString("TripData", json);
    //     PlayerPrefs.Save();
    // }

    // void LoadTrips() {
    //     string json = PlayerPrefs.GetString("TripData", "{}");
    //     TripListWrapper tripWrapper = JsonUtility.FromJson<TripListWrapper>(json);

    //     if (tripWrapper != null && tripWrapper.trips != null) {
    //         trips = tripWrapper.trips;
    //     }
    // }
}

[System.Serializable]
public class TripData {
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

[System.Serializable]
public class TripListWrapper {
    public List<TripData> trips;
}
