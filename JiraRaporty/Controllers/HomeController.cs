using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading;
using JiraRaporty.Models;
using JiraRaporty.Models.API;
using JiraRaporty.Models.Excel;
using JiraRaporty.Models.Zmiana;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.Style;
using RestSharp;
using RestSharp.Authenticators;

namespace JiraRaporty.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _iConfig;
        private const string SessionName = "_Name";
        private const string SessionPass = "_Pass";
        private const string SessionProjects = "_Project";
        private string SessionNumberOfLogins = "_Log";

        public HomeController(ILogger<HomeController> logger, IConfiguration iConfig)
        {
            _logger = logger;
            _iConfig = iConfig;
        }

        public async Task<IActionResult> Index()
        {
            var sessionName = HttpContext.Session.GetString(SessionName);
            var sessionPass = HttpContext.Session.GetString(SessionPass);
            if (!string.IsNullOrEmpty(sessionName) || !string.IsNullOrEmpty(sessionPass))
            {
                return RedirectToAction("Raport");
            }
            var NumberOfLogins = _iConfig.GetValue<int>("JiraSettings:NumberOfLogins");
            ViewData["NumberOfLogins"] = NumberOfLogins; 
            HttpContext.Session.SetInt32(SessionNumberOfLogins, NumberOfLogins);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var JiraUrlConfig = _iConfig.GetValue<string>("JiraSettings:ApiUrl");
                var jiraApi = new ApiJira(JiraUrlConfig, userName, password);

                try
                {
                    var projects = await jiraApi.GetProject();
                    HttpContext.Session.SetString(SessionName, userName);
                    HttpContext.Session.SetString(SessionPass, password);
                    HttpContext.Session.SetString(SessionProjects, JsonConvert.SerializeObject(projects));
                    
                    
                    return RedirectToAction("Raport");
                }
                catch (Exception e)
                {

                    var NumberOfLogins = HttpContext.Session.GetInt32(SessionNumberOfLogins);
                    if (NumberOfLogins > 0)
                    {
                        HttpContext.Session.SetInt32(SessionNumberOfLogins, (int)(--NumberOfLogins));
                        ViewData["Message"] = $"Podano niepoprawne dane logowania, pozostała ilość prób: {NumberOfLogins}";
                    }
                    else
                    {
                        ViewData["Message"] = $"Masz {NumberOfLogins} prób wróć później i spróbuj ponownie się zalogować";
                    }

                    ViewData["NumberOfLogins"] = NumberOfLogins;
                    return View();
                }
                
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Raport()
        {
            var sessionName = HttpContext.Session.GetString(SessionName);
            var sessionPass = HttpContext.Session.GetString(SessionPass);
            if (string.IsNullOrEmpty(sessionName) || string.IsNullOrEmpty(sessionPass))
            {
                return RedirectToAction("Index");
            }
            ViewData["Projects"] = JsonConvert.DeserializeObject<List<ProjectClass1>>(HttpContext.Session.GetString(SessionProjects));
            return View();
        }


        // Tworzenie pliku Excel
        public async Task<IActionResult> ExportToExcel(DateTime fromDate, DateTime toDate, List<string> projectList, bool inLocal)
        {
            var JiraUrlConfig = _iConfig.GetValue<string>("JiraSettings:ApiUrl");
            var sessionName = HttpContext.Session.GetString(SessionName);
            var sessionPass = HttpContext.Session.GetString(SessionPass);
            if (string.IsNullOrEmpty(sessionName) || string.IsNullOrEmpty(sessionPass))
            {
                return RedirectToAction("Index");
            }
            var jiraApi = new ApiJira(JiraUrlConfig, sessionName, sessionPass);

            // Sprawdzamy czy fromDate jest mniejsza od toDate, jak nie jest to zmieniamy miejsce danych pomiędzy zmiennymi  
            jiraApi.dateSwap(ref fromDate, ref toDate);
            toDate = toDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMicroseconds(999); // Dodajemy do toDate 24h 59min 59s 999ms aby mieć wyszukiwanie do całego dnia 
            var excelIssues = await jiraApi.Raport(fromDate, toDate, projectList); // Odpytujemy api i zwracamy obiekt 
            var projects = string.Join(", ", projectList); // Łączymy listę projectList w jeden ciąg znaków 
            var stream = new MemoryStream();

            new ExportExcel(fromDate, toDate, projectList, projects, inLocal, excelIssues, stream);

            // Tworzymy nazwę Excela
            //var excelName = $"{projects} {fromDate.ToString("dd.MM.yyyy")} - {toDate.ToString("dd.MM.yyyy")}.xlsm";
            var excelName = $"{projects} {fromDate.ToString("dd.MM.yyyy")} - {toDate.ToString("dd.MM.yyyy")}.xlsx";

            // Zwracamy utworzony plik i go pobieramy na dysk w miejsce ustawione przez użytkownika w przeglądarce (defaults: Pobrane) 
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}