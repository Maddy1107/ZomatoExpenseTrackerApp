using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text cityNameText;

    public Button startTripButton;
    public TMP_Text startTripButtonText;

    public bool tripActive = false;

    public TripManager tripPopup;

    void OnEnable()
    {
        cityNameText.text = PlayerPrefs.GetString("SavedCity");


        tripPopup = FindObjectOfType<TripManager>(true);
    }

    private void StartTrip()
    {
        tripActive = true;
        startTripButton.onClick.AddListener(tripActive ? EndTrip : StartTrip);
        tripPopup.gameObject.SetActive(true);
        tripPopup.ShowPopup(true);
        startTripButtonText.text = "End Trip";
    }

    private void EndTrip()
    {
        tripActive = false;
        startTripButton.onClick.AddListener(tripActive ? EndTrip : StartTrip);
        tripPopup.gameObject.SetActive(true);
        tripPopup.ShowPopup(false);
        startTripButtonText.text = "Start Trip";
    }
}
