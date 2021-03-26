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
using Toys.Module.BusinessObjects;


namespace Toys.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class NPOvjextViewController : ObjectViewController<ListView,BaseNonPersistent>
    {
        public NPOvjextViewController()
        {
            InitializeComponent();
        
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private void os_ObjectsGetting(object sender, ObjectsGettingEventArgs e)
        {
            var collection = new DynamicCollection((IObjectSpace)sender, e.ObjectType, e.Criteria, e.Sorting, e.InTransaction);
            collection.FetchObjects += DynamicCollection_FetchObjects;
            e.Objects = collection;
        }
        private void DynamicCollection_FetchObjects(object sender, FetchObjectsEventArgs e)
        {
            // what goes here to use NonPersistentObjectSpaceExtender to call e.Objects = NPCat.GetNPCats().ToList();
            e.ShapeData = true;

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var os = (NonPersistentObjectSpace)ObjectSpace;
            os.ObjectsGetting += os_ObjectsGetting;
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
            var os = (NonPersistentObjectSpace)ObjectSpace;
            os.ObjectsGetting += os_ObjectsGetting;
        }
    }
}
