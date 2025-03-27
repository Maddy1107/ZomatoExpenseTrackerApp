using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text cityNameText;
    public Button addTripButton;

    public Button tripLogsButton;

    void OnEnable()
    {
        cityNameText.text = PlayerPrefs.GetString("SavedCity", "Unknown City");

        addTripButton.onClick.AddListener(AddTrip);
    }

    private void AddTrip()
    {
        TripManager.OpenTripPopup();
    }

}
