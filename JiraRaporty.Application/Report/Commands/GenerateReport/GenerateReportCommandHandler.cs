using JiraRaporty.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; // Add this for session extensions

namespace JiraRaporty.Application.Report.Commands.GenerateReport
{
    public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, ReportResult>
    {
        private readonly IJiraApiService _jiraApiService;
        private readonly IExcelService _excelService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenerateReportCommandHandler(IJiraApiService jiraApiService, IExcelService excelService, IHttpContextAccessor httpContextAccessor)
        {
            _jiraApiService = jiraApiService;
            _excelService = excelService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ReportResult> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
        {
            // Create copies of dates to avoid modifying the original request
            var fromDate = request.FromDate;
            var toDate = request.ToDate;

            // Swap dates if needed
            _jiraApiService.DateSwap(ref fromDate, ref toDate);

            // Add time to include the entire day
            var endDate = toDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

            // Get report data from Jira
            var excelIssues = await _jiraApiService.GenerateReport(fromDate, endDate, request.ProjectList);

            // Join project names for display
            var projects = string.Join(", ", request.ProjectList);

            // Generate Excel file
            var stream = _excelService.GenerateExcelReport(fromDate, toDate, request.ProjectList, projects, request.InLocal, excelIssues);

            // Create file name
            var fileName = $"{projects} {fromDate:dd.MM.yyyy} - {toDate:dd.MM.yyyy}.xlsx";

            return new ReportResult
            {
                Stream = stream,
                FileName = fileName
            };
        }
    }
}