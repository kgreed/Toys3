using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Data;
namespace Toys.Module.BusinessObjects
{
    public abstract class BaseNonPersistent : INotifyPropertyChanged, IObjectSpaceLink
    {
        private static int _idCounter = 4;
        private readonly int _id;
        private IObjectSpace objectSpace;
        private string _name;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected BaseNonPersistent(int id, string name)
            : base()
        {
            this._id = id;
            this._name = name;

        }

        protected BaseNonPersistent() : this(_idCounter++, "") { }
        [Key]
        [Browsable(false)]
        public int ID => _id;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        public virtual BaseNonPersistent Clone(IObjectMap map)
        {
            var clone = (BaseNonPersistent)Activator.CreateInstance(this.GetType(), this.ID, this.Name);
            map.AcceptObject(clone);
            return clone;
        }

        public virtual void NPOnSaving(IObjectSpace objObjectSpace)
        {
            throw new Exception("This should be overriden");
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