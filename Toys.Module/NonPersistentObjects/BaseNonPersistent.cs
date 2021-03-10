using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Data;
namespace Toys.Module.BusinessObjects
{
    public abstract class BaseNonPersistent : INotifyPropertyChanged, IObjectSpaceLink
    {
        private static int _idCounter = 1;
        protected int _id;
        private IObjectSpace objectSpace;
       

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        protected BaseNonPersistent(int id )
            : base()
        {
            this._id = id;
            

        }
        protected BaseNonPersistent() : this(_idCounter++) { }
        [Key]
        //[Browsable(false)]
        public int ID => _id;

        public virtual BaseNonPersistent Clone(IObjectMap map) {
            throw new Exception("Clone needs to be overriden");
            //var clone = (BaseNonPersistent)Activator.CreateInstance(this.GetType(), this.ID, this.Name);
            //map.AcceptObject(clone);
            //return clone;
        }
        public virtual void CopyTo(BaseNonPersistent target, IObjectMap map)
        {
            throw new Exception("CopyTo needs to be overriden");
        }

        public virtual void NPOnSaving(IObjectSpace objObjectSpace)
        {
            throw new Exception("MPOnSaving needs to be overridden");
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