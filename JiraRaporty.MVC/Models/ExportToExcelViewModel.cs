using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace JiraRaporty.MVC.Models
{
    public class ExportToExcelViewModel
    {
        [Required(ErrorMessage = "Pole 'Od daty' jest wymagane")]
        [Display(Name = "Od daty")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Pole 'Do daty' jest wymagane")]
        [Display(Name = "Do daty")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Pole 'Projekty' jest wymagane")]
        [Display(Name = "Projekty")]
        public List<string> ProjectList { get; set; }

        public SelectList Projects { get; set; }

        [Display(Name = "Raport z pracami w lokalu")]
        public bool InLocal { get; set; }

        [Display(Name = "Podświetl zlecających bez nawiasów kwadratowych")]
        public bool HighlightReporters { get; set; }
    }
}