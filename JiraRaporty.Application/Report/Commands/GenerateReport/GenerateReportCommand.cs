using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRaporty.Application.Report.Commands.GenerateReport
{
    public class GenerateReportCommand : IRequest<ReportResult>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<string> ProjectList { get; set; } = new List<string>();
        public bool InLocal { get; set; }
    }

    public class ReportResult
    {
        public MemoryStream Stream { get; set; } = default!;
        public string FileName { get; set; } = default!;
    }
}