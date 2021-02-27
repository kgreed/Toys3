using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
namespace Toys.Module
{
    class TransientNonPersistentObjectAdapter
    {
        private NonPersistentObjectSpace objectSpace;
        private NonPersistentObjectFactoryBase factory;
        private ObjectMap objectMap;
        public TransientNonPersistentObjectAdapter(NonPersistentObjectSpace objectSpace, ObjectMap objectMap, NonPersistentObjectFactoryBase factory)
        {
            this.objectSpace = objectSpace;
            this.factory = factory;
            this.objectMap = objectMap;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.CustomCommitChanges += ObjectSpace_CustomCommitChanges;
            objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
        }
        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject != null && objectMap.IsKnown(e.SourceObject.GetType()))
            {
                if (IsNewObject(e.SourceObject))
                {
                    e.TargetObject = null;
                }
                else
                {
                    var key = objectSpace.GetKeyValue(e.SourceObject);
                    e.TargetObject = factory.GetObjectByKey(e.SourceObject.GetType(), key);
                }
            }
        }
        private void ObjectSpace_CustomCommitChanges(object sender, HandledEventArgs e)
        {
            var toSave = objectSpace.GetObjectsToSave(false);
            var toInsert = new List<object>();
            var toUpdate = new List<object>();
            foreach (var obj in toSave)
            {
                if (objectSpace.IsNewObject(obj))
                {
                    toInsert.Add(obj);
                }
                else
                {
                    toUpdate.Add(obj);
                }
            }
            var toDelete = objectSpace.GetObjectsToDelete(false);
            if (toInsert.Count != 0 || toUpdate.Count != 0 || toDelete.Count != 0)
            {
                factory.SaveObjects(toInsert, toUpdate, toDelete,objectSpace);
            }
            //e.Handled = false;// !!!
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e)
        {
            if (e.Key != null && objectMap.IsKnown(e.ObjectType))
            {
                Object obj = objectMap.Get(e.ObjectType, e.Key);
                if (obj == null)
                {
                    obj = factory.GetObjectByKey(e.ObjectType, e.Key);
                    if (obj != null && !objectMap.Contains(obj))
                    {
                        objectMap.Add(e.ObjectType, e.Key, obj);
                    }
                }
                if (obj != null)
                {
                    e.Object = obj;
                }
            }
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject != null && objectMap.IsKnown(e.SourceObject.GetType()))
            {
                var link = (IObjectSpaceLink)e.SourceObject;
                if (objectSpace.Equals(link.ObjectSpace) && (objectMap.Contains(e.SourceObject) || IsNewObject(e.SourceObject)))
                {
                    e.TargetObject = e.SourceObject;
                }
                else
                {
                    var key = objectSpace.GetKeyValue(e.SourceObject);
                    e.TargetObject = objectSpace.GetObjectByKey(e.SourceObject.GetType(), key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e)
        {
            if (objectMap.IsKnown(e.ObjectType))
            {
                var collection = new DynamicCollection(objectSpace, e.ObjectType, e.Criteria, e.Sorting, e.InTransaction);
                collection.FetchObjects += DynamicCollection_FetchObjects;
                e.Objects = collection;
            }
        }
        private static bool IsNewObject(object obj)
        {
            var sourceObjectSpace = BaseObjectSpace.FindObjectSpaceByObject(obj);
            return sourceObjectSpace == null ? false : sourceObjectSpace.IsNewObject(obj);
        }
        private IEnumerable GetList(Type objectType, CriteriaOperator criteria, IList<DevExpress.Xpo.SortProperty> sorting)
        {
            return factory.GetObjects(objectType, criteria, sorting);
        }
        private void DynamicCollection_FetchObjects(object sender, FetchObjectsEventArgs e)
        {
            e.Objects = GetList(e.ObjectType, e.Criteria, e.Sorting);
            e.ShapeData = true;
        }
    }
}