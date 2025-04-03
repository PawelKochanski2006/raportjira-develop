using JiraRaporty.Application.Authentication.Commands.Login;
using JiraRaporty.Application.Project.Queries.GetProjects;
using JiraRaporty.Application.Report.Commands.GenerateReport;
using JiraRaporty.MVC.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JiraRaporty.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IMediator mediator, ILogger<HomeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                return RedirectToAction("Raport");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var command = new LoginCommand
                {
                    Username = model.Username,
                    Password = model.Password
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("Raport");
                }
                else
                {
                    ModelState.AddModelError("", "Nieprawid³owa nazwa u¿ytkownika lub has³o");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError("", "Wyst¹pi³ b³¹d podczas logowania");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Raport()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") != "true")
            {
                return RedirectToAction("Index");
            }

            var projects = await _mediator.Send(new GetProjectsQuery());
            var model = new RaportViewModel
            {
                Projects = projects
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(RaportViewModel model)
        {
            if (HttpContext.Session.GetString("IsAuthenticated") != "true")
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                model.Projects = await _mediator.Send(new GetProjectsQuery());
                return View("Raport", model);
            }

            try
            {
                var command = new GenerateReportCommand
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ProjectList = model.ProjectList,
                    InLocal = model.InLocal,
                    HighlightReporters = model.HighlightReporters
                };

                var stream = await _mediator.Send(command);

                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Raport_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during export to Excel");
                ModelState.AddModelError("", "Wyst¹pi³ b³¹d podczas eksportu do Excela");
                model.Projects = await _mediator.Send(new GetProjectsQuery());
                return View("Raport", model);
            }
        }

        public IActionResult Logout()
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