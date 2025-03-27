using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class TripItem : MonoBehaviour
{
    public TMP_Text earnedText,distanceText,fuelCostText,profitText;

    public void SetTripData(List<TripData> trips, Filtertype filterType)
    {
        float totalEarnings = 0f;
        float totalProfit = 0f;
        float totalFuelExpense = 0f;
        float totalDistance = 0f;

        foreach (var trip in trips)
        {
            totalEarnings += trip.earnings;
            totalProfit += trip.profit;
            totalFuelExpense += trip.fuelExpense;
            totalDistance += trip.distance;
        }

        if (filterType == Filtertype.Daily && trips.Count == 1)
        {
            earnedText.text = $"Rs.{totalEarnings:F2}";
            profitText.text = $"Rs.{totalProfit:F2}";
            distanceText.text = $"{totalDistance:F2} km";
            fuelCostText.text = $"Rs.{totalFuelExpense:F2}";
        }
        else if (filterType == Filtertype.Weekly || filterType == Filtertype.Monthly)
        {
            earnedText.text = $"Rs.{totalEarnings:F2} (Total)";
            profitText.text = $"Rs.{totalProfit:F2} (Total)";
            distanceText.GetComponentInParent<Image>().gameObject.SetActive(false);
            fuelCostText.GetComponentInParent<Image>().gameObject.SetActive(false);}
    }


    public void Print()
    {
        Debug.Log("Printing trip data...");
    }
}
