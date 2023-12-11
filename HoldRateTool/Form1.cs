using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace HoldRateTool
{

    public partial class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private string ft1FilePath = "";
        private string sblFilePath = "";
        private string reportFilePath = "";
        public Form1()
        {
            InitializeComponent();
            // Initialize other necessary components
            // Initialize DateTimePicker

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            // Call the processing method
            ProcessData();
        }

        private void ProcessData()
        {
            Dictionary<string, List<(string LotNo, string PartNo, string Date)>> ft1UHSHData = new Dictionary<string, List<(string LotNo, string PartNo, string Date)>>();
            Dictionary<string, List<(string LotNo, string PartNo, string Date)>> sigmaUHSHData = new Dictionary<string, List<(string LotNo, string PartNo, string Date)>>();

            // 讀取FT1-QFN_BASE-Dispositon_Record

            using (StreamReader reader = new StreamReader(ft1FilePath))

            {
                string line;
                HashSet<string> processedLotNos = new HashSet<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length > 20 && parts[20].StartsWith("UHSH"))
                    {
                        string lotNo = parts[2];
                        string partNo = parts[1];
                        string uhsh = parts[20];
                        string date = parts[4]; // 提取日期資訊

                        if (!processedLotNos.Contains(lotNo))//避免重複lot
                        {
                            if (!ft1UHSHData.ContainsKey(uhsh))
                            {
                                ft1UHSHData[uhsh] = new List<(string LotNo, string PartNo, string Date)>();
                            }
                            ft1UHSHData[uhsh].Add((lotNo, partNo, date)); // 包含日期資訊
                            processedLotNos.Add(lotNo);
                        }
                    }
                }
            }

            // 讀取Sigma_site_socket_record
            string sigmaFilePath = sblFilePath;
            using (var reader = new StreamReader(sigmaFilePath))
            {
                reader.ReadLine();
                string line;
                HashSet<string> processedLotNos = new HashSet<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length > 5 )
                    {
                        string lotNo = parts[2];
                        string partNo = parts[1];
                        string date = parts[3]; // 提取日期資訊

                        if (!processedLotNos.Contains(lotNo))//避免重複lot
                        {
                            if (!sigmaUHSHData.ContainsKey(lotNo))
                            {
                                sigmaUHSHData[lotNo] = new List<(string LotNo, string PartNo, string Date)>();
                            }
                            sigmaUHSHData[lotNo].Add((lotNo, partNo, date)); // 包含日期資訊
                            processedLotNos.Add(lotNo);
                        }
                    }
                }
            }
            // 獲取輸入文件的目錄
            string inputDirectory = Path.GetDirectoryName(ft1FilePath);

            // 比對批次號
            List<string> ft1LotNos = ft1UHSHData.Values.SelectMany(lst => lst.Select(item => item.LotNo)).Distinct().ToList();
            List<string> sigmaLotNos = sigmaUHSHData.Values.SelectMany(lst => lst.Select(item => item.LotNo)).Distinct().ToList();

            // 獲取最早的日期和最晚的日期以確定日期範圍
            List<DateTime> allDates = ft1UHSHData.Values.SelectMany(lst => lst.Select(item => DateTime.Parse(item.Date)))
                .Concat(sigmaUHSHData.Values.SelectMany(lst => lst.Select(item => DateTime.Parse(item.Date)))).ToList();
            DateTime startDate = startDatePicker.Value.Date;
            DateTime endDate = allDates.Max();

            int totalWeeks = (int)((endDate - startDate).TotalDays / 7) + 1;
            int processedWeeks = 0;

            DateTime currentDate = startDate;
            bool isFirstWeek = true;
            while (currentDate <= endDate)
            {
                // 計算當前週的開始日期和結束日期
                DateTime currentWeekStart = currentDate.Date;
                DateTime currentWeekEnd = currentDate.Date.AddDays(6);

                // 在進入下一週之前，可以添加一個進度資訊
                processedWeeks++;
                double progress = (double)processedWeeks / totalWeeks * 100;
                UpdateOutput($"Process week{processedWeeks} : {progress:F2}% done");

                using (StreamWriter weeklyReportWriter = new StreamWriter(reportFilePath, true)) // 追加寫入
                {
                    if (isFirstWeek)
                    {
                        weeklyReportWriter.WriteLine("Week\tTester NO\tInput\tSBLFail\tFailRate");
                        isFirstWeek = false;
                    }
                    //weeklyReportWriter.WriteLine($"{currentWeekStart:yyyy/MM/dd}-{currentWeekEnd:yyyy/MM/dd}");
                    //weeklyReportWriter.WriteLine($"Tester NO\tInput\tSBLHold\tHoldRate");

                    List<(string Machine, int SigmaCount, int TotalCount, double HoldRate)> weeklyHoldRateData = new List<(string Machine, int SigmaCount, int TotalCount, double HoldRate)>();

                    foreach (var kvp in ft1UHSHData)
                    {

                        int count = kvp.Value.Count;
                        int sigmaCount = kvp.Value.Count(item => sigmaLotNos.Contains(item.LotNo));

                        // 過濾出當前週的記錄
                        List<(string LotNo, string PartNo, string Date)> currentWeekRecords = kvp.Value.Where(item => DateTime.Parse(item.Date) >= currentWeekStart && DateTime.Parse(item.Date) <= currentWeekEnd).ToList();
                        int currentWeekCount = currentWeekRecords.Count;
                        int currentWeekSigmaCount = currentWeekRecords.Count(item => sigmaLotNos.Contains(item.LotNo));

                        // 只顯示當週有生產的機台
                        if (currentWeekRecords.Count > 0)
                        {
                            double holdRate = currentWeekCount > 0 ? (double)currentWeekSigmaCount / currentWeekCount : 0.0;
                            weeklyHoldRateData.Add((kvp.Key, currentWeekSigmaCount, currentWeekCount, holdRate));
                        }
                    }

                    // 按 Hold Rate 排序
                    weeklyHoldRateData = weeklyHoldRateData.OrderByDescending(data => data.HoldRate).ToList();
                    foreach ((string machine, int sigmaCount, int totalCount, double holdRate) in weeklyHoldRateData)
                    {
                        string dateRange = $"{currentWeekStart:yyyy/MM/dd}-{currentWeekEnd:yyyy/MM/dd}";

                        // 收集该机器在当前周的sigmaLotNos
                        var relevantSigmaLotNos = ft1UHSHData[machine]
                                                    .Where(item => sigmaLotNos.Contains(item.LotNo) && DateTime.Parse(item.Date) >= currentWeekStart && DateTime.Parse(item.Date) <= currentWeekEnd)
                                                    .Select(item => item.LotNo)
                                                    .Distinct()
                                                    .ToList();

                        // 收集该机器在当前周的所有LotNos
                        var allCurrentWeekLotNos = ft1UHSHData[machine]
                                                    .Where(item => DateTime.Parse(item.Date) >= currentWeekStart && DateTime.Parse(item.Date) <= currentWeekEnd)
                                                    .Select(item => item.LotNo)
                                                    .Distinct()
                                                    .ToList();

                        // 将LotNo列表转换为使用制表符分隔的字符串
                        string sigmaLotNosString = string.Join("\t", relevantSigmaLotNos);
                        string allLotNosString = string.Join("\t", allCurrentWeekLotNos);

                        weeklyReportWriter.WriteLine($"{dateRange}\t{machine}\t{totalCount}\t{sigmaCount}\t{holdRate:P}\t{allLotNosString}\tTrigger->\t{sigmaLotNosString}");
                    }

                }

                currentDate = currentDate.AddDays(7); // 前進到下一週
            }

            UpdateOutput("done!");

        }
        private void UpdateOutput(string message)
        {
            if (this.richTextBoxOutput.InvokeRequired)
            {
                this.richTextBoxOutput.Invoke(new Action(() => richTextBoxOutput.AppendText(message + Environment.NewLine)));
            }
            else
            {
                richTextBoxOutput.AppendText(message + Environment.NewLine);
            }
        }


        private void ProductCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedProduct = ProductCombo.SelectedItem?.ToString();
            UpdateFilePaths(selectedProduct);
         
        }

        private void UpdateFilePaths(string selectedProduct)
        {
            string reportSuffix = meanRadioButton.Checked ? "Mean" : "Segma";

            switch (selectedProduct)
            {
                case "NPT":
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\NPT_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\HoldRateRport\Weekly_Hold_Rate_Report_NPT{reportSuffix}{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    break;
                case "XAN":
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\XAN_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\HoldRateRport\Weekly_Hold_Rate_Report_XAN{reportSuffix}{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    break;
                case "YOK":
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\YOK_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\HoldRateRport\Weekly_Hold_Rate_Report_YOK{reportSuffix}{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    break;
                default:
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\XAN_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\HoldRateRport\Weekly_Hold_Rate_Report_XAN{reportSuffix}{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    break;
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (meanRadioButton.Checked)
            {
                sblFilePath = @"\\twtsa-share01\OP\QA_Record\FT_SBLDown_disposition\Mean_Shift_site_socket_record_new.txt";
            }
            else if (segmaRadioButton.Checked)
            {
                sblFilePath = @"\\twtsa-share01\OP\QA_Record\FT_SBLDown_disposition\Sigma_site_socket_record.txt";
            }

            string selectedProduct = ProductCombo.SelectedItem?.ToString();
            UpdateFilePaths(selectedProduct);
        }

    }
}
