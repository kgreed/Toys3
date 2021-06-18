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
using Toys.Module.BusinessObjects;
namespace Toys.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
     
        public partial class AutoRefreshListViewController : ObjectViewController<ListView, BaseNonPersistent>
        {
            protected override void OnActivated()
            {
                base.OnActivated();
                ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            }

            protected override void OnDeactivated()
            {
                ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
                base.OnDeactivated();
            }

            private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
            {
                GridListEditor editor = View.Editor as GridListEditor;
                if (editor != null && editor.GridView != null && editor.GridView.FocusedRowHandle >= 0)
                    editor.GridView.RefreshRow(editor.GridView.FocusedRowHandle);
            }
        }
    
}
