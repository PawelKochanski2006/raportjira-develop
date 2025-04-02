using JiraRaporty.Application.Project;

namespace JiraRaporty.MVC.Models
{
    public class RaportViewModel
    {
        public List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    }
}