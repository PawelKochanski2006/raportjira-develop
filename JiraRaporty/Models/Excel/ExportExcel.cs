using JiraRaporty.Models.API;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace JiraRaporty.Models.Excel
{
    public class ExportExcel
    {
        public ExportExcel(DateTime fromDate, DateTime toDate, List<string> projectList, string projects, bool inLocal, ExcelIssues excelIssues, MemoryStream stream)
        {
            // Kolory do kolorowania pól
            var colorGreen = "#e2efda";
            var colorYellow = "#fff2cc";

            // Tworzymy plik excel i go uzupełniamy 
            using (var package = new ExcelPackage(stream))
            {
                var worksheetName = "Raport";
                // Dodawanie arkusza do pliku Excel
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);

                

                // Zaczynamy uzupełniać arkusz
                worksheet.Cells[1, 1].Value = $"Załącznik do faktury";
                // W zależności od ilości wybranych projektów inaczej zaczynamy plik  
                if (projectList.Count == 1)
                {
                    worksheet.Cells[2, 1].Value = $"Raport czasu pracy w projekcie {projects} od {fromDate.ToString("dd-MM-yyyy")} do {toDate.ToString("dd-MM-yyyy")}";
                }
                else
                {
                    worksheet.Cells[2, 1].Value = $"Raport czasu pracy w projektach {projects} od {fromDate.ToString("dd-MM-yyyy")} do {toDate.ToString("dd-MM-yyyy")}";
                }

                var headerRow = 4;
                // Nagłówki kolumn
                worksheet.Cells[headerRow, 1].Value = "Projekt";
                worksheet.Cells[headerRow, 2].Value = "Nr zgłoszenia";
                worksheet.Cells[headerRow, 3].Value = "Tytuł zgłoszenia";
                worksheet.Cells[headerRow, 4].Value = "Nazwa lokalu";
                worksheet.Cells[headerRow, 5].Value = "Zlecił";
                worksheet.Cells[headerRow, 6].Value = "Etykieta";
                worksheet.Cells[headerRow, 7].Value = "Start prac";
                worksheet.Cells[headerRow, 8].Value = "Prace wykonał";
                worksheet.Cells[headerRow, 9].Value = "Czas pracy zalogowany w JIRA";
                worksheet.Cells[headerRow, 10].Value = "Czas pracy przy realizacji zgłoszenia zgodnie z warunkami umowy";
                worksheet.Cells[headerRow, 11].Value = "Czas pracy przy realizacji zgłoszenia zgodnie z warunkami umowy";
                worksheet.Cells[headerRow, 12].Value = "Szczegóły realizacji";


                // Wypełnianie danymi
                var bodyRow = headerRow + 1; // Numer wiersza, z którego zaczynamy wpisywać dane
                List<string> WorkInLocationKeyList = new List<string> { }; // Zmienna do której zapisujemy wszystkie key gdzie była praca w lokalu aby wypisać key w 3 pkt. podsumowania 
                foreach (var item in excelIssues.rows)
                {
                    worksheet.Cells[bodyRow, 1].Value = item.project;
                    worksheet.Cells[bodyRow, 2].Value = item.key;
                    worksheet.Cells[bodyRow, 3].Value = item.summery;
                    worksheet.Cells[bodyRow, 4].Value = item.epicName;
                    worksheet.Cells[bodyRow, 5].Value = item.reporter;
                    worksheet.Cells[bodyRow, 6].Value = item.labels;
                    worksheet.Cells[bodyRow, 7].Value = item.started;
                    worksheet.Cells[bodyRow, 8].Value = item.worklogAutor;
                    worksheet.Cells[bodyRow, 9].Value = item.timeSpentSeconds / 3600.0;
                    worksheet.Cells[bodyRow, 10].Formula = $"IF(M{bodyRow}=0,ROUNDUP((I{bodyRow})/0.5,0)*0.5,ROUNDUP((I{bodyRow}),0))";
                    worksheet.Cells[bodyRow, 11].Formula = $"TIME(QUOTIENT(J{bodyRow},1),(J{bodyRow}-QUOTIENT(J{bodyRow},1))*60,0)";
                    worksheet.Cells[bodyRow, 12].Value = item.comment;
                    if (inLocal)
                    {
                        worksheet.Cells[bodyRow, 13].Formula = $"IF(IFERROR(SEARCH(\"Prac? w Lokalu\",L{bodyRow}),0)>0,SEARCH(\"Prac? w Lokalu\",L{bodyRow}),0)";
                        //  Szukamy czy w comment nie znajduje się Praca w terenie
                        if (item.comment.IndexOf("Prace w lokalu") >= 0 || item.comment.IndexOf("Praca w lokalu") >= 0)
                        {
                            WorkInLocationKeyList.Add(item.key);
                            // Ustawianie koloru tła komórek I,J,K
                            worksheet.Cells[bodyRow, 9, bodyRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[bodyRow, 9, bodyRow, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorYellow));
                        }
                        else
                        {
                            // Ustawianie koloru tła komórek I,J,K
                            worksheet.Cells[bodyRow, 9, bodyRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[bodyRow, 9, bodyRow, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorGreen));
                        }
                    }

                    // Idziemy do kolejnego wiersza
                    bodyRow++;
                }
                // Eliminujemy przyrost wiersza 
                --bodyRow;

                // Filtrujemy listę key w których jest praca w lokalu żeby nie było powtarzalnych key i wrzucamy keys do ciągu znaków po przecinku  
                var WorkInLocationKey = string.Join(", ", WorkInLocationKeyList.Distinct().ToList());

                // Style ogólne 
                styleForBodyAndHeader(worksheet, headerRow, bodyRow);

                if (inLocal)
                {
                    // Podsumowanie z wyodrębnieniem godzin pracy w lokalu
                    summaryOfTheWorkOnTheLocal(worksheet, headerRow, bodyRow, colorGreen, colorYellow, WorkInLocationKey);
                }
                else
                {
                    // Proste podsumowanie
                    summary(worksheet, headerRow, bodyRow);
                }

                // Tworzymy macro, które sortuje dane
                //macroSort(package, headerRow, bodyRow, worksheetName);

                // Save excel file
                package.Save();
            }
            stream.Position = 0;
        }

        private void styleForBodyAndHeader(ExcelWorksheet worksheet, int headerRow, int bodyRow)
        {
            // Style for body and header
            for (var row = headerRow; row <= bodyRow; row++)
            {
                for (var col = 1; col <= 12; col++)
                {
                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells[row, col].Style.Font.Size = 10;
                }
            }
            // Ustawienia wyrównania dla nagłówka i dla K
            worksheet.Cells[headerRow, 1, headerRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[headerRow, 1, headerRow, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[headerRow + 1, 11, bodyRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            

            // Dla kolumny B zmieniamy wielkość czcionki
            worksheet.Cells[$"B{headerRow + 1}:B{bodyRow}"].Style.Font.Size = 11;

            // Wyłączamy zawijanie tekstu dla podanych kolumnach w cells[A, B, G, I, J, K] bez nagłówków
            //worksheet.Cells[$"A{headerRow + 1}:A1000 ,B{headerRow + 1}:B1000, G{headerRow + 1}:G1000, I{headerRow + 1}:I1000,J{headerRow + 1}:J1000,K{headerRow + 1}:K1000"].Style.WrapText = false;

            // Ustawianie szerokości dla poszczególnych kolumn
            worksheet.Column(1).Width = 13;
            worksheet.Column(2).Width = 11.3;
            worksheet.Column(3).Width = 19.3;
            worksheet.Column(4).Width = 17.6;
            worksheet.Column(5).Width = 22.6;
            worksheet.Column(6).Width = 14.1;
            worksheet.Column(7).Width = 17.6;
            worksheet.Column(8).Width = 17.6;
            worksheet.Column(9).Width = 16.4;
            worksheet.Column(10).Width = 16;
            worksheet.Column(11).Width = 16.4;
            worksheet.Column(12).Width = 78;



            // Ustawianie formatowania 
            worksheet.Cells[$"G{headerRow + 1}:G{bodyRow}"].Style.Numberformat.Format = "dd.mm.yyyy hh:mm";
            worksheet.Cells[$"K{headerRow + 1}:K{bodyRow}"].Style.Numberformat.Format = "hh:mm";
        }

        private void summary(ExcelWorksheet worksheet, int headerRow, int bodyRow)
        {
            // Podsumowanie
            worksheet.Cells[bodyRow + 2, 8].Value = "SUMA długości czasu pracy:";
            worksheet.Cells[bodyRow + 2, 8].Style.Font.Bold = true;
            worksheet.Cells[bodyRow + 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[bodyRow + 2, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            // Sumujemy wszystkie godziny jakie zostały przepracowane 
            worksheet.Cells[bodyRow + 2, 9].Formula = $"SUM(I{headerRow + 1}:I{bodyRow})";

            // chowamy kolumny I,J  
            worksheet.Column(10).Hidden = true;
            worksheet.Column(11).Hidden = true;

            // Ustawianie formatowania 
            worksheet.Cells[$"K{bodyRow + 2}:K{bodyRow + 4}"].Style.Numberformat.Format = "[hh]:mm";

            worksheet.Cells[headerRow + 1, 9, bodyRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private void summaryOfTheWorkOnTheLocal(ExcelWorksheet worksheet, int headerRow, int bodyRow, string colorGreen, string colorYellow, string WorkInLocationKey)
        {
            // Podsumowanie
            worksheet.Cells[bodyRow + 2, 7].Value = "RAZEM długość czasu pracy:";
            worksheet.Cells[bodyRow + 2, 7].Style.Font.Bold = true;

            worksheet.Cells[bodyRow + 3, 7].Value = "w tym:";

            worksheet.Cells[bodyRow + 3, 8].Value = "prace zdalne";
            worksheet.Cells[bodyRow + 4, 8].Value = "prace w lokalu";


            // Sumujemy wszystkie godziny jakie zostały przepracowane 
            worksheet.Cells[bodyRow + 2, 10].Formula = $"SUM(J{headerRow + 1}:J{bodyRow})";

            // Kolorujemy pole i obliczamy sumę godzin pracy zdalnej  
            worksheet.Cells[bodyRow + 3, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[bodyRow + 3, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorGreen));
            worksheet.Cells[bodyRow + 3, 10].Formula = $"J{bodyRow + 2}-J{bodyRow + 4}";

            // Kolorujemy pole i obliczamy sumę godzin pracy w lokalu
            worksheet.Cells[bodyRow + 4, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[bodyRow + 4, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorYellow));
            worksheet.Cells[bodyRow + 4, 10].Formula = $"SUMIF(M{headerRow + 1}:M{bodyRow},\">0\",J{headerRow + 1}:J{bodyRow})";


            // Sumujemy wszystkie godziny jakie zostały przepracowane 
            worksheet.Cells[bodyRow + 2, 11].Formula = $"SUM(K{headerRow + 1}:K{bodyRow})";

            // Kolorujemy pole i obliczamy sumę godzin pracy zdalnej  
            worksheet.Cells[bodyRow + 3, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[bodyRow + 3, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorGreen));
            worksheet.Cells[bodyRow + 3, 11].Formula = $"K{bodyRow + 2}-K{bodyRow + 4}";

            // Kolorujemy pole i obliczamy sumę godzin pracy w lokalu
            worksheet.Cells[bodyRow + 4, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[bodyRow + 4, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorYellow));
            worksheet.Cells[bodyRow + 4, 11].Formula = $"SUMIF(M{headerRow + 1}:M{bodyRow},\">0\",K{headerRow + 1}:K{bodyRow})";


            worksheet.Cells[bodyRow + 6, 3].Value = "Do rozliczenia zgodnie z warunkami umowy:";
            worksheet.Cells[bodyRow + 6, 3].Style.Font.Bold = true;
            worksheet.Cells[bodyRow + 6, 3].Style.Font.UnderLine = true;

            // 1 pkt. podsumowania
            worksheet.Cells[bodyRow + 7, 2].Value = "1";
            worksheet.Cells[bodyRow + 7, 3].Value = "20 godzin - wg. stawki ryczałtowej: 5 400 PLN netto";
            worksheet.Cells[bodyRow + 7, 3, bodyRow + 7, 10].Merge = true;
            worksheet.Cells[bodyRow + 7, 11].Value = 5400;
            worksheet.Cells[bodyRow + 7, 12].Value = "PLN netto";

            // 2 pkt. podsumowania
            worksheet.Cells[bodyRow + 8, 2].Value = "2";
            worksheet.Cells[bodyRow + 8, 3].Formula = $"\"dodatkowe \" & IF(J{bodyRow + 3}>20,(J{bodyRow + 2}-20-J{bodyRow + 4}),0) & \" godz. - wg. stawki: 95 PLN netto/0,5 godz.\"";
            worksheet.Cells[bodyRow + 8, 3, bodyRow + 8, 10].Merge = true;
            worksheet.Cells[bodyRow + 8, 11].Formula = $"IF(J{bodyRow + 3}>20,(J{bodyRow + 2}-20-J{bodyRow + 4})*190,0)";
            worksheet.Cells[bodyRow + 8, 12].Value = "PLN netto";

            // 3 pkt. podsumowania
            worksheet.Cells[bodyRow + 9, 2].Value = "3";
            worksheet.Cells[bodyRow + 9, 3].Formula = $"\"Praca serwisanta w miejscu instalacji urządzeń \" & J{bodyRow + 4} & \" godz. wg. stawki 240 PLN netto za rozpoczętą godzinę - zgłoszenia nr \" & M{bodyRow + 9} & \".\"";
            worksheet.Cells[bodyRow + 9, 3, bodyRow + 9, 10].Merge = true;
            worksheet.Cells[bodyRow + 9, 3, bodyRow + 9, 10].Style.WrapText = true;
            worksheet.Cells[bodyRow + 9, 11].Formula = $"ROUNDUP(J{bodyRow + 4},0)*240";
            worksheet.Cells[bodyRow + 9, 12].Value = "PLN netto";

            worksheet.Cells[bodyRow + 9, 13].Value = WorkInLocationKey;

            // 4 pkt. podsumowania
            worksheet.Cells[bodyRow + 10, 2].Value = "4";
            worksheet.Cells[bodyRow + 10, 2, bodyRow + 11, 2].Merge = true;
            worksheet.Cells[bodyRow + 10, 3].Value = "Zamówione i dostarczone materiały/urządzenia/usługi:";
            worksheet.Cells[bodyRow + 10, 3, bodyRow + 10, 10].Merge = true;
            worksheet.Cells[bodyRow + 11, 3, bodyRow + 11, 10].Merge = true;
            worksheet.Cells[bodyRow + 10, 3, bodyRow + 11, 10].Style.WrapText = true;
            worksheet.Cells[bodyRow + 10, 11].Value = "";
            worksheet.Cells[bodyRow + 10, 11, bodyRow + 11, 11].Merge = true;
            worksheet.Cells[bodyRow + 10, 12].Value = "PLN netto";
            worksheet.Cells[bodyRow + 10, 12, bodyRow + 11, 12].Merge = true;

            // Sumowanie w podsumowaniu
            worksheet.Cells[bodyRow + 13, 8].Value = "SUMA";
            worksheet.Cells[bodyRow + 13, 8].Style.Font.Bold = true;
            worksheet.Cells[bodyRow + 13, 11].Formula = $"SUM(K{bodyRow + 7}:K{bodyRow + 11})";
            worksheet.Cells[bodyRow + 13, 11].Style.Font.Bold = true;
            worksheet.Cells[bodyRow + 13, 11].Style.Font.UnderLine = true;
            worksheet.Cells[bodyRow + 13, 12].Value = "PLN netto";

            //Style do podsumowania
            worksheet.Cells[bodyRow + 2, 11, bodyRow + 4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            // wyrównanie dla długości czasu pracy
            worksheet.Cells[bodyRow + 2, 7, bodyRow + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[bodyRow + 2, 7, bodyRow + 3, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

            // wyrównanie dla prace zdalne i prace w lokalu 
            worksheet.Cells[bodyRow + 2, 8, bodyRow + 4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[bodyRow + 2, 8, bodyRow + 4, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

            // wyrównanie dla sumowania godzin
            worksheet.Cells[bodyRow + 2, 10, bodyRow + 4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[bodyRow + 2, 10, bodyRow + 4, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // wyrównanie dla numeracji pkt. w podsumowaniu 
            worksheet.Cells[bodyRow + 7, 2, bodyRow + 11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[bodyRow + 7, 2, bodyRow + 11, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // wyrównanie dla opisu pkt. w podsumowaniu 
            worksheet.Cells[bodyRow + 7, 3, bodyRow + 11, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[bodyRow + 7, 3, bodyRow + 11, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            // wyrównanie dla rozliczenia pkt. w podsumowaniu 
            worksheet.Cells[bodyRow + 7, 11, bodyRow + 11, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[bodyRow + 7, 11, bodyRow + 11, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // wyrównanie, pogrubienie i zmiana czcionki dla "PLN netto" 
            worksheet.Cells[bodyRow + 7, 12, bodyRow + 11, 12].Style.Font.Bold = true;
            worksheet.Cells[bodyRow + 7, 12, bodyRow + 11, 12].Style.Font.Size = 9;
            worksheet.Cells[bodyRow + 7, 12, bodyRow + 11, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[bodyRow + 7, 12, bodyRow + 11, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // Ustawianie wysokość dla poszczególnych 
            worksheet.Row(bodyRow + 7).Height = 15;
            worksheet.Row(bodyRow + 8).Height = 15;
            worksheet.Row(bodyRow + 9).Height = 35;
            worksheet.Row(bodyRow + 10).Height = 400;
            worksheet.Row(bodyRow + 11).Height = 15;

            // Chowamy kolumny (I,J,M) pomocniczą  
            worksheet.Column(9).Hidden = true;
            worksheet.Column(10).Hidden = true;
            worksheet.Column(13).Hidden = true;

            // Ustawianie formatowania 
            worksheet.Cells[$"K{bodyRow + 2}:K{bodyRow + 4}"].Style.Numberformat.Format = "[hh]:mm";
            worksheet.Cells[$"K{bodyRow + 7}:K{bodyRow + 13}"].Style.Numberformat.Format = "# ##0.00;-# ##0.00";
        }

        private void macroSort(ExcelPackage package, int headerRow, int bodyRow, string worksheetName)
        {
            // Check if the workbook supports VBA
            if (package.Workbook.VbaProject == null)
            {
                // Create a VBA project for the workbook
                package.Workbook.CreateVBAProject();
            }

            // Access the VBA project of the workbook
            var vbaProject = package.Workbook.VbaProject;

            // Check if the module already exists or create a new one
            var moduleName = "Module1"; // Replace with the appropriate module name
            var codeModule = vbaProject.Modules.FirstOrDefault(m => m.Name == moduleName);

            if (codeModule == null)
            {
                // If the module doesn't exist, create a new one
                codeModule = vbaProject.Modules.AddModule(moduleName);
            }

            //tworzymy i wstrzykujemy macro do excela w języku VBA
            var vbaCode = "Sub sortowanie()\r\n" +
                              $"Range(\"A{headerRow + 1}:L{bodyRow}\").Select\r\n" +
                              $"ActiveWorkbook.Worksheets(\"{worksheetName}\").Sort.SortFields.Clear" +
                              $"ActiveWorkbook.Worksheets(\"{worksheetName}\").Sort.SortFields.Add2 Key: Range (\"A{headerRow + 1}:A{bodyRow}\") SortOn:=xlSortOnValues, Order:=xlAscending, DataOption:=xlSortNormal\r\n" +
                              $"ActiveWorkbook.Worksheets(\"{worksheetName}\").Sort.SortFields.Add2 Key: Range (\"B{headerRow + 1}:B{bodyRow}\") SortOn:=xlSortOnValues, Order:=xlAscending, DataOption:=xlSortNormal\r\n" +
                              $"ActiveWorkbook.Worksheets(\"{worksheetName}\").Sort.SortFields.Add2 Key: Range (\"G{headerRow + 1}:G{bodyRow}\") SortOn:=xlSortOnValues, Order:=xlAscending, DataOption:=xlSortNormal\r\n" +
                              $"With ActiveWorkbook.Worksheets(\"Raport\").Sort\r\n" +
                                  $".SetRange Range(\"A{headerRow + 1}:L{bodyRow}\")\r\n" +
                                  ".Header = x1Guess\r\n" +
                                  ".MatchCase = False\r\n" +
                                  ".Orientation = x1TopToBottom\r\n" +
                                  ".SortMethod = xlPinYin\r\n" +
                                  ".Apply\r\n" +
                          "End Sub";

            codeModule.Code = vbaCode;
        }
    }
}


