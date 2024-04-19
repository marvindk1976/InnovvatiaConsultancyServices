using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace WebApi5Paisa
{
    public class Logger
    {
        public static void LogWrite(string logs)
        {
            var currentDate = DateTime.Now;
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentDate.Month);
            //var root = AppDomain.CurrentDomain.BaseDirectory + "\\logs";

            string root = Path.Combine(Directory.GetCurrentDirectory(), @"logs");
            var yearPath = root + "\\" + currentDate.Year.ToString() + "\\";
            var MonthPath = yearPath + currentDate.Year + "-" + monthName + "\\";
            var errorFile = MonthPath + "ErrorLogs-" + String.Format("{0:d-M-yyyy}", currentDate.Date) + ".txt";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            if (!Directory.Exists(yearPath))
            {
                Directory.CreateDirectory(yearPath);
            }
            if (!Directory.Exists(MonthPath))
            {
                Directory.CreateDirectory(MonthPath);
            }
            if (!File.Exists(errorFile))
            {
                FileStream fs = File.Create(errorFile);
                fs.Close();
            }
            var oPErrorLog = new StringBuilder();
            oPErrorLog.Append(" ------------------ Application Log Started! " + currentDate.ToString() + " ------------------");
            oPErrorLog.AppendLine("");
            oPErrorLog.Append(logs);
            oPErrorLog.AppendLine("");
            oPErrorLog.AppendLine("--------------------------------* Ended *------------------------------------------");
            using (StreamWriter writer = File.AppendText(errorFile))
            {
                writer.WriteAsync(oPErrorLog.ToString());
            }
        }
    }
}
