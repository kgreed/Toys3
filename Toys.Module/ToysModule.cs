using System;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using System.Data.Entity;
using System.Diagnostics;
using DevExpress.Persistent.BaseImpl;
using Toys.Module.BusinessObjects;

namespace Toys.Module {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    public sealed partial class ToysModule : ModuleBase {
        static ToysModule() {
            DevExpress.Data.Linq.CriteriaToEFExpressionConverter.SqlFunctionsType = typeof(System.Data.Entity.SqlServer.SqlFunctions);
			DevExpress.Data.Linq.CriteriaToEFExpressionConverter.EntityFunctionsType = typeof(System.Data.Entity.DbFunctions);
			DevExpress.ExpressApp.SystemModule.ResetViewSettingsController.DefaultAllowRecreateView = false;
            // Uncomment this code to delete and recreate the database each time the data model has changed.
            // Do not use this code in a production environment to avoid data loss.
            // #if DEBUG
            // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ToysDbContext>());
            // #endif 
        }
        private NonPersistentObjectSpaceHelper nonPersistentObjectSpaceHelper;
        public ToysModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.ViewCreating += Application_ViewCreating;
            application.SetupComplete += Application_SetupComplete;
            // Manage various aspects of the application UI and behavior at the module level.
        }
        private void Application_SetupComplete(object sender, EventArgs e)
        {
            nonPersistentObjectSpaceHelper = new NonPersistentObjectSpaceHelper((XafApplication)sender, typeof(BaseObject));
            nonPersistentObjectSpaceHelper.AdapterCreators.Add(npos => {
                var types = new Type[] { typeof(NPToy)  };
                var map = new ObjectMap(npos, types);
                new TransientNonPersistentObjectAdapter(npos, map, new NPFactory(map));
            });
        }
        private void Application_ViewCreating(object sender, ViewCreatingEventArgs e)
        {
            //if (e.ViewID == "NPToy_DetailView")
            //{

            //}

            //var detailView = e.View as DetailView;
            //if (detailView != null)
            //{
            //    Debug.Print(detailView.Caption);
            //    //To avoid assigning multiple adapters to the same Object Space, you can create a type-specific adapter in the XafApplication.ViewCreating event handler. Use the e.ViewID parameter to determine which adapter is required.
            //}

            //Debug.Print("hi");
        }
    }
}

