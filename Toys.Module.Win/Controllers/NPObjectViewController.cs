using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Win.Editors;
using Toys.Module.BusinessObjects;


namespace Toys.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class NPObjectViewController : ObjectViewController<ListView,BaseNonPersistent>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            var filterController = Frame.GetController<FilterController>();
            filterController.SetFilterAction.SelectedItemChanged += SetFilterAction_SelectedItemChanged;
            View.ControlsCreated += View_ControlsCreated;
        }

        private void SetFilterAction_SelectedItemChanged(object sender, EventArgs e)
        {
            var filterController = Frame.GetController<FilterController>();
            View.Tag = new ViewTag
            {
                FilterChoice = filterController.SetFilterAction.SelectedItem,
                SearchString = ""
            }; // not sure how to get the search string
        }

        private void View_ControlsCreated(object sender, EventArgs e)
        {
            var listView = View;
            if (!(listView?.Editor is GridListEditor editor)) return;
            editor.GridView.OptionsFind.AlwaysVisible = true;
        }

        protected override void OnDeactivated()
        {
            var filterController = Frame.GetController<FilterController>();
            filterController.SetFilterAction.SelectedItemChanged -= SetFilterAction_SelectedItemChanged;
            base.OnDeactivated();
            View.ControlsCreated -= View_ControlsCreated;
        }
    }
}
