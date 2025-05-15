using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TripItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text earnedText, distanceText, fuelCostText, profitText, datetext;

    public void SetTripData(TripData trips)
    {
        earnedText.text = $"Rs. {trips.earnings:F2}";
        profitText.text = $"Rs. {trips.profit:F2}";
        distanceText.text = $"{trips.distance:F2} km";
        fuelCostText.text = $"Rs. {trips.fuelExpense:F2}";
        datetext.text = trips.date.ToString("dd MMM yyyy");
    }

    public void Print()
    {
        Debug.Log("Printing trip data...");
    }
}
