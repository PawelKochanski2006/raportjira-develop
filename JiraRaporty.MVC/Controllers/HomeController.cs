using JiraRaporty.Application.Authentication.Commands.Login;
using JiraRaporty.Application.Project.Queries.GetProjects;
using JiraRaporty.Application.Report.Commands.GenerateReport;
using JiraRaporty.MVC.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;

namespace JiraRaporty.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var sessionName = HttpContext.Session.GetString("_Name");
            var sessionPass = HttpContext.Session.GetString("_Pass");

            if (!string.IsNullOrEmpty(sessionName) && !string.IsNullOrEmpty(sessionPass))
            {
                return RedirectToAction("Raport");
            }

            var numberOfLogins = int.Parse(_configuration["JiraSettings:NumberOfLogins"] ?? "3");
            ViewData["NumberOfLogins"] = numberOfLogins;
            HttpContext.Session.SetInt32("_Log", numberOfLogins);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var command = new LoginCommand
            {
                Username = model.Username,
                Password = model.Password
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                ViewData["NumberOfLogins"] = HttpContext.Session.GetInt32("_Log");
                return View(model);
            }

            return RedirectToAction("Raport");
        }

        public async Task<IActionResult> Raport()
        {
            var sessionName = HttpContext.Session.GetString("_Name");
            var sessionPass = HttpContext.Session.GetString("_Pass");

            if (string.IsNullOrEmpty(sessionName) || string.IsNullOrEmpty(sessionPass))
            {
                return RedirectToAction("Index");
            }

            var projects = await _mediator.Send(new GetProjectsQuery());
            return View(new RaportViewModel { Projects = projects });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(ExportToExcelViewModel model)
        {
            var sessionName = HttpContext.Session.GetString("_Name");
            var sessionPass = HttpContext.Session.GetString("_Pass");

            if (string.IsNullOrEmpty(sessionName) || string.IsNullOrEmpty(sessionPass))
            {
                return RedirectToAction("Index");
            }

            var command = new GenerateReportCommand
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ProjectList = model.ProjectList,
                InLocal = model.InLocal
            };

            var result = await _mediator.Send(command);

            return File(result.Stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
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