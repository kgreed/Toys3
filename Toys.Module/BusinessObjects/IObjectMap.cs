namespace Toys.Module.BusinessObjects
{
    public interface IObjectMap
    {
        object GetObject(object obj);
        void AcceptObject(object obj);
    }
}