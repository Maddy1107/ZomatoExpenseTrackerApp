using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JSONHelper
{
    private static string GetFilePath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

    public static void SaveToJson<T>(string fileName, T data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(GetFilePath(fileName), json);
            Debug.Log($"✅ Data saved to {fileName}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error saving JSON: {ex.Message}");
        }
    }

    public static T LoadFromJson<T>(string fileName)
    {
        string filePath = GetFilePath(fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"⚠️ File not found: {fileName}");
            return default;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error loading JSON: {ex.Message}");
            return default;
        }
    }

    public static bool FileExists(string fileName) => File.Exists(GetFilePath(fileName));

    public static void DeleteFile(string fileName)
    {
        string filePath = GetFilePath(fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"🗑️ Deleted file: {fileName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Cannot delete, file not found: {fileName}");
        }
    }
}
