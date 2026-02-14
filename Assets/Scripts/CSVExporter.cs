using System.IO;
using UnityEngine;

public class CSVExporter : MonoBehaviour
{
    public static string ExportToCSV()
    {
        if (StatsTracker.Instance == null) return null;

        string folder = Path.Combine(Application.persistentDataPath, "Exports");
        Directory.CreateDirectory(folder);

        string fileName = "GameStats_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        string filePath = Path.Combine(folder, fileName);

        // Header + 1 row
        string header = "Player Score,Enemies Killed,Accuracy";
        string row = $"{StatsTracker.Instance.playerScore},{StatsTracker.Instance.enemiesKilled},{StatsTracker.Instance.AccuracyPercent:F2}%";

        File.WriteAllText(filePath, header + "\n" + row);

        Debug.Log("✅ CSV exported to: " + filePath);
        return filePath;
    }
}
