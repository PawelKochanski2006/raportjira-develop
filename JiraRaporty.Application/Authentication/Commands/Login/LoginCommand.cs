using JiraRaporty.Application.Project;
using MediatR;

namespace JiraRaporty.Application.Authentication.Commands.Login
{
    public class LoginCommand : IRequest<LoginResult>
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<ProjectDto>? Projects { get; set; }
    }
}