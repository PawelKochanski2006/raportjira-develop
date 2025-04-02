using AutoMapper;
using JiraRaporty.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace JiraRaporty.Application.Project.Queries.GetProjects
{
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetProjectsQueryHandler(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projectsJson = _httpContextAccessor.HttpContext!.Session.GetString("_Project");
            if (string.IsNullOrEmpty(projectsJson))
            {
                throw new Exception("No projects found in session. Please log in again.");
            }

            var projects = JsonConvert.DeserializeObject<List<ProjectDetails>>(projectsJson);
            var projectDtos = _mapper.Map<List<ProjectDto>>(projects);

            return Task.FromResult(projectDtos);
        }
    }
}