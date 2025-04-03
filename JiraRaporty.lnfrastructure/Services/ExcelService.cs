using JiraRaporty.Domain.Entities;
using JiraRaporty.Domain.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using System.Drawing;

namespace JiraRaporty.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        // Define constant colors
        private const string ColorGreen = "#e2efda";

        private const string ColorYellow = "#fff2cc";
        private const string ColorRed = "#ff0000";

        public MemoryStream GenerateExcelReport(
            DateTime fromDate,
            DateTime toDate,
            List<string> projectList,
            string projects,
            bool inLocal,
            ExcelIssues excelIssues,
            bool highlightReporters = true)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Raport");

                AddTitle(worksheet, fromDate, toDate, projectList, projects);
                const int headerRow = 4;
                AddHeaders(worksheet, headerRow);
                var dataRowStart = headerRow + 1;
                var dataRowEnd = FillData(worksheet, dataRowStart, excelIssues, inLocal, highlightReporters);
                ApplyStyles(worksheet, headerRow, dataRowEnd);

                if (inLocal)
                {
                    AddLocalWorkSummary(worksheet, headerRow, dataRowEnd, excelIssues);
                }
                else
                {
                    AddSimpleSummary(worksheet, headerRow, dataRowEnd);
                }

                package.Save();
            }

            stream.Position = 0;
            return stream;
        }

        private void AddTitle(ExcelWorksheet worksheet, DateTime fromDate, DateTime toDate, List<string> projectList, string projects)
        {
            worksheet.Cells[1, 1].Value = "Załącznik do faktury";
            worksheet.Cells[2, 1].Value = projectList.Count == 1
                ? $"Raport czasu pracy w projekcie {projects} od {fromDate:dd-MM-yyyy} do {toDate:dd-MM-yyyy}"
                : $"Raport czasu pracy w projektach {projects} od {fromDate:dd-MM-yyyy} do {toDate:dd-MM-yyyy}";
        }

        private void AddHeaders(ExcelWorksheet worksheet, int headerRow)
        {
            var headers = new[]
            {
                "Projekt", "Nr zgłoszenia", "Tytuł zgłoszenia", "Nazwa lokalu", "Zlecił",
                "Etykieta", "Start prac", "Prace wykonał", "Czas pracy zalogowany w JIRA",
                "Czas pracy przy realizacji zgłoszenia zgodnie z warunkami umowy",
                "Czas pracy przy realizacji zgłoszenia zgodnie z warunkami umowy",
                "Szczegóły realizacji"
            };

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cells[headerRow, col].Value = headers[col - 1];
            }
        }

        private int FillData(ExcelWorksheet worksheet, int dataRowStart, ExcelIssues excelIssues, bool inLocal, bool highlightReporters)
        {
            var currentRow = dataRowStart;
            var workInLocationKeys = new List<string>();

            foreach (var item in excelIssues.Rows)
            {
                worksheet.Cells[currentRow, 1].Value = item.Project;
                worksheet.Cells[currentRow, 2].Value = item.Key;
                worksheet.Cells[currentRow, 3].Value = item.Summary;
                worksheet.Cells[currentRow, 4].Value = item.EpicName;
                worksheet.Cells[currentRow, 5].Value = item.Reporter;
                worksheet.Cells[currentRow, 6].Value = item.Labels;
                worksheet.Cells[currentRow, 7].Value = item.Started;
                worksheet.Cells[currentRow, 8].Value = item.WorklogAuthor;
                worksheet.Cells[currentRow, 9].Value = item.TimeSpentSeconds / 3600.0;
                worksheet.Cells[currentRow, 10].Formula = $"IF(M{currentRow}=0,ROUNDUP((I{currentRow})/0.5,0)*0.5,ROUNDUP((I{currentRow}),0))";
                worksheet.Cells[currentRow, 11].Formula = $"TIME(QUOTIENT(J{currentRow},1),(J{currentRow}-QUOTIENT(J{currentRow},1))*60,0)";
                worksheet.Cells[currentRow, 12].Value = item.Comment;

                // Check for reporter highlighting if enabled
                if (highlightReporters && item.Reporter != null)
                {
                    // Check if the reporter name contains square brackets, indicating contractor
                    bool isContractor = item.Reporter.Contains("[") && item.Reporter.Contains("]");

                    // Highlight if it's not a contractor (no square brackets)
                    if (!isContractor)
                    {
                        worksheet.Cells[currentRow, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, 5].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorRed));
                    }
                }

                if (inLocal)
                {
                    worksheet.Cells[currentRow, 13].Formula = $"IF(IFERROR(SEARCH(\"Prac? w Lokalu\",L{currentRow}),0)>0,SEARCH(\"Prac? w Lokalu\",L{currentRow}),0)";
                    if (item.Comment != null && (item.Comment.Contains("Prace w lokalu") || item.Comment.Contains("Praca w lokalu")))
                    {
                        workInLocationKeys.Add(item.Key ?? string.Empty);
                        worksheet.Cells[currentRow, 9, currentRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, 9, currentRow, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorYellow));
                    }
                    else
                    {
                        worksheet.Cells[currentRow, 9, currentRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, 9, currentRow, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorGreen));
                    }
                }
                currentRow++;
            }

            return currentRow - 1;
        }

        private void ApplyStyles(ExcelWorksheet worksheet, int headerRow, int dataRowEnd)
        {
            for (var row = headerRow; row <= dataRowEnd; row++)
            {
                for (var col = 1; col <= 12; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    cell.Style.WrapText = true;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    cell.Style.Font.Color.SetColor(Color.Black);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    cell.Style.Font.Size = 10;
                }
            }

            worksheet.Cells[headerRow, 1, headerRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[headerRow, 1, headerRow, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[headerRow + 1, 11, dataRowEnd, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[$"B{headerRow + 1}:B{dataRowEnd}"].Style.Font.Size = 11;

            var columnWidths = new[] { 13, 11.3, 19.3, 17.6, 22.6, 14.1, 17.6, 17.6, 16.4, 16, 16.4, 78 };
            for (int col = 1; col <= columnWidths.Length; col++)
            {
                worksheet.Column(col).Width = columnWidths[col - 1];
            }

            worksheet.Cells[$"G{headerRow + 1}:G{dataRowEnd}"].Style.Numberformat.Format = "dd.mm.yyyy hh:mm";
            worksheet.Cells[$"K{headerRow + 1}:K{dataRowEnd}"].Style.Numberformat.Format = "hh:mm";
        }

        private void AddSimpleSummary(ExcelWorksheet worksheet, int headerRow, int dataRowEnd)
        {
            worksheet.Cells[dataRowEnd + 2, 8].Value = "SUMA długości czasu pracy:";
            worksheet.Cells[dataRowEnd + 2, 8].Style.Font.Bold = true;
            worksheet.Cells[dataRowEnd + 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[dataRowEnd + 2, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[dataRowEnd + 2, 9].Formula = $"SUM(I{headerRow + 1}:I{dataRowEnd})";

            worksheet.Column(10).Hidden = true;
            worksheet.Column(11).Hidden = true;

            worksheet.Cells[$"K{dataRowEnd + 2}:K{dataRowEnd + 4}"].Style.Numberformat.Format = "[hh]:mm";
            worksheet.Cells[headerRow + 1, 9, dataRowEnd, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private void AddLocalWorkSummary(ExcelWorksheet worksheet, int headerRow, int dataRowEnd, ExcelIssues excelIssues)
        {
            var workInLocationKeys = string.Join(", ", excelIssues.Rows
                .Where(r => r.Comment != null && (r.Comment.Contains("Prace w lokalu") || r.Comment.Contains("Praca w lokalu")))
                .Select(r => r.Key)
                .Where(k => k != null)
                .Distinct());

            worksheet.Cells[dataRowEnd + 2, 7].Value = "RAZEM długość czasu pracy:";
            worksheet.Cells[dataRowEnd + 2, 7].Style.Font.Bold = true;

            worksheet.Cells[dataRowEnd + 3, 7].Value = "w tym:";
            worksheet.Cells[dataRowEnd + 3, 8].Value = "prace zdalne";
            worksheet.Cells[dataRowEnd + 4, 8].Value = "prace w lokalu";

            worksheet.Cells[dataRowEnd + 2, 10].Formula = $"SUM(J{headerRow + 1}:J{dataRowEnd})";
            worksheet.Cells[dataRowEnd + 3, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[dataRowEnd + 3, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorGreen));
            worksheet.Cells[dataRowEnd + 3, 10].Formula = $"J{dataRowEnd + 2}-J{dataRowEnd + 4}";
            worksheet.Cells[dataRowEnd + 4, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[dataRowEnd + 4, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorYellow));
            worksheet.Cells[dataRowEnd + 4, 10].Formula = $"SUMIF(M{headerRow + 1}:M{dataRowEnd},\">0\",J{headerRow + 1}:J{dataRowEnd})";

            worksheet.Cells[dataRowEnd + 2, 11].Formula = $"SUM(K{headerRow + 1}:K{dataRowEnd})";
            worksheet.Cells[dataRowEnd + 3, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[dataRowEnd + 3, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorGreen));
            worksheet.Cells[dataRowEnd + 3, 11].Formula = $"K{dataRowEnd + 2}-K{dataRowEnd + 4}";
            worksheet.Cells[dataRowEnd + 4, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[dataRowEnd + 4, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(ColorYellow));
            worksheet.Cells[dataRowEnd + 4, 11].Formula = $"SUMIF(M{headerRow + 1}:M{dataRowEnd},\">0\",K{headerRow + 1}:K{dataRowEnd})";

            worksheet.Cells[dataRowEnd + 6, 3].Value = "Do rozliczenia zgodnie z warunkami umowy:";
            worksheet.Cells[dataRowEnd + 6, 3].Style.Font.Bold = true;
            worksheet.Cells[dataRowEnd + 6, 3].Style.Font.UnderLine = true;

            worksheet.Cells[dataRowEnd + 7, 2].Value = "1";
            worksheet.Cells[dataRowEnd + 7, 3].Value = "20 godzin - wg. stawki ryczałtowej: 5 400 PLN netto";
            worksheet.Cells[dataRowEnd + 7, 3, dataRowEnd + 7, 10].Merge = true;
            worksheet.Cells[dataRowEnd + 7, 11].Value = 5400;
            worksheet.Cells[dataRowEnd + 7, 12].Value = "PLN netto";

            worksheet.Cells[dataRowEnd + 8, 2].Value = "2";
            worksheet.Cells[dataRowEnd + 8, 3].Formula = $"\"dodatkowe \" & IF(J{dataRowEnd + 3}>20,(J{dataRowEnd + 2}-20-J{dataRowEnd + 4}),0) & \" godz. - wg. stawki: 95 PLN netto/0,5 godz.\"";
            worksheet.Cells[dataRowEnd + 8, 3, dataRowEnd + 8, 10].Merge = true;
            worksheet.Cells[dataRowEnd + 8, 11].Formula = $"IF(J{dataRowEnd + 3}>20,(J{dataRowEnd + 2}-20-J{dataRowEnd + 4})*190,0)";
            worksheet.Cells[dataRowEnd + 8, 12].Value = "PLN netto";

            worksheet.Cells[dataRowEnd + 9, 2].Value = "3";
            worksheet.Cells[dataRowEnd + 9, 3].Formula = $"\"Praca serwisanta w miejscu instalacji urządzeń \" & J{dataRowEnd + 4} & \" godz. wg. stawki 240 PLN netto za rozpoczętą godzinę - zgłoszenia nr \" & M{dataRowEnd + 9} & \".\"";
            worksheet.Cells[dataRowEnd + 9, 3, dataRowEnd + 9, 10].Merge = true;
            worksheet.Cells[dataRowEnd + 9, 3, dataRowEnd + 9, 10].Style.WrapText = true;
            worksheet.Cells[dataRowEnd + 9, 11].Formula = $"ROUNDUP(J{dataRowEnd + 4},0)*240";
            worksheet.Cells[dataRowEnd + 9, 12].Value = "PLN netto";
            worksheet.Cells[dataRowEnd + 9, 13].Value = workInLocationKeys;

            worksheet.Cells[dataRowEnd + 10, 2].Value = "4";
            worksheet.Cells[dataRowEnd + 10, 2, dataRowEnd + 11, 2].Merge = true;
            worksheet.Cells[dataRowEnd + 10, 3].Value = "Zamówione i dostarczone materiały/urządzenia/usługi:";
            worksheet.Cells[dataRowEnd + 10, 3, dataRowEnd + 10, 10].Merge = true;
            worksheet.Cells[dataRowEnd + 11, 3, dataRowEnd + 11, 10].Merge = true;
            worksheet.Cells[dataRowEnd + 10, 3, dataRowEnd + 11, 10].Style.WrapText = true;
            worksheet.Cells[dataRowEnd + 10, 11].Value = "";
            worksheet.Cells[dataRowEnd + 10, 11, dataRowEnd + 11, 11].Merge = true;
            worksheet.Cells[dataRowEnd + 10, 12].Value = "PLN netto";
            worksheet.Cells[dataRowEnd + 10, 12, dataRowEnd + 11, 12].Merge = true;

            worksheet.Cells[dataRowEnd + 13, 8].Value = "SUMA";
            worksheet.Cells[dataRowEnd + 13, 8].Style.Font.Bold = true;
            worksheet.Cells[dataRowEnd + 13, 11].Formula = $"SUM(K{dataRowEnd + 7}:K{dataRowEnd + 11})";
            worksheet.Cells[dataRowEnd + 13, 11].Style.Font.Bold = true;
            worksheet.Cells[dataRowEnd + 13, 11].Style.Font.UnderLine = true;
            worksheet.Cells[dataRowEnd + 13, 12].Value = "PLN netto";

            worksheet.Cells[dataRowEnd + 2, 11, dataRowEnd + 4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            worksheet.Cells[dataRowEnd + 2, 7, dataRowEnd + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[dataRowEnd + 2, 7, dataRowEnd + 3, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

            worksheet.Cells[dataRowEnd + 2, 8, dataRowEnd + 4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[dataRowEnd + 2, 8, dataRowEnd + 4, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

            worksheet.Cells[dataRowEnd + 2, 10, dataRowEnd + 4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[dataRowEnd + 2, 10, dataRowEnd + 4, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[dataRowEnd + 7, 2, dataRowEnd + 11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[dataRowEnd + 7, 2, dataRowEnd + 11, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[dataRowEnd + 7, 3, dataRowEnd + 11, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[dataRowEnd + 7, 3, dataRowEnd + 11, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[dataRowEnd + 7, 11, dataRowEnd + 11, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[dataRowEnd + 7, 11, dataRowEnd + 11, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[dataRowEnd + 7, 12, dataRowEnd + 11, 12].Style.Font.Bold = true;
            worksheet.Cells[dataRowEnd + 7, 12, dataRowEnd + 11, 12].Style.Font.Size = 9;
            worksheet.Cells[dataRowEnd + 7, 12, dataRowEnd + 11, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[dataRowEnd + 7, 12, dataRowEnd + 11, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Row(dataRowEnd + 7).Height = 15;
            worksheet.Row(dataRowEnd + 8).Height = 15;
            worksheet.Row(dataRowEnd + 9).Height = 35;
            worksheet.Row(dataRowEnd + 10).Height = 400;
            worksheet.Row(dataRowEnd + 11).Height = 15;

            worksheet.Column(9).Hidden = true;
            worksheet.Column(10).Hidden = true;
            worksheet.Column(13).Hidden = true;

            worksheet.Cells[$"K{dataRowEnd + 2}:K{dataRowEnd + 4}"].Style.Numberformat.Format = "[hh]:mm";
            worksheet.Cells[$"K{dataRowEnd + 7}:K{dataRowEnd + 13}"].Style.Numberformat.Format = "# ##0.00;-# ##0.00";
        }
    }
}