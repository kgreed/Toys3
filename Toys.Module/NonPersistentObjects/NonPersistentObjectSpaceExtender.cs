using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
namespace Toys.Module.BusinessObjects
{
    public class NonPersistentObjectSpaceExtender : IObjectMap
    {
        private NonPersistentObjectSpace objectSpace;
        private GlobalObjectStorage globalObjects;
        private Dictionary<int, BaseNonPersistentClass> localObjects;

        object IObjectMap.GetObject(object obj)
        {
            return objectSpace.GetObject(obj);
        }
        void IObjectMap.AcceptObject(object obj)
        {
            BaseNonPersistentClass keyobj = obj as BaseNonPersistentClass;
            if (keyobj != null)
            {
                localObjects.Add(keyobj.ID, keyobj);
            }
        }
        private BaseNonPersistentClass GetObject(BaseNonPersistentClass obj)
        {
            return !objectSpace.IsNewObject(obj) ? GetObjectByKey(obj.ID) : obj;
        }
        private BaseNonPersistentClass GetObjectByKey(int key)
        {
            BaseNonPersistentClass obj;
            if (!localObjects.TryGetValue(key, out obj))
            {
                obj = LoadObject(key);
            }
            return obj;
        }
        private BaseNonPersistentClass LoadObject(int key)
        {
            BaseNonPersistentClass obj = globalObjects.FindObject(key);
            if (obj != null)
            {
                BaseNonPersistentClass clone = (BaseNonPersistentClass)obj.Clone(this);
                clone.ObjectSpace = objectSpace;
                return clone;
            }
            return null;
        }
        private void ObjectSpace_ObjectsGetting(Object sender, ObjectsGettingEventArgs e)
        {
            if (!typeof(BaseNonPersistentClass).IsAssignableFrom(e.ObjectType)) return;
            BindingList<BaseNonPersistentClass> objects = new BindingList<BaseNonPersistentClass>
            {
                AllowNew = true, AllowEdit = true, AllowRemove = true
            };
            foreach (BaseNonPersistentClass obj in globalObjects.Objects)
            {
                if (e.ObjectType.IsInstanceOfType(obj))
                {
                    objects.Add(GetObject(obj));
                }
            }
            e.Objects = objects;
        }
        private void ObjectSpace_ObjectByKeyGetting(Object sender, ObjectByKeyGettingEventArgs e)
        {
            if (typeof(BaseNonPersistentClass).IsAssignableFrom(e.ObjectType) && e.Key is Int32)
            {
                e.Object = GetObjectByKey((Int32)e.Key);
            }
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject is BaseNonPersistentClass obj)
            {
                e.TargetObject = GetObject(obj);
            }
        }
        private void ObjectSpace_Committing(Object sender, CancelEventArgs e)
        {
            NonPersistentObjectSpace objectSpace = (NonPersistentObjectSpace)sender;
            foreach (Object obj in objectSpace.ModifiedObjects)
            {
                if (!(obj is BaseNonPersistentClass baseobj)) continue;
                if (objectSpace.IsDeletedObject(baseobj))
                {
                    globalObjects.DeleteObject(baseobj);
                }
                else
                {
                    globalObjects.SaveObject(baseobj);
                }
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            localObjects.Clear();
        }
        private void ObjectSpace_Disposed(object sender, EventArgs e)
        {
            NonPersistentObjectSpace objectSpace = sender as NonPersistentObjectSpace;
            objectSpace.ObjectsGetting -= ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting -= ObjectSpace_ObjectByKeyGetting;
            objectSpace.ObjectGetting -= ObjectSpace_ObjectGetting;
            objectSpace.Committing -= ObjectSpace_Committing;
            objectSpace.Reloaded -= ObjectSpace_Reloaded;
        }
        public NonPersistentObjectSpaceExtender(NonPersistentObjectSpace objectSpace, GlobalObjectStorage globalObjects)
        {
            this.objectSpace = objectSpace;
            this.globalObjects = globalObjects;
            this.localObjects = new Dictionary<int, BaseNonPersistentClass>();
            if (objectSpace == null) return;
            objectSpace.Committing += ObjectSpace_Committing;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.Disposed += ObjectSpace_Disposed;
        }
    }
}