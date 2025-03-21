using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JSONHelper
{
    public static void SaveToJson<T>(string fileName, T data)
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Debug.Log($"✅ Data saved to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error saving JSON: {ex.Message}");
        }
    }

    public static T LoadFromJson<T>(string fileName)
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                Debug.LogWarning($"⚠️ File not found: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error loading JSON: {ex.Message}");
        }

        return default;
    }

    public static bool FileExists(string fileName)
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, fileName));
    }

    public static void DeleteFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"🗑️ Deleted file: {filePath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Cannot delete, file not found: {filePath}");
        }
    }
}
