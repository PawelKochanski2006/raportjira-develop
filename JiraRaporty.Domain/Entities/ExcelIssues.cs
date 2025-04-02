using System;

namespace JiraRaporty.Domain.Entities
{
    public class ExcelIssues
    {
        public List<ExcelIssuesRow> Rows { get; set; } = new List<ExcelIssuesRow>();
    }

    public class ExcelIssuesRow
    {
        public string? Project { get; set; }
        public string? Key { get; set; }
        public string? Summary { get; set; }
        public string? EpicName { get; set; }
        public string? Reporter { get; set; }
        public string? Labels { get; set; }
        public DateTime Started { get; set; }
        public string? WorklogAuthor { get; set; }
        public string? TimeSpent { get; set; }
        public int TimeSpentSeconds { get; set; }
        public string? Comment { get; set; }
    }
}