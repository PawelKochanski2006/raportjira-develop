using JiraRaporty.Domain.Entities;
using JiraRaporty.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace JiraRaporty.Infrastructure.Services
{
    public class JiraApiService : IJiraApiService
    {
        private readonly RestClient _client;

        public JiraApiService(string baseUrl, string userName, string password)
        {
            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };

            _client = new RestClient(options);
        }

        public async Task<List<ProjectDetails>> GetProjects()
        {
            var endpoint = "/project";
            var request = new RestRequest(endpoint);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to get projects: {response.ErrorMessage}");
            }

            var projects = JsonConvert.DeserializeObject<List<ProjectDetails>>(response.Content!);
            return projects!;
        }

        public async Task<ProjectIssues> GetProjectIssues(DateTime fromDate, DateTime toDate, List<string> projects)
        {
            var endpoint = "/search";
            var request = new RestRequest(endpoint);
            var projectsString = string.Join("\",\"", projects);
            var query = $"project in(\"{projectsString}\") AND worklogDate >= {fromDate:yyyy-MM-dd} AND worklogDate <= {toDate:yyyy-MM-dd}";

            request.AddParameter("jql", query);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to get project issues: {response.ErrorMessage}");
            }

            var projectIssues = JsonConvert.DeserializeObject<ProjectIssues>(response.Content!);
            return projectIssues!;
        }

        public async Task<IssueWorklog> GetIssueWorklog(string issueId)
        {
            var endpoint = $"/issue/{issueId}/worklog";
            var request = new RestRequest(endpoint);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to get issue worklog: {response.ErrorMessage}");
            }

            var worklog = JsonConvert.DeserializeObject<IssueWorklog>(response.Content!);
            return worklog!;
        }

        public async Task<EpicName> GetEpicName(string epicKey)
        {
            var endpoint = $"/issue/{epicKey}";
            var request = new RestRequest(endpoint);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to get epic: {response.ErrorMessage}");
            }

            var epic = JsonConvert.DeserializeObject<EpicName>(response.Content!);
            return epic!;
        }

        public async Task<ExcelIssues> GenerateReport(DateTime fromDate, DateTime toDate, List<string> projects)
        {
            var projectIssues = await GetProjectIssues(fromDate, toDate, projects);
            var excelIssues = new ExcelIssues();

            foreach (var issue in projectIssues.issues)
            {
                // Get worklogs for each issue
                var issueWorklog = await GetIssueWorklog(issue.key);

                // Filter worklogs by date range
                var worklogs = issueWorklog.worklogs.Where(w =>
                    w.started >= fromDate && w.started <= toDate).ToArray();

                // Get epic name if available
                string epicName = "";
                if (!string.IsNullOrEmpty(issue.fields.customfield_10003))
                {
                    try
                    {
                        var epic = await GetEpicName(issue.fields.customfield_10003);
                        epicName = epic.fields.summary;
                    }
                    catch
                    {
                        // If we can't get the epic, just use the key
                        epicName = issue.fields.customfield_10003;
                    }
                }

                // Add each worklog as a row in the Excel report
                foreach (var worklog in worklogs)
                {
                    excelIssues.Rows.Add(new ExcelIssuesRow
                    {
                        Project = issue.fields.project.name,
                        Key = issue.key,
                        Summary = issue.fields.summary,
                        Reporter = issue.fields.reporter.displayName,
                        Labels = string.Join(", ", issue.fields.labels ?? Array.Empty<string>()),
                        Started = worklog.started,
                        WorklogAuthor = worklog.author.displayName,
                        TimeSpent = worklog.timeSpent,
                        TimeSpentSeconds = worklog.timeSpentSeconds,
                        Comment = worklog.comment,
                        EpicName = epicName
                    });
                }
            }

            return excelIssues;
        }

        public void DateSwap(ref DateTime firstDate, ref DateTime secondDate)
        {
            if (firstDate > secondDate)
            {
                var temp = firstDate;
                firstDate = secondDate;
                secondDate = temp;
            }
        }
    }
}