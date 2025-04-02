using JiraRaporty.Domain.Entities;

namespace JiraRaporty.Domain.Interfaces
{
    public interface IJiraApiService
    {
        Task<List<ProjectDetails>> GetProjects();

        Task<ProjectIssues> GetProjectIssues(DateTime fromDate, DateTime toDate, List<string> projects);

        Task<IssueWorklog> GetIssueWorklog(string issueId);

        Task<EpicName> GetEpicName(string epicKey);

        Task<ExcelIssues> GenerateReport(DateTime fromDate, DateTime toDate, List<string> projects);

        void DateSwap(ref DateTime firstDate, ref DateTime secondDate);
    }
}