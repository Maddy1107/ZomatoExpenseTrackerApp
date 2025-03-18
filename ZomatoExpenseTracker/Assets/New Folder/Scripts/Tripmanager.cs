using UnityEngine;
using TMPro;
using System;

public class TripManager : MonoBehaviour {
    public GameObject tripPopup;  // Popup UI
    public TextMeshProUGUI tripButtonText;  // "Start Trip" / "End Trip" button text
    public TMP_InputField startTripReading, endTripReading;  // Input for odometer readings
    public TMP_InputField earningsInput, expensesInput;  // Fields for earnings & other expenses
    public TextMeshProUGUI fuelExpenseText;  // Auto-calculated fuel expense

    public GameObject start,end;

    private bool tripActive = false;
    private float startOdometer;
    private DateTime startTime;

    void OnEnable()
    {
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
            end.SetActive(true);
            // Ending a trip
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
            if (float.TryParse(startTripReading.text, out float endOdometer)) {
                float distance = endOdometer - startOdometer;
                float mileage = PlayerPrefs.GetFloat("Mileage", 40f);  // Default 40km/L if not set
                float fuelPrice = PlayerPrefs.GetFloat("FuelPrice", 100f);  // Default ₹100/L if not fetched
                float fuelExpense = distance / mileage * fuelPrice;

                fuelExpenseText.text = "Fuel Expense: ₹" + fuelExpense.ToString("F2");

                // Save Trip Details
                SaveTrip(distance, fuelExpense);
                
                tripActive = false;
                tripButtonText.text = "Start Trip";
                tripPopup.SetActive(false);
            }
        }
    }

    // Save trip details
    void SaveTrip(float distance, float fuelExpense) {
        PlayerPrefs.SetFloat("LastTripDistance", distance);
        PlayerPrefs.SetFloat("LastTripFuelExpense", fuelExpense);
        PlayerPrefs.SetString("LastTripDate", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }
}
