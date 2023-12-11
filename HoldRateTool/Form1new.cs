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
            // 獲取輸入文件的目錄
            string inputDirectory = Path.GetDirectoryName(ft1FilePath);
            var ft1UHSHData = ReadData(ft1FilePath, 20, 2, 1, 4);
            var sigmaUHSHData = ReadData(sblFilePath, 5, 2, 1, 3);
            var mean


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
                        weeklyReportWriter.WriteLine("Week\tTester NO\tInput\tSBLHold\tHoldRate");
                        isFirstWeek = false;
                    }

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
                        weeklyReportWriter.WriteLine($"{dateRange}\t{machine}\t{totalCount}\t{sigmaCount}\t{holdRate:P}");
                        //weeklyReportWriter.WriteLine($"{machine}\t{totalCount}\t{sigmaCount}\t{holdRate:P}");odle
                    }
                }

                currentDate = currentDate.AddDays(7); // 前進到下一週
            }

            Console.WriteLine("結果已寫入文件。");

        }

        private Dictionary<string, List<(string LotNo, string PartNo, string Date)>> ReadData(string filePath, int uhshIndex, int lotNoIndex, int partNoIndex, int dateIndex)
        {
            var data = new Dictionary<string, List<(string LotNo, string PartNo, string Date)>>();
            var processedLotNos = new HashSet<string>();

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('\t');
                    if (parts.Length > uhshIndex && parts[uhshIndex].StartsWith("UHSH"))
                    {
                        var lotNo = parts[lotNoIndex];
                        var partNo = parts[partNoIndex];
                        var uhsh = parts[uhshIndex];
                        var date = parts[dateIndex];

                        if (!processedLotNos.Contains(lotNo))
                        {
                            if (!data.ContainsKey(uhsh))
                            {
                                data[uhsh] = new List<(string LotNo, string PartNo, string Date)>();
                            }
                            data[uhsh].Add((lotNo, partNo, date));
                            processedLotNos.Add(lotNo);
                        }
                    }
                }
            }
            return data;
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

            // 使用switch語句根據所選選項設置 ft1FilePath


            switch (selectedProduct)
            {
                case "NPTSegma":
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\NPT_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\XanHoldRateRport\Weekly_Hold_Rate_Report_NPT{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    sblFilePath = @"\\twtsa-share01\OP\QA_Record\FT_SBLDown_disposition\Sigma_site_socket_record.txt";
                    break;
                case "XANSegma":
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\XAN_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\XanHoldRateRport\Weekly_Hold_Rate_Report_XAN{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    sblFilePath = @"\\twtsa-share01\OP\QA_Record\FT_SBLDown_disposition\Sigma_site_socket_record.txt";
                    break;
                case "YOKSegma":
                    ft1FilePath = @"\\twtsa-share01\OP\UHSH_DATA\YOK_FT_DISPOSITION\FT1-QFN_BASE-Dispositon_Record.txt";
                    reportFilePath = $@"M:\TE\Bin Chuang\XanHoldRateRport\Weekly_Hold_Rate_Report_YOK{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    sblFilePath = @"\\twtsa-share01\OP\QA_Record\FT_SBLDown_disposition\Sigma_site_socket_record.txt";
                    break;
                default:
                    // 如果沒有匹配的選項，默認XAN路徑。
                    break;
            }

        }
    }
}
