using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

public class TripPopup : MonoBehaviour
{
    public static TripPopup Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Button confirmButton, dateSelectorButton, closeButton;
    [SerializeField] private TMP_InputField startTripReading, endTripReading, earningsInput,fuelPriceInput;
    [SerializeField] private TMP_Text fuelCostText, dateText, distanceText, fuelPriceText;
    [SerializeField] private TMP_Text profitTitle, profitText;
    [SerializeField] private Image profitBG;

    private float mileage;
    private UserData userData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Load UserData from JSON
        userData = JSONHelper.LoadFromJson<UserData>("userdata.json") ?? new UserData();
    }

    private void OnEnable()
    {
        ShowPopup();
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(AddTrip);
        closeButton.onClick.AddListener(ClosePopup);
        dateSelectorButton.onClick.AddListener(() => GenerateCalender());

        startTripReading.onValueChanged.AddListener(_ => UpdateDynamicValues());
        endTripReading.onValueChanged.AddListener(_ => UpdateDynamicValues());
        earningsInput.onValueChanged.AddListener(_ => UpdateDynamicValues());
    }

    public static void OpenTripPopup()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<TripPopup>(true);
            if (Instance == null) { Debug.LogError("TripManager is missing in the scene!"); return; }
        }
        Instance.ShowPopup();
    }

    private void ShowPopup()
    {
        gameObject.SetActive(true);
        dateText.text = DateTime.Now.ToString("dd MMM yyyy");
        mileage = PlayerPrefs.GetFloat("Mileage", 40f);
        ResetInputs();
    }
    #if UNITY_ANDROID
        using UnityEngine.Android;
    #endif

    public void GenerateCalender()
    {
    #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.CalendarRead))
        {
            Permission.RequestUserPermission(Permission.CalendarRead);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.CalendarRead))
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW");
            intent.Call<AndroidJavaObject>("setData", AndroidJavaObject.CallStatic<AndroidJavaObject>("android.net.Uri", "content://com.android.calendar/time"));
            currentActivity.Call("startActivity", intent);
        }
        else
        {
            Debug.LogError("Calendar permission not granted.");
        }
    #else
        Debug.LogError("Calendar integration is only supported on Android.");
    #endif
    }
    
    private void SetDate(DateTime selectedDate, DateTime _)
    {
        dateText.text = selectedDate.ToString("dd MMM yyyy");
    }

    private void AddTrip()
    {
        if (!TryParseInputs(out int startOdo, out int endOdo, out float earnings, out float fuelprice)) return;

        DateTime tripDate;
        if (!DateTime.TryParse(dateText.text, out tripDate))
        {
            Debug.LogError("❌ Invalid date format.");
            return;
        }

        TripData newTrip = new TripData
        {
            date = tripDate,
            startOdometer = startOdo,
            endOdometer = endOdo,
            earnings = earnings
        };

        newTrip.fuelExpense = (newTrip.distance / mileage) * fuelprice;
        newTrip.profit = newTrip.earnings - newTrip.fuelExpense;

        // Add trip to UserData
        userData.trips.Add(newTrip);
        SaveUserData();

        ClosePopup();
    }

    private void UpdateDynamicValues()
    {
        if (!TryParseInputs(out int startOdo, out int endOdo, out float earnings, out float fuelprice)) return;

        float distance = Mathf.Max(endOdo - startOdo, 0);
        float fuelExpense = (distance / mileage) * fuelprice;
        float profit = earnings - fuelExpense;

        distanceText.text = $"{distance:F2} km";
        fuelCostText.text = $"Rs. {fuelExpense:F2}";
        profitText.text = $"Rs. {profit:F2}";

        profitText.color = profit >= 0 ? Color.green : Color.red;
        profitBG.color = profit >= 0 ? new Color(0.5f, 1f, 0.5f, 0.5f) : new Color(1f, 0.5f, 0.5f, 0.5f);
        profitTitle.text = profit >= 0 ? "Profit" : "Loss";
    }

    private bool TryParseInputs(out int startOdo, out int endOdo, out float earnings, out float fuelprice)
    {
        startOdo = endOdo = 0;
        earnings = fuelprice = 0f;

        bool valid = int.TryParse(startTripReading.text, out startOdo)
                && int.TryParse(endTripReading.text, out endOdo)
                && float.TryParse(earningsInput.text, out earnings)
                && float.TryParse(fuelPriceInput.text, out fuelprice);

        if (!valid)
        {
            Debug.LogError("❌ Invalid input in trip fields.");
            return false;
        }

        return true;
    }

    private void ClosePopup()
    {
        ResetInputs();
        gameObject.SetActive(false);
    }

    private void ResetInputs()
    {
        startTripReading.text = endTripReading.text = earningsInput.text = string.Empty;
        fuelCostText.text = "Rs. 0.00";
        distanceText.text = "0 km";
    }

    private void SaveUserData()
    {
        JSONHelper.SaveToJson("userdata.json", userData);
    }
}


[System.Serializable]
public class TripData
{
    public DateTime date;
    public float startOdometer;
    public float endOdometer;
    public float distance => Mathf.Max(endOdometer - startOdometer, 0);
    public float fuelExpense;
    public float earnings;
    public float profit;
}

[System.Serializable]
public class DeductionData
{
    public DateTime date;
    public string reason;
    public float amount;
}

[System.Serializable]
public class UserData
{
    public List<TripData> trips = new List<TripData>();
    public List<DeductionData> deductions = new List<DeductionData>();
}
