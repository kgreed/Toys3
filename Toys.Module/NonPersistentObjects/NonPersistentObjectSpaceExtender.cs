using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
namespace Toys.Module.BusinessObjects
{
    public class NonPersistentObjectSpaceExtender : IObjectMap
    {
        private readonly GlobalObjectStorage globalObjects;
        private readonly Dictionary<int, BaseNonPersistent> localObjects;
        private readonly NonPersistentObjectSpace objectSpace;

        public NonPersistentObjectSpaceExtender(NonPersistentObjectSpace objectSpace, GlobalObjectStorage globalObjects)
        {
            this.objectSpace = objectSpace;
            this.globalObjects = globalObjects;
            localObjects = new Dictionary<int, BaseNonPersistent>();
            if (objectSpace == null) return;
            objectSpace.Committing += ObjectSpace_Committing;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.Disposed += ObjectSpace_Disposed;
            objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
        }

        object IObjectMap.GetObject(object obj)
        {
            return objectSpace.GetObject(obj);
        }

        void IObjectMap.AcceptObject(object obj)
        {
            if (obj is BaseNonPersistent keyobj) localObjects.Add(keyobj.ID, keyobj);
        }

        private BaseNonPersistent GetObject(BaseNonPersistent obj)
        {
            return !objectSpace.IsNewObject(obj) ? GetObjectByKey(obj.ID) : obj;
        }

        private BaseNonPersistent GetObjectByKey(int key)
        {
            if (!localObjects.TryGetValue(key, out var obj)) obj = LoadObject(key);
            return obj;
        }

        private BaseNonPersistent LoadObject(int key)
        {
            var obj = globalObjects.FindObject(key);
            if (obj == null) return null;
            var clone = obj.Clone(this);
            clone.ObjectSpace = objectSpace;
            return clone;
        }

        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e)
        {
            var collection = new DynamicCollection((IObjectSpace) sender, e.ObjectType, e.Criteria, e.Sorting,
                e.InTransaction);
            collection.FetchObjects += DynamicCollection_FetchObjects;
            e.Objects = collection;
        }

        private void DynamicCollection_FetchObjects(object sender, FetchObjectsEventArgs e)
        {
            if (!typeof(BaseNonPersistent).IsAssignableFrom(e.ObjectType)) return;
            var tempBo = Activator.CreateInstance(e.ObjectType);
            var dc = (DynamicCollection) sender;
            var os = dc.ObjectSpace;
            var npos = (NonPersistentObjectSpace) os;
            var lv = (ListView) npos.Owner;
            var viewTag = lv.Tag as ViewTag;
            var data = ((BaseNonPersistent) tempBo).NPGetData(viewTag);
            globalObjects.ObjectsInit(data);
            var objects = new BindingList<BaseNonPersistent>
            {
                AllowNew = false,
                AllowEdit = true,
                AllowRemove = false
            };
            foreach (var obj in globalObjects.Objects)
                if (e.ObjectType.IsInstanceOfType(obj))
                    objects.Add(GetObject(obj));
            e.Objects = objects;
            e.ShapeData = true;
        }

        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e)
        {
            if (typeof(BaseNonPersistent).IsAssignableFrom(e.ObjectType) && e.Key is int)
                e.Object = GetObjectByKey((int) e.Key);
        }

        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject is BaseNonPersistent obj) e.TargetObject = GetObject(obj);
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            var objectSpace = (NonPersistentObjectSpace) sender;
            foreach (var obj in objectSpace.ModifiedObjects)
            {
                if (!(obj is BaseNonPersistent baseobj)) continue;
                if (objectSpace.IsDeletedObject(baseobj))
                    globalObjects.DeleteObject(baseobj);
                else
                    globalObjects.SaveObject(baseobj);
            }
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            localObjects.Clear();
        }

        private void ObjectSpace_Disposed(object sender, EventArgs e)
        {
            var objectSpace = sender as NonPersistentObjectSpace;
            objectSpace.ObjectsGetting -= ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting -= ObjectSpace_ObjectByKeyGetting;
            objectSpace.ObjectGetting -= ObjectSpace_ObjectGetting;
            objectSpace.Committing -= ObjectSpace_Committing;
            objectSpace.Reloaded -= ObjectSpace_Reloaded;
        }

        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e)
        {
            if (!(e.SourceObject is BaseNonPersistent obj)) return;
            e.TargetObject = null;
            if (obj.ObjectSpace.IsNewObject(obj)) return;
            localObjects.Remove(obj.ID);
            var storedObj = globalObjects.FindObject(obj.ID);
            if (storedObj == null) return;
            storedObj.CopyTo(obj, this);
            e.TargetObject = obj;
        }
    }
}