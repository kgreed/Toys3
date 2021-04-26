using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using DevExpress.Data.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using Toys.Module.BusinessObjects;
using Updater = Toys.Module.DatabaseUpdate.Updater;
namespace Toys.Module
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    public sealed partial class ToysModule : ModuleBase
    {
        private NonPersistentObjectSpaceHelper nonPersistentObjectSpaceHelper;

        static ToysModule()
        {
            CriteriaToEFExpressionConverter.SqlFunctionsType = typeof(SqlFunctions);
            CriteriaToEFExpressionConverter.EntityFunctionsType = typeof(DbFunctions);
            ResetViewSettingsController.DefaultAllowRecreateView = false;
            // Uncomment this code to delete and recreate the database each time the data model has changed.
            // Do not use this code in a production environment to avoid data loss.
            // #if DEBUG
            // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ToysDbContext>());
            // #endif 
        }

        public ToysModule()
        {
            InitializeComponent();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[] {updater};
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.ViewCreating += Application_ViewCreating;
            application.SetupComplete += Application_SetupComplete;
            // Manage various aspects of the application UI and behavior at the module level.
        }

        private void Application_SetupComplete(object sender, EventArgs e)
        {
            nonPersistentObjectSpaceHelper =
                new NonPersistentObjectSpaceHelper((XafApplication) sender, typeof(BaseObject));
            nonPersistentObjectSpaceHelper.AdapterCreators.Add(npos =>
            {
                var types = new[] {typeof(NPToy)};
                var map = new ObjectMap(npos, types);
                new TransientNonPersistentObjectAdapter(npos, map, new NPFactory(map));
            });
        }

        private void Application_ViewCreating(object sender, ViewCreatingEventArgs e)
        {
            //nonPersistentObjectSpaceHelper = new NonPersistentObjectSpaceHelper((XafApplication)sender, typeof(BaseObject));
            //nonPersistentObjectSpaceHelper.AdapterCreators.Add(npos => {
            //    var types = new Type[] { typeof(NPToy) };
            //    var map = new ObjectMap(npos, types);
            //    new TransientNonPersistentObjectAdapter(npos, map, new NPFactory(map));
            //});
        }
    }
}