using UnityEngine;
using TMPro;

public class TripItem : MonoBehaviour
{
    public TMP_Text dateText;
    public TMP_Text durationText;
    public TMP_Text profitText;

    public void SetTripData(TripData trip)
    {
        dateText.text = trip.date.ToString("dd MMM yyyy");
        durationText.text = $"{trip.Duration}";
        profitText.text = $"Rs.{trip.profit:F2}";
    }

    public void Print()
    {
        Debug.Log("Printing trip data...");
    }
}
