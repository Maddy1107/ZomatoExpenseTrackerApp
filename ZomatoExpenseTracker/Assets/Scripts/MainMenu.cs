using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text earningsText, deductionsText, expensesText, profitText;
    [SerializeField] private Button addTripButton, addDeductionButton;
    public GameObject tripPopup, deductionPopup;

    private void OnEnable()
    {
        addTripButton.onClick.AddListener(OpenTripPopup);
        addDeductionButton.onClick.AddListener(OpenDeductionPopup);
        LoadLifetimeStats();
    }

    public void OpenTripPopup()
    {
        tripPopup.SetActive(true);
    }   
    public void OpenDeductionPopup()
    {
        deductionPopup.SetActive(true);
    }   

    private void LoadLifetimeStats()
    {
        UserData userData = JSONHelper.LoadFromJson<UserData>("userdata.json") ?? new UserData();

        float lifetimeEarnings = userData.trips.Sum(trip => trip.earnings);

        float lifetimeDeductions = userData.deductions.Sum(d => d.amount);

        float lifetimeExpenses = userData.trips.Sum(trip => trip.fuelExpense);
        
        float lifetimeProfit = lifetimeEarnings - lifetimeDeductions - lifetimeExpenses;

        // Display values in UI
        earningsText.text = $"Rs. {lifetimeEarnings:F2}";
        deductionsText.text = $"Rs. {lifetimeDeductions:F2}";
        expensesText.text = $"Rs. {lifetimeExpenses:F2}";
        profitText.text = $"Rs. {lifetimeProfit:F2}";
    }

    private void OnDisable()
    {
        addTripButton.onClick.RemoveListener(TripPopup.OpenTripPopup);
    }
}
