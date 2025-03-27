using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EarningsBarChart : MonoBehaviour
{
    [SerializeField] private RectTransform barContainer;
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private TMP_Text titleText;

    void OnEnable()
    {
        CalenderScript.OnDateSelected += GenerateBarChart;
    }

    void OnDisable()
    {
        CalenderScript.OnDateSelected -= GenerateBarChart;
    }

    public void GenerateBarChart(DateTime time1, DateTime time2)
    {
        List<TripData> allTrips = JSONHelper.LoadFromJson<List<TripData>>("trips.json") ?? new List<TripData>();
        List<TripData> filteredTrips = allTrips.FindAll(trip => trip.date >= time1 && trip.date <= time2);

        if ((time2 - time1).TotalDays <= 7)
        {
            GenerateWeeklyChart(filteredTrips);
        }
        else
        {
            GenerateMonthlyChart(filteredTrips);
        }
    }

    public void GenerateWeeklyChart(List<TripData> trips)
    {
        ClearChart();

        DateTime startOfWeek = trips.Count > 0 ? trips[0].date.AddDays(-(int)trips[0].date.DayOfWeek + 1) : DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
        
        Dictionary<string, float> dailyEarnings = new Dictionary<string, float>();

        for (int i = 0; i < 7; i++)
        {
            string day = startOfWeek.AddDays(i).ToString("dd");
            dailyEarnings[day] = 0; 
        }

        foreach (var trip in trips)
        {
            string day = trip.date.ToString("dd");
            if (dailyEarnings.ContainsKey(day))
            {
                dailyEarnings[day] += trip.earnings;
            }
        }

        CreateBars(dailyEarnings.Where(d => d.Value > 0).ToDictionary(d => d.Key, d => d.Value));
    }


    public void GenerateMonthlyChart(List<TripData> trips)
    {
        ClearChart();

        if (trips.Count == 0) return;

        DateTime firstDay = new DateTime(trips[0].date.Year, trips[0].date.Month, 1);
        DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

        Dictionary<string, float> weeklyEarnings = new Dictionary<string, float>();

        for (DateTime weekStart = firstDay; weekStart <= lastDay; weekStart = weekStart.AddDays(7))
        {
            string weekLabel = $"{weekStart:dd} - {weekStart.AddDays(6):dd}";
            weeklyEarnings[weekLabel] = 0; 
        }

        foreach (var trip in trips)
        {
            DateTime startOfWeek = trip.date.AddDays(-(int)trip.date.DayOfWeek + (int)DayOfWeek.Monday);
            string weekLabel = $"{startOfWeek:dd} - {startOfWeek.AddDays(6):dd}";

            if (weeklyEarnings.ContainsKey(weekLabel))
            {
                weeklyEarnings[weekLabel] += trip.earnings;
            }
        }

        CreateBars(weeklyEarnings.Where(w => w.Value > 0).ToDictionary(w => w.Key, w => w.Value));
    }


    private void CreateBars(Dictionary<string, float> earningsData)
    {
        float maxEarnings = earningsData.Count > 0 ? Mathf.Max(1, Mathf.Max(new List<float>(earningsData.Values).ToArray())) : 1f;

        foreach (var entry in earningsData)
        {
            GameObject bar = Instantiate(barPrefab, barContainer);
            bar.transform.Find("Label").GetComponent<TMP_Text>().text = entry.Key;
            bar.transform.Find("Value").GetComponent<TMP_Text>().text = $"Rs.{entry.Value:F2}";

            float height = (entry.Value / maxEarnings) * 200f; // Scale bars
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, height);
        }
    }

    private void ClearChart()
    {
        foreach (Transform child in barContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
