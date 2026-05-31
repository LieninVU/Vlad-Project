using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ForVlad.Services
{
    public static class CsvExportService
    {
        public static void Export(string filePath, string[] headers, IEnumerable<string[]> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", headers.Select(Escape)));
            foreach (var row in rows)
                sb.AppendLine(string.Join(";", row.Select(Escape)));
            
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            
            if (value.Contains(";") || value.Contains("\"") || value.Contains("\n"))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            
            return value;
        }
    }
}
