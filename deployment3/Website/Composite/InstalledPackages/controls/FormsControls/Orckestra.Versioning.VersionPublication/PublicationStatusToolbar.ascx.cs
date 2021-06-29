using System;
using Composite.C1Console.Forms;
using Composite.Plugins.Forms.WebChannel.UiControlFactories;


    public partial class Composite_Plugins_Forms_WebChannel_UiControlFactories_ToolbarButtonTemplateUserControlBase :
        UserControlBasedUiControl
    {
        [BindableProperty()]
        [FormsProperty()]
        public Guid PageIdStatus { get; set; }

        [BindableProperty()]
        [FormsProperty()]
        public string PageVersionId { get; set; }

        public string PageIdStatusString;

        [BindableProperty()]
        [FormsProperty()]
        public bool ShowPublishAsScheduled { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            PageIdStatusString = PageIdStatus.ToString();
            this.DataBind();
        }

        public override void BindStateToControlProperties()
        {


        }

        public override void InitializeViewState()
        {

        }

        public string GetSaveAndPublishTitle()
        {
            if (ShowPublishAsScheduled)
            {
                return "${string:Orckestra.Versioning.VersionPublication:SaveAndPublishAsScheduledAction.Label}" ;
            }
            return null;
        }
    }
