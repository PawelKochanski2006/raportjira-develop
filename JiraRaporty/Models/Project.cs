namespace JiraRaporty.Models
{
    public class Project
    {
        public List<ProjectClass1> Property1 { get; set; }
    }

    public class ProjectClass1
    {
        public string expand { get; set; }
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public ProjectAvatarurls avatarUrls { get; set; }
        public ProjectProjectcategory projectCategory { get; set; }
        public string projectTypeKey { get; set; }
        public bool archived { get; set; }
    }

    public class ProjectAvatarurls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class ProjectProjectcategory
    {
        public string self { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

}
