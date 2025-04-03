using JiraRaporty.Application.Project;
using System.ComponentModel.DataAnnotations;

namespace JiraRaporty.MVC.Models
{
    public class RaportViewModel
    {
        [Required(ErrorMessage = "Pole 'Data od' jest wymagane")]
        [Display(Name = "Data od")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Now.AddDays(-7);

        [Required(ErrorMessage = "Pole 'Data do' jest wymagane")]
        [Display(Name = "Data do")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Wybierz co najmniej jeden projekt")]
        [Display(Name = "Projekty")]
        public List<string> ProjectList { get; set; } = new List<string>();

        public List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();

        [Display(Name = "Praca w lokalu")]
        public bool InLocal { get; set; }

        [Display(Name = "Podświetl zlecających")]
        public bool HighlightReporters { get; set; } = true;
    }
}