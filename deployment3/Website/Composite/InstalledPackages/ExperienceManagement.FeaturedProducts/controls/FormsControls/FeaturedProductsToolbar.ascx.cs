using System;
using Composite.C1Console.Forms;
using Composite.Plugins.Forms.WebChannel.UiControlFactories;


    public partial class Composite_Plugins_Forms_WebChannel_UiControlFactories_ToolbarButtonTemplateUserControlBase :
        UserControlBasedUiControl
    {
        [BindableProperty()]
        [FormsProperty()]
        public string Path { get; set; }

        public override void BindStateToControlProperties()
        {
            this.DataBind();
        }

        public override void InitializeViewState()
        {

        }

    }
