using System;
using System.Web.UI.WebControls;
using Composite.Data;
using Composite.Data.Types;
using System.Data;
using System.Linq;
using Orckestra.Versioning.VersionPublication;
using Composite.C1Console.Security;
using Composite.Core.WebClient.FlowMediators;
using Composite.Core.WebClient.UiControlLib;
using System.Collections.Generic;
using Composite.C1Console.Workflow;
using Orckestra.Versioning.VersionPublication.Workflows;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Core.Extensions;
using Composite.Core.Linq;
using Composite.Core.ResourceSystem;
using Orckestra.Versioning.VersionPublication.Data;
using ManagementStrings = Composite.Core.ResourceSystem.LocalizationFiles.Composite_Management;

public partial class Orckestra_Versioning_VersionPublication_publicationschedule : System.Web.UI.Page
{
    protected Guid _pageId;

    private string PageVersionId
    {
        get { return Request["PageVersionId"]; }
    }

    private string ConsoleId
    {
        get { return Request["__CONSOLEID"]; }
    }


    private bool SortByAscending
    {
        get { return (bool)ViewState["SortByAscending"]; }
        set { ViewState["SortByAscending"] = value; }
    }


    private string SortColumn
    {
        get { return (string)ViewState["Column"]; }
        set { ViewState["Column"] = value; }
    }

    protected string PublicationStatus
    {
        get { return (string)ViewState["PublicationStatus"]; }
        set { ViewState["PublicationStatus"] = value; }
    }

    public bool ShowAddVersion = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        _pageId = new Guid(Request.QueryString["PageId"]);

        if (!IsPostBack)
        {
            using (var con = new DataConnection())
            {
                con.AddService(VersioningServiceSettings.Active());
                PublicationStatus = con.Get<IPage>().Any(f => f.Id == _pageId) ? "Active" : "Past";
            }

            SortColumn = "Version";
            SortByAscending = true;

            BindRepeater();
        }
        else if (Request["__EVENTTARGET"] == "UPDATE")
        {
            BindRepeater();
        }

    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (PublicationStatus == "Active")
        {
            ActiveButton.Attributes.Add("client_active", "true");
            PastButton.Attributes.Remove("client_active");
        }
        else {
            ActiveButton.Attributes.Remove("client_active");
            PastButton.Attributes.Add("client_active", "true");
        }
    }

    private void BindRepeater()
    {

        using (var con = new DataConnection())
        {
            string liveVersionName;
            Guid? liveVersionId;
            using (var con2 = new DataConnection(PublicationScope.Published))
            {
                con2.AddService(VersioningServiceSettings.Published());
                var published = con2.Get<IPage>().FirstOrDefault(page => page.Id == _pageId);
                liveVersionName = published == null ? null : published.LocalizedVersionName();
                liveVersionId = published != null ? published.VersionId : (Guid?)null;
            }

            var totalVersionCount = con.QueryPageVersions(p => p.Id == _pageId).Count();

            con.AddService(PublicationStatus == "Active"
                ? VersioningServiceSettings.Active() : VersioningServiceSettings.Past());

            Func<DateTime?, string> sortableFormat = date => date == null ? "" : date.Value.ToString("s");

            var allVersions = from pv in con.QueryPageVersions(p => p.Id == _pageId).ToList()
                              let page = pv.Page
                              select
                                  new VersionViewModel
                                  {
                                      Version = pv.CampaignName ?? LocalizationFiles.Composite_Management.DefaultVersionName,
                                      VersionId = page.VersionId,
                                      Status = GetTransitionName(page, pv, page.VersionId == liveVersionId),
                                      Author = page.ChangedBy,
                                      DateCreated = page.CreationDate.ToTimeZoneDateTimeString(),
                                      DateModified = page.ChangeDate.ToTimeZoneDateTimeString(),
                                      PublishDate = pv.PublishTime.ToTimeZoneDateTimeString(),
                                      UnpublishDate = pv.UnpublishTime.ToTimeZoneDateTimeString(),
                                      SortableDateCreated = sortableFormat(page.CreationDate),
                                      SortableDateModified = sortableFormat(page.ChangeDate),
                                      SortablePublishDate = sortableFormat(pv.PublishTime),
                                      SortableUnpublishDate = sortableFormat(pv.UnpublishTime),
                                      EntityToken = page.GetDataEntityToken()
                                  };

            allVersions = allVersions.ToList();

            foreach (var v in allVersions)
            {
                bool isLiveVersion = v.Version == liveVersionName;
                bool isCurrentlyEditedVersion = v.VersionId.ToString() == PageVersionId;

                v.Live = isLiveVersion;

                // Editing the default version would require changing page data id
                bool disableEditing = isCurrentlyEditedVersion && v.VersionId == Guid.Empty;

                v.ShowEditButton = HasPermission(v.EntityToken, PermissionType.Edit) && !disableEditing;
                v.ShowDeleteButton = HasPermission(v.EntityToken, PermissionType.Delete) 
                                        && !isCurrentlyEditedVersion
                                        && totalVersionCount > 1;
            }

            string sortBy = SortColumn;
            if (sortBy.Contains("Date"))
            {
                sortBy = "Sortable" + sortBy;
            }

            var property = typeof (VersionViewModel).GetProperty(sortBy);

            allVersions = (SortByAscending
                ? allVersions.OrderBy(v => (string)property.GetValue(v))
                : allVersions.OrderByDescending(v => (string)property.GetValue(v)))
                .ToList();


            Versions.Visible = allVersions.Any();
            Versions.DataSource = allVersions;
            Versions.DataBind();
        }
    }

    private string GetTransitionName(IPage p, PageWithCampaignInfo v, bool isLive)
    {
        string statusName;

        if (v != null && v.PublishTime > DateTime.Now && p.PublicationStatus == GenericPublishProcessController.Published)
        {
            statusName = Localization.PublicationSchedule_Status_ScheduledforPublication;
        }
        else
        {
            var statusNames = new Dictionary<string, string>
            {
                {GenericPublishProcessController.Draft, ManagementStrings.PublishingStatus_draft},
                {GenericPublishProcessController.AwaitingApproval, ManagementStrings.PublishingStatus_awaitingApproval},
                {GenericPublishProcessController.AwaitingPublication, ManagementStrings.PublishingStatus_awaitingPublication},
                {GenericPublishProcessController.Published, ManagementStrings.PublishingStatus_published}
            };

            statusName = statusNames[p.PublicationStatus];
        }
        

        if (isLive)
        {
            if (statusName == ManagementStrings.PublishingStatus_published)
            {
                return Localization.PublicationSchedule_Status_Live;
            }
            return string.Format("{0}, {1}", statusName, Localization.PublicationSchedule_Status_Live);
        }

        return statusName;
    }

    private bool HasPermission(EntityToken et, PermissionType t)
    {
        var permissions = PermissionsFacade.GetPermissionsForCurrentUser(et).Evaluate();

        if (!permissions.Any())
        {
            return true;
        }

        return permissions.Contains(t);
    }

    protected void rptUserData_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Header)
        {
            foreach (var linkButton in e.Item.Controls.OfType<LinkButton>())
            {
                if (linkButton.CommandName == SortColumn)
                {
                    linkButton.Attributes["direction"] = SortByAscending ? "asc" : "desc";
                }
            }
        }
    }

    protected void rptUserData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "")
        {
            if (e.CommandName == SortColumn)
            {
                SortByAscending = !SortByAscending;
            }
            else
            {
                SortColumn = e.CommandName;
                SortByAscending = true;
            }

            BindRepeater();
        }
    }

    public void EditButton_Click(object sender, EventArgs args)
    {
        using (var con = new DataConnection())
        {
            var btn = (ToolbarButton)sender;
            var versionId = Guid.Parse(btn.CommandArgument);

            var page = con.Get<IPage>().First(p => p.Id == _pageId && p.VersionId == versionId);

            var editVersionActionToken = new WorkflowActionToken(typeof (EditVersionWorkflow), new[] {PermissionType.Edit, PermissionType.Publish})
            {
                DoIgnoreEntityTokenLocking = true
            };

            ExecutePageAction(page, editVersionActionToken);
        }
    }

    public void DeleteButton_Click(object sender, EventArgs args)
    {
        using (var con = new DataConnection())
        {
            var btn = (ToolbarButton)sender;

            var versionId = Guid.Parse(btn.CommandArgument);
            var page = con.Get<IPage>().First(p => p.Id == _pageId && p.VersionId == versionId);

            var deleteActionToken = StartupHandler.GetCustomDeleteAction();

            ExecutePageAction(page, deleteActionToken);
        }
    }

    public void AddVersionButton_Click(object sender, EventArgs args)
    {
        using (var con = new DataConnection())
        {
            var page = con.Get<IPage>().First(f => f.Id == _pageId);
            var addActionToken = new WorkflowActionToken(typeof(AddNewPageVersionWorkflow))
            {
                DoIgnoreEntityTokenLocking = true
            };

            ExecutePageAction(page, addActionToken);
        }
    }

    private void ExecutePageAction(IPage page, ActionToken actionToken)
    {
        var serializedEntityToken = EntityTokenSerializer.Serialize(page.GetDataEntityToken(), true);
        var serializedAction = ActionTokenSerializer.Serialize(actionToken, true);
        var providerName = actionToken.GetType().Namespace;
        TreeServicesFacade.ExecuteElementAction(providerName, serializedEntityToken, null, serializedAction, ConsoleId);
    }


    public void PastButton_Click(object sender, EventArgs args)
    {
        PublicationStatus = "Past";
        BindRepeater();
    }

    public void ActiveButton_Click(object sender, EventArgs args)
    {
        PublicationStatus = "Active";
        BindRepeater();
    }
}

public class VersionViewModel
{
    public Guid VersionId { get; set; }
    public EntityToken EntityToken { get; set; }

    public bool Live { get; set; }
    public string Version { get; set; }

    public string Status { get; set; }
    public string Author { get; set; }

    public string DateCreated { get; set; }
    public string DateModified { get; set; }
    public string PublishDate { get; set; }
    public string UnpublishDate { get; set; }

    public string SortableDateCreated { get; set; }
    public string SortableDateModified { get; set; }
    public string SortablePublishDate { get; set; }
    public string SortableUnpublishDate { get; set; }

    public bool ShowEditButton { get; set; }
    public bool ShowDeleteButton { get; set; }
}