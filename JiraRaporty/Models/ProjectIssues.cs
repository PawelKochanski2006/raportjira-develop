namespace JiraRaporty.Models
{

    public class ProjectIssues
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public ProjectIssuesIssue[] issues { get; set; }
    }

    public class ProjectIssuesIssue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public ProjectIssuesFields fields { get; set; }
    }

    public class ProjectIssuesFields
    {
        public ProjectIssuesIssuetype issuetype { get; set; }
        public int timespent { get; set; }
        public ProjectIssuesProject project { get; set; }
        public object customfield_11000 { get; set; }
        public object[] fixVersions { get; set; }
        public object customfield_11001 { get; set; }
        public string customfield_11200 { get; set; }
        public int aggregatetimespent { get; set; }
        public ProjectIssuesResolution resolution { get; set; }
        public string customfield_11401 { get; set; }
        public object customfield_11400 { get; set; }
        public object customfield_10820 { get; set; }
        public object customfield_11712 { get; set; }
        public object customfield_10900 { get; set; }
        public DateTime? resolutiondate { get; set; }
        public long workratio { get; set; }
        public DateTime? lastViewed { get; set; }
        public ProjectIssuesWatches watches { get; set; }
        public DateTime created { get; set; }
        public object customfield_12200 { get; set; }
        public ProjectIssuesPriority priority { get; set; }
        public object customfield_10100 { get; set; }
        public object customfield_12400 { get; set; }
        public object customfield_12202 { get; set; }
        public object customfield_12201 { get; set; }
        public object customfield_10300 { get; set; }
        public object customfield_12600 { get; set; }
        public string[] labels { get; set; }
        public object customfield_11501 { get; set; }
        public object customfield_10810 { get; set; }
        public object customfield_10811 { get; set; }
        public object customfield_10812 { get; set; }
        public int? timeestimate { get; set; }
        public int? aggregatetimeoriginalestimate { get; set; }
        public object[] versions { get; set; }
        public object customfield_10813 { get; set; }
        public object customfield_10814 { get; set; }
        public object customfield_10815 { get; set; }
        public object customfield_10816 { get; set; }
        public object customfield_10817 { get; set; }
        public object[] issuelinks { get; set; }
        public object customfield_10818 { get; set; }
        public object customfield_10819 { get; set; }
        public ProjectIssuesAssignee assignee { get; set; }
        public DateTime updated { get; set; }
        public ProjectIssuesStatus status { get; set; }
        public object[] components { get; set; }
        public int? timeoriginalestimate { get; set; }
        public string description { get; set; }
        public object customfield_10010 { get; set; }
        public object customfield_10011 { get; set; }
        public string customfield_11100 { get; set; }
        public object customfield_11301 { get; set; }
        public object customfield_11500 { get; set; }
        public object customfield_11302 { get; set; }
        public object customfield_11414 { get; set; }
        public object customfield_11413 { get; set; }
        public object customfield_12502 { get; set; }
        public object customfield_10600 { get; set; }
        public object customfield_10007 { get; set; }
        public object customfield_11415 { get; set; }
        public object customfield_10009 { get; set; }
        public int? aggregatetimeestimate { get; set; }
        public object customfield_10804 { get; set; }
        public object customfield_10805 { get; set; }
        public object customfield_10806 { get; set; }
        public object customfield_10807 { get; set; }
        public object customfield_10808 { get; set; }
        public object customfield_10809 { get; set; }
        public string summary { get; set; }
        public ProjectIssuesCreator creator { get; set; }
        public object[] subtasks { get; set; }
        public ProjectIssuesReporter reporter { get; set; }
        public object customfield_12100 { get; set; }
        public ProjectIssuesAggregateprogress aggregateprogress { get; set; }
        public object customfield_11410 { get; set; }
        public object customfield_12301 { get; set; }
        public string customfield_10002 { get; set; }
        public object customfield_12102 { get; set; }
        public object customfield_12300 { get; set; }
        public string customfield_10003 { get; set; }
        public object customfield_11412 { get; set; }
        public object customfield_12501 { get; set; }
        public object customfield_11411 { get; set; }
        public object customfield_12500 { get; set; }
        public object customfield_10400 { get; set; }
        public object customfield_11601 { get; set; }
        public object customfield_11600 { get; set; }
        public object environment { get; set; }
        public object customfield_11603 { get; set; }
        public object customfield_11602 { get; set; }
        public object duedate { get; set; }
        public object customfield_11409 { get; set; }
        public ProjectIssuesProgress progress { get; set; }
        public ProjectIssuesVotes votes { get; set; }
    }

    public class ProjectIssuesIssuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }

    public class ProjectIssuesProject
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string projectTypeKey { get; set; }
        public ProjectIssuesAvatarurls avatarUrls { get; set; }
        public ProjectIssuesProjectcategory projectCategory { get; set; }
    }

    public class ProjectIssuesAvatarurls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class ProjectIssuesProjectcategory
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    public class ProjectIssuesResolution
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    public class ProjectIssuesWatches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class ProjectIssuesPriority
    {
        public string self { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class ProjectIssuesAssignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public ProjectIssuesAvatarurls1 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class ProjectIssuesAvatarurls1
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class ProjectIssuesStatus
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public ProjectIssuesStatuscategory statusCategory { get; set; }
    }

    public class ProjectIssuesStatuscategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class ProjectIssuesCreator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public ProjectIssuesAvatarurls2 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class ProjectIssuesAvatarurls2
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class ProjectIssuesReporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public ProjectIssuesAvatarurls3 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class ProjectIssuesAvatarurls3
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class ProjectIssuesAggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
        public int percent { get; set; }
    }

    public class ProjectIssuesProgress
    {
        public int progress { get; set; }
        public int total { get; set; }
        public int percent { get; set; }
    }

    public class ProjectIssuesVotes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

}
