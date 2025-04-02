namespace JiraRaporty.Models
{ 

    public class EpicName
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public EpicNameFields fields { get; set; }
    }

    public class EpicNameFields
    {
        public object[] fixVersions { get; set; }
        public string customfield_11200 { get; set; }
        public object resolution { get; set; }
        public object customfield_11712 { get; set; }
        public object customfield_10900 { get; set; }
        public DateTime? lastViewed { get; set; }
        public EpicNamePriority priority { get; set; }
        public object customfield_10100 { get; set; }
        public object customfield_12400 { get; set; }
        public object[] labels { get; set; }
        public object timeestimate { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public object[] versions { get; set; }
        public object[] issuelinks { get; set; }
        public EpicNameAssignee assignee { get; set; }
        public EpicNameStatus status { get; set; }
        public object[] components { get; set; }
        public object customfield_11301 { get; set; }
        public object customfield_11302 { get; set; }
        public object archiveddate { get; set; }
        public object customfield_11414 { get; set; }
        public object customfield_11413 { get; set; }
        public object customfield_12502 { get; set; }
        public object customfield_10600 { get; set; }
        public object customfield_11415 { get; set; }
        public object aggregatetimeestimate { get; set; }
        public EpicNameCreator creator { get; set; }
        public object[] subtasks { get; set; }
        public EpicNameReporter reporter { get; set; }
        public object customfield_12100 { get; set; }
        public EpicNameAggregateprogress aggregateprogress { get; set; }
        public object customfield_11410 { get; set; }
        public object customfield_12102 { get; set; }
        public object customfield_11412 { get; set; }
        public object customfield_12501 { get; set; }
        public object customfield_11411 { get; set; }
        public object customfield_12500 { get; set; }
        public object customfield_11409 { get; set; }
        public EpicNameProgress progress { get; set; }
        public EpicNameVotes votes { get; set; }
        public EpicNameWorklog worklog { get; set; }
        public object archivedby { get; set; }
        public EpicNameIssuetype issuetype { get; set; }
        public object timespent { get; set; }
        public EpicNameProject project { get; set; }
        public object customfield_11000 { get; set; }
        public object customfield_11001 { get; set; }
        public object aggregatetimespent { get; set; }
        public string customfield_11401 { get; set; }
        public object customfield_11400 { get; set; }
        public object customfield_10820 { get; set; }
        public object resolutiondate { get; set; }
        public int workratio { get; set; }
        public EpicNameWatches watches { get; set; }
        public DateTime created { get; set; }
        public object customfield_12200 { get; set; }
        public object customfield_12202 { get; set; }
        public object customfield_12201 { get; set; }
        public object customfield_10300 { get; set; }
        public EpicNameCustomfield_12600 customfield_12600 { get; set; }
        public object customfield_11501 { get; set; }
        public object customfield_10810 { get; set; }
        public object customfield_10811 { get; set; }
        public object customfield_10812 { get; set; }
        public object customfield_10813 { get; set; }
        public object customfield_10814 { get; set; }
        public object customfield_10815 { get; set; }
        public object customfield_10816 { get; set; }
        public object customfield_10817 { get; set; }
        public object customfield_10818 { get; set; }
        public object customfield_10819 { get; set; }
        public DateTime updated { get; set; }
        public object timeoriginalestimate { get; set; }
        public object description { get; set; }
        public object customfield_10010 { get; set; }
        public object customfield_10011 { get; set; }
        public string customfield_11100 { get; set; }
        public object customfield_10012 { get; set; }
        public object customfield_11500 { get; set; }
        public EpicNameTimetracking timetracking { get; set; }
        public string customfield_10005 { get; set; }
        public string customfield_10006 { get; set; }
        public object customfield_10007 { get; set; }
        public object[] attachment { get; set; }
        public object customfield_10009 { get; set; }
        public object customfield_10804 { get; set; }
        public object customfield_10805 { get; set; }
        public object customfield_10806 { get; set; }
        public object customfield_10807 { get; set; }
        public object customfield_10808 { get; set; }
        public object customfield_10809 { get; set; }
        public string summary { get; set; }
        public object customfield_12301 { get; set; }
        public string customfield_10002 { get; set; }
        public object customfield_12300 { get; set; }
        public object customfield_10003 { get; set; }
        public EpicNameCustomfield_10004 customfield_10004 { get; set; }
        public object customfield_10400 { get; set; }
        public object customfield_11601 { get; set; }
        public object customfield_11600 { get; set; }
        public object environment { get; set; }
        public object customfield_11603 { get; set; }
        public object customfield_11602 { get; set; }
        public object duedate { get; set; }
        public EpicNameComment comment { get; set; }
    }

    public class EpicNamePriority
    {
        public string self { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class EpicNameAssignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public EpicNameAvatarurls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class EpicNameAvatarurls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class EpicNameStatus
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public EpicNameStatuscategory statusCategory { get; set; }
    }

    public class EpicNameStatuscategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class EpicNameCreator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public EpicNameAvatarurls1 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class EpicNameAvatarurls1
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class EpicNameReporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public EpicNameAvatarurls2 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class EpicNameAvatarurls2
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class EpicNameAggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class EpicNameProgress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class EpicNameVotes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class EpicNameWorklog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public object[] worklogs { get; set; }
    }

    public class EpicNameIssuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }

    public class EpicNameProject
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string projectTypeKey { get; set; }
        public EpicNameAvatarurls3 avatarUrls { get; set; }
        public EpicNameProjectcategory projectCategory { get; set; }
    }

    public class EpicNameAvatarurls3
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class EpicNameProjectcategory
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    public class EpicNameWatches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class EpicNameCustomfield_12600
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public EpicNameAvatarurls4 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class EpicNameAvatarurls4
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class EpicNameTimetracking
    {
    }

    public class EpicNameCustomfield_10004
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public bool disabled { get; set; }
    }

    public class EpicNameComment
    {
        public object[] comments { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public int startAt { get; set; }
    }

}
