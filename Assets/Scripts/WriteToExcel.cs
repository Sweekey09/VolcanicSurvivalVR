using System;
using System.IO;
using UnityEngine;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class WriteToExcel : MonoBehaviour
{
    [Header("User")]
    public string userName = "User1"; // you can set from input field

    [Header("Tasks")]
    // Edit this to match your lab tasks (colors / steps / sequence)
    public string[] taskNames = new string[] { "Task1", "Task2", "Task3" };

    // 0 = NotDone, 1 = Done, 2 = Skipped
    private int[] taskStatus;

    [Header("Export")]
    public string sheetName = "sheet1";
    public bool createNewFileEachDay = true;

    // Example output: Export/20260214_TestResult.xls
    private string excelPath;

    private void Awake()
    {
        taskStatus = new int[taskNames.Length];
        for (int i = 0; i < taskStatus.Length; i++) taskStatus[i] = 0;

        string fileName = createNewFileEachDay
            ? DateTime.Now.ToString("yyyyMMdd") + "_TestResult.xls"
            : "TestResult.xls";

        // IMPORTANT: save to ProjectRoot/Export (same level as Assets)
        excelPath = Application.dataPath + "/../Export/" + fileName;

        Debug.Log("Excel path: " + excelPath);

        EnsureWorkbookReady();
    }

    // =========================
    // Call these from buttons / triggers
    // =========================

    // Mark a task as DONE
    public void MarkTaskDone(int taskIndex)
    {
        if (!IsValidTask(taskIndex)) return;
        taskStatus[taskIndex] = 1;
        Debug.Log($"Task {taskIndex} DONE");
    }

    // Mark a task as SKIPPED (counts as fail if you want)
    public void MarkTaskSkipped(int taskIndex)
    {
        if (!IsValidTask(taskIndex)) return;
        taskStatus[taskIndex] = 2;
        Debug.Log($"Task {taskIndex} SKIPPED");
    }

    // Call this when testing phase ends (button/timer/sequence end)
    public void EndTestAndWriteRecord()
    {
        // PASS only if all tasks are Done (no Skipped / NotDone)
        bool pass = true;
        for (int i = 0; i < taskStatus.Length; i++)
        {
            if (taskStatus[i] != 1)
            {
                pass = false;
                break;
            }
        }

        AppendRecordToExcel(userName, taskStatus, pass ? "PASS" : "FAIL");
        Debug.Log("Record saved: " + (pass ? "PASS" : "FAIL"));

        // optional: reset for next run
        ResetTasks();
    }

    public void ResetTasks()
    {
        for (int i = 0; i < taskStatus.Length; i++) taskStatus[i] = 0;
    }

    // =========================
    // Excel Core (NPOI HSSF .xls)
    // =========================

    private void EnsureWorkbookReady()
    {
        // Ensure Export folder exists
        string exportDir = Path.GetDirectoryName(excelPath);
        if (!Directory.Exists(exportDir))
            Directory.CreateDirectory(exportDir);

        if (!File.Exists(excelPath))
        {
            // Create new workbook + sheet + header + END row
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet(sheetName);

            CreateHeaderRow(sheet);
            WriteEndRow(sheet);

            using (FileStream fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
            {
                book.Write(fs);
            }

            Debug.Log("Created new Excel: " + excelPath);
        }
        else
        {
            // Ensure header exists + ensure END exists
            HSSFWorkbook book;
            using (FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
            {
                book = new HSSFWorkbook(fs);
            }

            ISheet sheet = book.GetSheet(sheetName) ?? book.CreateSheet(sheetName);

            // Create header if missing
            if (sheet.GetRow(0) == null || sheet.GetRow(0).GetCell(0) == null)
                CreateHeaderRow(sheet);

            // Ensure END row exists at bottom
            EnsureEndRow(sheet);

            using (FileStream fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
            {
                book.Write(fs);
            }
        }
    }

    private void AppendRecordToExcel(string username, int[] statuses, string result)
    {
        EnsureWorkbookReady();

        HSSFWorkbook book;
        using (FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
        {
            book = new HSSFWorkbook(fs);
        }

        ISheet sheet = book.GetSheet(sheetName) ?? book.CreateSheet(sheetName);

        // Remove existing END row so we can append above it
        int endRowIndex = FindEndRowIndex(sheet);
        if (endRowIndex >= 0)
        {
            sheet.RemoveRow(sheet.GetRow(endRowIndex));
        }

        int nextRow = sheet.LastRowNum + 1;
        if (nextRow < 1) nextRow = 1; // row 0 is header

        IRow row = sheet.CreateRow(nextRow);

        // Col 0: Timestamp
        row.CreateCell(0).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        // Col 1: Username
        row.CreateCell(1).SetCellValue(username);

        // Col 2..: Task statuses
        for (int i = 0; i < statuses.Length; i++)
        {
            row.CreateCell(2 + i).SetCellValue(StatusToString(statuses[i]));
        }

        // Last col: Result
        row.CreateCell(2 + statuses.Length).SetCellValue(result);

        // Put END row again at bottom
        WriteEndRow(sheet);

        using (FileStream fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
        {
            book.Write(fs);
        }
    }

    private void CreateHeaderRow(ISheet sheet)
    {
        IRow header = sheet.GetRow(0) ?? sheet.CreateRow(0);

        header.CreateCell(0).SetCellValue("Timestamp");
        header.CreateCell(1).SetCellValue("User");

        for (int i = 0; i < taskNames.Length; i++)
        {
            header.CreateCell(2 + i).SetCellValue(taskNames[i]);
        }

        header.CreateCell(2 + taskNames.Length).SetCellValue("Result");
    }

    private void WriteEndRow(ISheet sheet)
    {
        int rowIndex = sheet.LastRowNum + 1;
        if (rowIndex < 1) rowIndex = 1;

        IRow endRow = sheet.CreateRow(rowIndex);
        endRow.CreateCell(0).SetCellValue("-END-");
    }

    private void EnsureEndRow(ISheet sheet)
    {
        int endRowIndex = FindEndRowIndex(sheet);
        if (endRowIndex < 0)
        {
            WriteEndRow(sheet);
        }
        else
        {
            // If END exists but not last row, remove and rewrite at bottom
            if (endRowIndex != sheet.LastRowNum)
            {
                sheet.RemoveRow(sheet.GetRow(endRowIndex));
                WriteEndRow(sheet);
            }
        }
    }

    private int FindEndRowIndex(ISheet sheet)
    {
        for (int r = 0; r <= sheet.LastRowNum; r++)
        {
            IRow row = sheet.GetRow(r);
            if (row == null) continue;

            ICell cell = row.GetCell(0);
            if (cell == null) continue;

            if (cell.CellType == CellType.String && cell.StringCellValue == "-END-")
                return r;
        }
        return -1;
    }

    private string StatusToString(int s)
    {
        // 0 NotDone, 1 Done, 2 Skipped
        if (s == 1) return "Done";
        if (s == 2) return "Skipped";
        return "NotDone";
    }

    private bool IsValidTask(int i)
    {
        if (taskNames == null || taskNames.Length == 0)
        {
            Debug.LogError("taskNames is empty!");
            return false;
        }

        if (i < 0 || i >= taskNames.Length)
        {
            Debug.LogError("Invalid task index: " + i);
            return false;
        }
        return true;
    }
        void Update()
{
    if (Input.GetKeyDown(KeyCode.K))
    {
        MarkTaskDone(0);
        MarkTaskDone(1);
        MarkTaskDone(2);
        EndTestAndWriteRecord();
        Debug.Log("K pressed -> wrote record");
    }
}
}
