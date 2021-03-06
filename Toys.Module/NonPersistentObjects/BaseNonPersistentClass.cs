using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Data;
namespace Toys.Module.BusinessObjects
{
    public abstract class BaseNonPersistentClass : INotifyPropertyChanged, IObjectSpaceLink
    {
        protected static int idCounter = 4;
        private Int32 id;
        private IObjectSpace objectSpace;
        private String name;

        protected virtual void RaisePropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public BaseNonPersistentClass(Int32 id, String name)
            : base()
        {
            this.id = id;
            this.name = name;

        }
        public BaseNonPersistentClass() : this(idCounter++, "") { }
        [Key]
        [Browsable(false)]
        public Int32 ID => id;
        public String Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        public virtual BaseNonPersistentClass Clone(IObjectMap map)
        {
            var clone = (BaseNonPersistentClass)Activator.CreateInstance(this.GetType(), this.ID, this.Name);
            map.AcceptObject(clone);
            return clone;
        }
        // IObjectSpaceLink
        [Browsable(false)]
        public IObjectSpace ObjectSpace
        {
            get => objectSpace;
            set => objectSpace = value;
        }
        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
    }
}