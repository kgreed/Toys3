using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using Toys.Module.BusinessObjects;
namespace Toys.Module.NonPersistentObjects
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class NonPersistantViewController : ViewController
    {
        public NonPersistantViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(BaseNonPersistent);
            TargetViewType = ViewType.DetailView;
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            
            base.OnActivated();
            var dv = ((DetailView) View);
            dv.Closed += Dv_Closed;

            
            // Perform various tasks depending on the target View.
        }

        private void Dv_Closed(object sender, EventArgs e)
        {
            Debug.Print("hi"); // is there anything I can put here to refresh the record in the list view?
        }

        
        private void View_SelectionChanged(object sender, EventArgs e)
        {
            var v = sender as ListView;
           // v.Refresh();

        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            var dv = ((DetailView)View);
            dv.Closed -= Dv_Closed;
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
