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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
namespace Toys.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ViewControllerToggleRHS : ViewController
    {
        public ViewControllerToggleRHS()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void simpleAction1_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (!(View is ListView lv)) return;
            var savedView = (ListView)Frame.View;
            var caption = savedView.Caption;
            var tr = lv.CurrentObject as IToggleRHS;
            var ed = savedView.Editor as GridListEditor;
            var gv = ed.GridView;
            var rowHandle =  FindRowHandleByRowObject(gv, tr);
            if (!Frame.SetView(null, true, null, false)) return;

            // Make required changes to the related Application Model nodes.
            var defaultMasterDetailMode = MasterDetailMode.ListViewOnly;
            savedView.Model.MasterDetailMode = savedView.Model.MasterDetailMode == defaultMasterDetailMode
                ? MasterDetailMode.ListViewAndDetailView
                : defaultMasterDetailMode;
            // Update the saved View according to the latest model changes and assign it back to the current Frame.
            savedView.LoadModel(false);
            savedView.Caption = caption;
            Frame.SetView(savedView);
            var ed2 = savedView.Editor as GridListEditor;
            var gv2 = ed2.GridView;
            gv2.ClearSelection();
            gv2.SelectRow(rowHandle);
        }

        private int FindRowHandleByRowObject(GridView view, IToggleRHS row)
        {
            if (row == null) return GridControl.InvalidRowHandle;
            for (var i = 0; i < view.DataRowCount; i++)
                if (row.Equals(view.GetRow(i)))
                    return i;
            return GridControl.InvalidRowHandle;
        }
    }
}
