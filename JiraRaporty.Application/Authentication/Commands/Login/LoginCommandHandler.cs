using AutoMapper;
using JiraRaporty.Application.Project;
using JiraRaporty.Domain.Entities;
using JiraRaporty.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Extensions;

namespace JiraRaporty.Application.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IJiraApiService _jiraApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public LoginCommandHandler(IJiraApiService jiraApiService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _jiraApiService = jiraApiService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Set session variables
                _httpContextAccessor.HttpContext!.Session.SetString("_Name", request.Username);
                _httpContextAccessor.HttpContext!.Session.SetString("_Pass", request.Password);

                // Get projects from Jira
                var projects = await _jiraApiService.GetProjects();
                var projectDtos = _mapper.Map<List<ProjectDto>>(projects);

                // Store projects in session
                _httpContextAccessor.HttpContext!.Session.SetString("_Project", JsonConvert.SerializeObject(projects));

                return new LoginResult
                {
                    Success = true,
                    Projects = projectDtos
                };
            }
            catch (Exception ex)
            {
                // Decrement login attempts
                var numberOfLogins = _httpContextAccessor.HttpContext!.Session.GetInt32("_Log") ?? 0;
                if (numberOfLogins > 0)
                {
                    _httpContextAccessor.HttpContext!.Session.SetInt32("_Log", numberOfLogins - 1);
                }

                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}