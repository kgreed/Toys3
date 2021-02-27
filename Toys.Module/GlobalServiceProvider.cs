using System;
namespace Toys.Module
{
    public class GlobalServiceProvider<T>
    {
        private static Func<T> factory;
        private static T instance;
        public static void AddService(Func<T> factory)
        {
            GlobalServiceProvider<T>.factory = factory;
        }
        public static T GetService()
        {
            typeof(T).TypeInitializer.Invoke(null, null);
            if (Equals(instance, default(T)))
            {
                instance = factory.Invoke();
            }
            return instance;
        }
    }
}