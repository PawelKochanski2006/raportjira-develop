using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRaporty.Application.Project
{
    public class ProjectDto
    {
        public string Id { get; set; } = default!;
        public string Key { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}