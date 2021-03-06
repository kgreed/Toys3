using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
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
        public ToysModule() {
            InitializeComponent();
        }
        private GlobalObjectStorage globalNonPersistentObjects;
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater }; 
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);

            CreateGlobalNonPersistentObjects();
            Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
            // Manage various aspects of the application UI and behavior at the module level.
        }
        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            if (!(e.ObjectSpace is NonPersistentObjectSpace nonPersistentObjectSpace)) return;
            nonPersistentObjectSpace.AdditionalObjectSpaces.Add(Application.CreateObjectSpace(typeof(BaseObject)));
            nonPersistentObjectSpace.AutoDisposeAdditionalObjectSpaces = true;
            nonPersistentObjectSpace.AutoRefreshAdditionalObjectSpaces = true;
            nonPersistentObjectSpace.ModifiedChanging += NonPersistentObjectSpace_ModifiedChanging;
            new NonPersistentObjectSpaceExtender(nonPersistentObjectSpace, globalNonPersistentObjects);
        }
        private void NonPersistentObjectSpace_ModifiedChanging(object sender, ObjectSpaceModificationEventArgs e)
        {
            if (e.Object is BaseNonPersistentClass)
            {
                e.Cancel = false;
            }
        }
        private void CreateGlobalNonPersistentObjects()
        {
            globalNonPersistentObjects = new GlobalObjectStorage();
            var npToy = new NPToy(0,"");
            var objects = npToy.GetData();
            foreach (NPToy obj in objects)
            {
                globalNonPersistentObjects.Add(obj);
            }

            //SimpleNonPersistentClass simpleNonPersistentClass = new SimpleNonPersistentClass(0, "Simple Non-Persistent Object");
            //NonPersistentClassWithNonPersistentCollection nonPersistentClassWithNonPersistentCollection = new NonPersistentClassWithNonPersistentCollection(1, "Non-Persistent Object with a Non-Persistent Collection");
            //NonPersistentClassWithPersistentProperty nonPersistentClassWithPersistentProperty = new NonPersistentClassWithPersistentProperty(2, "First Non-Persistent Object with a Persistent Property");
            //NonPersistentClassWithPersistentProperty nonPersistentClassWithPersistentProperty1 = new NonPersistentClassWithPersistentProperty(3, "Second Non-Persistent Object with a Persistent Property");

            //globalNonPersistentObjects.Add(simpleNonPersistentClass);
            //globalNonPersistentObjects.Add(nonPersistentClassWithNonPersistentCollection);
            //globalNonPersistentObjects.Add(nonPersistentClassWithPersistentProperty);
            //globalNonPersistentObjects.Add(nonPersistentClassWithPersistentProperty1);

            //nonPersistentClassWithNonPersistentCollection.AddNonPersistentClassWithPersistentProperty(nonPersistentClassWithPersistentProperty);
            //nonPersistentClassWithNonPersistentCollection.AddNonPersistentClassWithPersistentProperty(nonPersistentClassWithPersistentProperty1);
        }
    }
}
