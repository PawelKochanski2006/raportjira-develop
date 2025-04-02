using JiraRaporty.Domain.Entities;

namespace JiraRaporty.Domain.Interfaces
{
    public interface IExcelService
    {
        MemoryStream GenerateExcelReport(DateTime fromDate, DateTime toDate, List<string> projectList, string projects, bool inLocal, ExcelIssues excelIssues);
    }
}