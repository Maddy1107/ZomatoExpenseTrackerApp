using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class StartUpPermissions : MonoBehaviour
{
    private static StartUpPermissions instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string savedCityKey = "SavedCity";

    void Start()
    {
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {
#if UNITY_EDITOR
        float latitude = 22.5744f;  // Pune
        float longitude = 88.3629f;
        yield return StartCoroutine(GetCityName(latitude, longitude));
        yield break;  // Ensure method exits correctly
#else
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            ShowToast("Location permission needed!", true);
            yield break;  // Stop coroutine
        }

        if (!Input.location.isEnabledByUser)
        {
            ShowToast("Please enable location services!", true);
            yield break;  // Stop coroutine
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            ShowToast("Failed to get location", true);
            yield break;  // Stop coroutine
        }

        float latitude = Input.location.lastData.latitude;
        float longitude = Input.location.lastData.longitude;
        Input.location.Stop();

        yield return StartCoroutine(GetCityName(latitude, longitude));  // Ensure coroutine completes
#endif
    }


    IEnumerator GetCityName(float latitude, float longitude)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}&zoom=10&addressdetails=1";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "UnityApp"); // Required by Nominatim API
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ShowToast("Error fetching city", true);
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            JObject json = JObject.Parse(jsonResponse);
            string city = json["address"]?["city"]?.ToString() ?? json["address"]?["town"]?.ToString() ?? json["address"]?["village"]?.ToString();

            if (!string.IsNullOrEmpty(city))
            {
                string savedCity = PlayerPrefs.GetString(savedCityKey, "");
                if (savedCity != city)
                {
                    PlayerPrefs.SetString(savedCityKey, city);
                    PlayerPrefs.Save();
                    ShowToast($"Location updated: {city}", false);
                }
            }
        }
    }

    private void ShowToast(string message, bool isLongDuration)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, isLongDuration ? 1 : 0);
            toastObject.Call("show");
        }));
#else
        Debug.Log(message);
#endif
    }
}
