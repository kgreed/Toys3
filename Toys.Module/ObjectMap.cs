using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
namespace Toys.Module
{
    public class ObjectMap
    {
        private Dictionary<Type, Dictionary<Object, Object>> typeMap;
        private NonPersistentObjectSpace objectSpace;
        public ObjectMap(NonPersistentObjectSpace objectSpace, params Type[] types)
        {
            this.objectSpace = objectSpace;
            this.typeMap = new Dictionary<Type, Dictionary<object, object>>();
            foreach (var type in types)
            {
                typeMap.Add(type, new Dictionary<object, object>());
            }
        }
        public bool IsKnown(Type type)
        {
            return typeMap.ContainsKey(type);
        }
        public bool Contains(Object obj)
        {
            Dictionary<Object, Object> objectMap;
            if (typeMap.TryGetValue(obj.GetType(), out objectMap))
            {
                return objectMap.ContainsValue(obj);
            }
            return false;
        }
        public void Clear()
        {
            foreach (var kv in typeMap)
            {
                kv.Value.Clear();
            }
        }
        public T Get<T>(Object key)
        {
            return (T)Get(typeof(T), key);
        }
        public Object Get(Type type, Object key)
        {
            Dictionary<Object, Object> objectMap;
            if (typeMap.TryGetValue(type, out objectMap))
            {
                Object obj;
                if (objectMap.TryGetValue(key, out obj))
                {
                    return obj;
                }
            }
            return null;
        }
        public void Add(Type type, Object key, Object obj)
        {
            Dictionary<Object, Object> objectMap;
            if (typeMap.TryGetValue(type, out objectMap))
            {
                objectMap.Add(key, obj);
            }
        }
        public void Accept(Object obj)
        {
            objectSpace.GetObject(obj);
        }
    }
}