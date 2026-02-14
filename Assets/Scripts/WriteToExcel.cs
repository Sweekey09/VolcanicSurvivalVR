using System;
using System.IO;
using UnityEngine;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class WriteToExcel : MonoBehaviour
{
    [Header("User")]
    public string userName = "User1";

    [Header("Tasks")]
    public string[] taskNames = new string[] { "Task1", "Task2", "Task3" };

    // 0 = NotDone, 1 = Done, 2 = Skipped
    private int[] taskStatus;

    [Header("Export")]
    public string sheetName = "sheet1";
    public bool createNewFileEachDay = true;

    private string excelPath;
    private bool initialized = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (initialized) return;
        initialized = true;

        if (taskNames == null || taskNames.Length == 0)
            taskNames = new string[] { "Task1", "Task2", "Task3" };

        taskStatus = new int[taskNames.Length];
        ResetTasks();

        string fileName = createNewFileEachDay
            ? DateTime.Now.ToString("yyyyMMdd") + "_TestResult.xls"
            : "TestResult.xls";

        // ProjectRoot/Export (same level as Assets)
        string exportDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Export"));
        excelPath = Path.Combine(exportDir, fileName);

        Debug.Log("Excel path used: " + excelPath);

        EnsureWorkbookReady();
    }

    // =========================
    // Public API (call from your game)
    // =========================

    public void MarkTaskDone(int taskIndex)
    {
        if (!IsValidTask(taskIndex)) return;
        taskStatus[taskIndex] = 1;
        Debug.Log($"Task {taskIndex} DONE");
    }

    public void MarkTaskSkipped(int taskIndex)
    {
        if (!IsValidTask(taskIndex)) return;
        taskStatus[taskIndex] = 2;
        Debug.Log($"Task {taskIndex} SKIPPED");
    }

    public void EndTestAndWriteRecord()
    {
        if (!initialized) Init();

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

        ResetTasks();
    }

    public void ResetTasks()
    {
        if (taskStatus == null) return;
        for (int i = 0; i < taskStatus.Length; i++) taskStatus[i] = 0;
    }

    // =========================
    // Excel Core (NPOI HSSF .xls)
    // =========================

    private void EnsureWorkbookReady()
    {
        string exportDir = Path.GetDirectoryName(excelPath);
        if (!Directory.Exists(exportDir))
            Directory.CreateDirectory(exportDir);

        if (!File.Exists(excelPath))
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet(sheetName);

            CreateHeaderRow(sheet);
            WriteEndRow(sheet);

            WriteWorkbookToDisk(book);
            Debug.Log("Created new Excel: " + excelPath);
        }
        else
        {
            HSSFWorkbook book = ReadWorkbookFromDisk();
            ISheet sheet = book.GetSheet(sheetName) ?? book.CreateSheet(sheetName);

            if (sheet.GetRow(0) == null || sheet.GetRow(0).GetCell(0) == null)
                CreateHeaderRow(sheet);

            EnsureEndRow(sheet);

            WriteWorkbookToDisk(book);
        }
    }

    private void AppendRecordToExcel(string username, int[] statuses, string result)
    {
        EnsureWorkbookReady();

        HSSFWorkbook book = ReadWorkbookFromDisk();
        ISheet sheet = book.GetSheet(sheetName) ?? book.CreateSheet(sheetName);

        // Remove END row so we append above it
        int endRowIndex = FindEndRowIndex(sheet);
        if (endRowIndex >= 0)
        {
            IRow endRow = sheet.GetRow(endRowIndex);
            if (endRow != null) sheet.RemoveRow(endRow);
        }

        int nextRowIndex = sheet.LastRowNum + 1;
        if (nextRowIndex < 1) nextRowIndex = 1; // row 0 = header

        IRow row = sheet.CreateRow(nextRowIndex);

        row.CreateCell(0).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        row.CreateCell(1).SetCellValue(username);

        for (int i = 0; i < statuses.Length; i++)
        {
            row.CreateCell(2 + i).SetCellValue(StatusToString(statuses[i]));
        }

        row.CreateCell(2 + statuses.Length).SetCellValue(result);

        // Add END row back
        WriteEndRow(sheet);

        WriteWorkbookToDisk(book);

        Debug.Log($"Wrote row {nextRowIndex} then -END- at bottom.");
    }

    private HSSFWorkbook ReadWorkbookFromDisk()
    {
        // Read fully into memory to avoid file locks
        byte[] bytes = File.ReadAllBytes(excelPath);
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            return new HSSFWorkbook(ms);
        }
    }

    private void WriteWorkbookToDisk(HSSFWorkbook book)
    {
        // Overwrite file safely (make sure Excel is CLOSED)
        using (FileStream fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            book.Write(fs);
            fs.Flush();
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
            return;
        }

        if (endRowIndex != sheet.LastRowNum)
        {
            IRow endRow = sheet.GetRow(endRowIndex);
            if (endRow != null) sheet.RemoveRow(endRow);
            WriteEndRow(sheet);
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

    // =========================
    // Test trigger (press K)
    // =========================
    private void Update()
    {
        if (!initialized) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("K pressed -> writing test record (close Excel file first!)");

            if (taskNames.Length > 0) MarkTaskDone(0);
            if (taskNames.Length > 1) MarkTaskDone(1);
            if (taskNames.Length > 2) MarkTaskDone(2);

            EndTestAndWriteRecord();
        }
    }
}

