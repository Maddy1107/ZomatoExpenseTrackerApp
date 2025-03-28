using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TripItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text earnedText, distanceText, fuelCostText, profitText;
    [SerializeField] private GameObject distanceContainer, fuelCostContainer;

    public void SetTripData(List<TripData> trips, FilterType filterType)
    {
        float totalEarnings = 0f, totalProfit = 0f, totalFuelExpense = 0f, totalDistance = 0f;

        foreach (var trip in trips)
        {
            totalEarnings += trip.earnings;
            totalProfit += trip.profit;
            totalFuelExpense += trip.fuelExpense;
            totalDistance += trip.distance;
        }

        earnedText.text = $"Rs. {totalEarnings:F2}" + (filterType == FilterType.Daily ? "" : " (Total)");
        profitText.text = $"Rs. {totalProfit:F2}" + (filterType == FilterType.Daily ? "" : " (Total)");

        if (filterType == FilterType.Daily && trips.Count == 1)
        {
            distanceText.text = $"{totalDistance:F2} km";
            fuelCostText.text = $"Rs. {totalFuelExpense:F2}";
            SetVisibility(distanceContainer, true);
            SetVisibility(fuelCostContainer, true);
        }
        else
        {
            SetVisibility(distanceContainer, false);
            SetVisibility(fuelCostContainer, false);
        }
    }

    private void SetVisibility(GameObject container, bool isVisible)
    {
        if (container != null) container.SetActive(isVisible);
    }

    public void Print()
    {
        Debug.Log("Printing trip data...");
    }
}
