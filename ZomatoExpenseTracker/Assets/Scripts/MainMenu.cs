using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text cityNameText;
    public Button startTripButton;
    public TMP_Text startTripButtonText;

    public Button tripLogsButton;

    void OnEnable()
    {
        cityNameText.text = PlayerPrefs.GetString("SavedCity", "Unknown City");
        startTripButton.onClick.AddListener(ToggleTrip);
        tripLogsButton.onClick.AddListener(OpenTripLogs);
        
        TripManager.OnTripStatusChanged += UpdateButtonText;

        UpdateButtonText();
    }

    void OnDisable()
    {
        TripManager.OnTripStatusChanged -= UpdateButtonText;
    }

    private void ToggleTrip()
    {
        TripManager.OpenTripPopup();
        UpdateButtonText();
    }

    public void OpenTripLogs()
    {
        tripLogsButton.interactable = false;
        TripLogsManager.OpenTripLogs();
    }

    private void UpdateButtonText()
    {
        bool tripActive = PlayerPrefs.GetInt("TripActive", 0) == 1;
        startTripButtonText.text = tripActive ? "End Trip" : "Start Trip";
    }
}
