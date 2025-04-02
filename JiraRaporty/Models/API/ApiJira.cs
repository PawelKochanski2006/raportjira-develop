using RestSharp.Authenticators;
using RestSharp;
using Newtonsoft.Json;
using JiraRaporty.Models.Zmiana;
using static System.Runtime.InteropServices.JavaScript.JSType;
using JiraRaporty.Models.Excel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;

namespace JiraRaporty.Models.API
{
    public class ApiJira
    {
        private readonly RestClient _client;

        public ApiJira(string baseUrl, string userName, string password)
        {
            var Options = new RestClientOptions(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };

            _client = new RestClient(Options);
        }

        public async Task<List<ProjectClass1>> GetProject()
        {
            var endpoint = $"/project";
            var request = new RestRequest(endpoint);

            var response = await _client.ExecuteAsync(request);

            var project = JsonConvert.DeserializeObject<List<ProjectClass1>>(response.Content!);
            return project;
        }

        public async Task<ProjectIssues> GetProjectIssue(DateTime fromDate, DateTime toDate, List<string> project)
        {
            var endpoint = $"/search";
            var request = new RestRequest(endpoint);
            var projects = string.Join("\",\"", project);
            var query = $"project in(\"{projects}\") AND worklogDate >= {fromDate.ToString("yyyy-MM-dd")} AND worklogDate <= {toDate.ToString("yyyy-MM-dd")}";

            request.AddParameter("jql", query);

            var response = await _client.ExecuteAsync(request);
            var pojectIssues  = JsonConvert.DeserializeObject<ProjectIssues>(response.Content!);
            return pojectIssues;  
        }

        public async Task<IssueWorklog> GetIssueWorklog(string key)
        {
            var endpoint = $"issue/{key}/worklog";
            var request = new RestRequest(endpoint);

            var response = await _client.ExecuteAsync(request);

            var issueWorklog = JsonConvert.DeserializeObject<IssueWorklog>(response.Content!);
            return issueWorklog;
        }

        public async Task<EpicName> GetEpicName(string key)
        {
            var endpoint = $"issue/{key}";
            var request = new RestRequest(endpoint);

            var response = await _client.ExecuteAsync(request);

            var epicName = JsonConvert.DeserializeObject<EpicName>(response.Content!);
            return epicName;
        }

        public async Task<ExcelIssues> Raport(DateTime fromDate, DateTime toDate, List<string> project)
        {
            var excelIssues = new ExcelIssues();
            var projectIssues = await GetProjectIssue(fromDate, toDate, project);

            foreach (var issue in projectIssues.issues)
            {
                var issueWorklogs = await GetIssueWorklog(issue.key);

                var worklogs = from w in issueWorklogs.worklogs
                               where w.started <= toDate && w.started >= fromDate
                               select w;
                var epicName = "";
                if (!string.IsNullOrEmpty(issue.fields.customfield_10003)) {
                    var epicNames = await GetEpicName(issue.fields.customfield_10003);
                    if (epicNames != null && epicNames.fields != null)
                    {
                        epicName = epicNames.fields.customfield_10005;
                    }
                    else
                    {
                        epicName= issue.fields.customfield_10003;
                    }
                } 


                foreach (var worklog in worklogs)
                    {
                       excelIssues.rows.Add(new ExcelIssuesRow()
                       {
                           project = issue.fields.project.name,
                           key = issue.key,
                           summery = issue.fields.summary,
                           reporter = issue.fields.reporter.displayName,
                           labels = string.Join(", ", issue.fields.labels),
                           started = worklog.started,
                           worklogAutor = worklog.author.displayName,
                           timeSpent = worklog.timeSpent,
                           timeSpentSeconds = worklog.timeSpentSeconds,
                           comment = worklog.comment,
                           epicName = epicName
                       });
                    }
            }

            return excelIssues;
        }

        // Jeżeli pierwsza data jest większa od drugiej daty to zamieniamy danych w zminnych 
        public void dateSwap(ref DateTime firstDate, ref DateTime secoundDate)
        {
            if (firstDate > secoundDate)
            {
                (firstDate, secoundDate) = (secoundDate, firstDate);
            }
        }

    }
}
