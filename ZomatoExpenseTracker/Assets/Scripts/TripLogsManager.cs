// using UnityEngine;
// using TMPro;
// using System.Collections.Generic;

// public class TripLogsManager : MonoBehaviour {
//     public GameObject tripItemPrefab;  // Trip item template prefab
//     public Transform tripListContent;  // Content panel inside Scroll View
//     public GameObject tripDetailsPopup;  // Popup for trip details
//     public TextMeshProUGUI tripDetailsText;  // Text in popup

//     private List<TripData> trips = new List<TripData>();

//     void Start() {
//         LoadTrips();
//     }

//     void LoadTrips() {
//         tripListContent.DetachChildren();

//         foreach (TripData trip in trips) {
//             GameObject tripItem = Instantiate(tripItemPrefab, tripListContent);
//             tripItem.transform.Find("DateText").GetComponent<TextMeshProUGUI>().text = trip.date;
//             tripItem.transform.Find("DurationText").GetComponent<TextMeshProUGUI>().text = trip.duration;
//             tripItem.transform.Find("ProfitText").GetComponent<TextMeshProUGUI>().text = "₹" + trip.profit;

//             tripItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ShowTripDetails(trip));
//         }
//     }

//     void ShowTripDetails(TripData trip) {
//         tripDetailsText.text = $"Date: {trip.date}\nDuration: {trip.duration}\nDistance: {trip.distance} km\nFuel: ₹{trip.fuelExpense}\nEarnings: ₹{trip.earnings}\nOther Expenses: ₹{trip.otherExpenses}";
//         tripDetailsPopup.SetActive(true);
//     }
// }
