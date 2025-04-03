using JiraRaporty.Domain.Entities;
using JiraRaporty.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace JiraRaporty.Application.Report.Commands.GenerateReport
{
    public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, MemoryStream>
    {
        private readonly IJiraApiService _jiraApiService;
        private readonly IExcelService _excelService;

        public GenerateReportCommandHandler(IJiraApiService jiraApiService, IExcelService excelService)
        {
            _jiraApiService = jiraApiService;
            _excelService = excelService;
        }

        public async Task<MemoryStream> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
        {
            var projectIssues = await _jiraApiService.GetProjectIssues(request.FromDate, request.ToDate, request.ProjectList);
            var excelIssues = new ExcelIssues();

            foreach (var issue in projectIssues.issues)
            {
                var worklog = await _jiraApiService.GetIssueWorklog(issue.id);

                foreach (var log in worklog.worklogs)
                {
                    var logDate = log.started.Date;
                    if (logDate >= request.FromDate.Date && logDate <= request.ToDate.Date)
                    {
                        var epicName = issue.fields.customfield_10002;

                        excelIssues.Rows.Add(new ExcelIssuesRow
                        {
                            Project = issue.fields.project?.name,
                            Key = issue.key,
                            Summary = issue.fields.summary,
                            EpicName = epicName,
                            Reporter = issue.fields.reporter?.displayName,
                            Labels = string.Join(", ", issue.fields.labels ?? Array.Empty<string>()),
                            Started = log.started,
                            WorklogAuthor = log.author?.displayName,
                            TimeSpent = log.timeSpent,
                            TimeSpentSeconds = log.timeSpentSeconds,
                            Comment = log.comment
                        });
                    }
                }
            }

            var projects = string.Join(", ", request.ProjectList);
            return _excelService.GenerateExcelReport(
                request.FromDate,
                request.ToDate,
                request.ProjectList,
                projects,
                request.InLocal,
                excelIssues,
                request.HighlightReporters);
        }
    }
}