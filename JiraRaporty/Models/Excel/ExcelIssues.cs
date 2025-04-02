using System;

namespace JiraRaporty.Models.Excel
{
    public class ExcelIssues
    {
        public List<ExcelIssuesRow> rows { get; set; } = new List<ExcelIssuesRow>();
    }

    public class ExcelIssuesRow
    {
        public string? project {  get; set; }
        public string? key { get; set; }
        public string? summery { get; set; }
        public string? epicName { get; set; }
        public string? reporter { get; set; }
        public string? labels { get; set; }
        public DateTime started { get; set; }
        public string? worklogAutor { get; set; }
        public string? timeSpent { get; set; }
        public int timeSpentSeconds { get; set; }
        public string? comment { get; set; }
    }
}