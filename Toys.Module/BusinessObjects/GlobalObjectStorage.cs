using System.Collections.Generic;
using System.Linq;
namespace Toys.Module.BusinessObjects
{
    public class GlobalObjectStorage : IObjectMap
    {
        private readonly List<BaseNonPersistent> objects = new List<BaseNonPersistent>();
        public IEnumerable<BaseNonPersistent> Objects => objects.AsReadOnly();

        public void Add(BaseNonPersistent obj)
        {
            objects.Add(obj);
        }
        public BaseNonPersistent FindObject(int key)
        {
            return objects.FirstOrDefault(obj => obj.ID == key);
        }
        public void SaveObject(BaseNonPersistent obj)
        {
            var found = FindObject(obj.ID);
            if (found != null)
            {
                objects.Remove(found);
            }
            objects.Add(obj.Clone(this));
            obj.NPOnSaving(obj.ObjectSpace);
        }
        public void DeleteObject(BaseNonPersistent obj)
        {
            var found = FindObject(obj.ID);
            if (found != null)
            {
                objects.Remove(found);
            }
        }
        object IObjectMap.GetObject(object obj)
        {
            if (obj is BaseNonPersistent keyobj)
            {
                return FindObject(keyobj.ID);
            }

            return obj;
        }
        void IObjectMap.AcceptObject(object obj)
        {
        }
    }
}