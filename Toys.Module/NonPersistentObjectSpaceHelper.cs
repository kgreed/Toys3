using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
namespace Toys.Module
{
    public class NonPersistentObjectSpaceHelper : IDisposable
    {
        private readonly XafApplication application;
        private readonly Type[] basePersistentTypes;
        public List<Action<NonPersistentObjectSpace>> AdapterCreators { get; }

        public NonPersistentObjectSpaceHelper(XafApplication application, params Type[] basePersistentTypes)
        {
            this.application = application;
            this.basePersistentTypes = basePersistentTypes;
            AdapterCreators = new List<Action<NonPersistentObjectSpace>>();
            application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
            NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = true;
            NonPersistentObjectSpace.AutoSetModifiedOnObjectChangeByDefault = true;
        }

       

        public void Dispose()
        {
            application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
        }

        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            if (!(e.ObjectSpace is NonPersistentObjectSpace)) return;
            var npos = (NonPersistentObjectSpace) e.ObjectSpace;
            if (basePersistentTypes != null)
                foreach (var type in basePersistentTypes)
                    EnsureObjectSpaceForType(npos, type);
            npos.AutoDisposeAdditionalObjectSpaces = true;
            foreach (var adapterCreator in AdapterCreators) adapterCreator.Invoke(npos);
        }

        private void EnsureObjectSpaceForType(NonPersistentObjectSpace npos, Type type)
        {
            if (npos.IsKnownType(type)) return;
            if (npos.AdditionalObjectSpaces.Any(os => os.IsKnownType(type))) return;
            var persistentObjectSpace = application.CreateObjectSpace(type);
            npos.AdditionalObjectSpaces.Add(persistentObjectSpace);
        }
    }
}