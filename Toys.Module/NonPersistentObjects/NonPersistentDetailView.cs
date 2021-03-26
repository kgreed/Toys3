using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using Toys.Module.BusinessObjects;
namespace Toys.Module.NonPersistentObjects
{
    public partial class NonPersistentDetailView : ObjectViewController<DetailView, BaseNonPersistent>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CustomizeViewItemControl(this, SetViewItem);
        }

        private void SetViewItem(ViewItem viewItem)
        {
            var ed = viewItem as PropertyEditor;
            ed.ValueStored += Ed_ValueStored;
        }

        private void Ed_ValueStored(object sender, EventArgs e)
        {
           View.ObjectSpace.SetModified(View.CurrentObject);
        }
    }
}