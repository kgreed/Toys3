﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
namespace Toys.Module.BusinessObjects
{
    public class NonPersistentObjectSpaceExtender : IObjectMap
    {
        private NonPersistentObjectSpace objectSpace;
        private GlobalObjectStorage globalObjects;
        private Dictionary<int, BaseNonPersistent> localObjects;

        object IObjectMap.GetObject(object obj)
        {
            return objectSpace.GetObject(obj);
        }
        void IObjectMap.AcceptObject(object obj)
        {
            if (obj is BaseNonPersistent keyobj)
            {
                localObjects.Add(keyobj.ID, keyobj);
            }
        }
        private BaseNonPersistent GetObject(BaseNonPersistent obj)
        {
            return !objectSpace.IsNewObject(obj) ? GetObjectByKey(obj.ID) : obj;
        }
        private BaseNonPersistent GetObjectByKey(int key)
        {
            if (!localObjects.TryGetValue(key, out var obj))
            {
                obj = LoadObject(key);
            }
            return obj;
        }
        private BaseNonPersistent LoadObject(int key)
        {
            var obj = globalObjects.FindObject(key);
            if (obj == null) return null;
            var clone = (BaseNonPersistent)obj.Clone(this);
            clone.ObjectSpace = objectSpace;
            return clone;
        }
        private void ObjectSpace_ObjectsGetting(Object sender, ObjectsGettingEventArgs e)
        {
            if (!typeof(BaseNonPersistent).IsAssignableFrom(e.ObjectType)) return;
            var objects = new BindingList<BaseNonPersistent>
            {
                AllowNew = false, AllowEdit = true, AllowRemove = false
            };
            foreach (var obj in globalObjects.Objects)
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
            if (typeof(BaseNonPersistent).IsAssignableFrom(e.ObjectType) && e.Key is Int32)
            {
                e.Object = GetObjectByKey((Int32)e.Key);
            }
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject is BaseNonPersistent obj)
            {
                e.TargetObject = GetObject(obj);
            }
        }
        private void ObjectSpace_Committing(Object sender, CancelEventArgs e)
        {
            var objectSpace = (NonPersistentObjectSpace)sender;
            foreach (var obj in objectSpace.ModifiedObjects)
            {
                if (!(obj is BaseNonPersistent baseobj)) continue;
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
            var objectSpace = sender as NonPersistentObjectSpace;
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
            this.localObjects = new Dictionary<int, BaseNonPersistent>();
            if (objectSpace == null) return;
            objectSpace.Committing += ObjectSpace_Committing;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.Disposed += ObjectSpace_Disposed;
            objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
        }

        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e) {
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