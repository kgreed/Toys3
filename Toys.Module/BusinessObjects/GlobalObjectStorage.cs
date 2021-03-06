using System.Collections.Generic;
namespace Toys.Module.BusinessObjects
{
    public class GlobalObjectStorage : IObjectMap
    {
        private List<BaseNonPersistentClass> objects = new List<BaseNonPersistentClass>();
        public IEnumerable<BaseNonPersistentClass> Objects { get { return objects.AsReadOnly(); } }
        public void Add(BaseNonPersistentClass obj)
        {
            objects.Add(obj);
        }
        public BaseNonPersistentClass FindObject(int key)
        {
            foreach (BaseNonPersistentClass obj in objects)
            {
                if (obj.ID == key)
                {
                    return obj;
                }
            }
            return null;
        }
        public void SaveObject(BaseNonPersistentClass obj)
        {
            BaseNonPersistentClass found = FindObject(obj.ID);
            if (found != null)
            {
                objects.Remove(found);
            }
            objects.Add(obj.Clone(this));
        }
        public void DeleteObject(BaseNonPersistentClass obj)
        {
            BaseNonPersistentClass found = FindObject(obj.ID);
            if (found != null)
            {
                objects.Remove(found);
            }
        }
        object IObjectMap.GetObject(object obj)
        {
            BaseNonPersistentClass keyobj = obj as BaseNonPersistentClass;
            if (keyobj != null)
            {
                return FindObject(keyobj.ID);
            }
            else
            {
                return obj;
            }
        }
        void IObjectMap.AcceptObject(object obj)
        {
        }
    }
}