using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Data;
namespace Toys.Module.BusinessObjects
{
    public abstract class BaseNonPersistent : INotifyPropertyChanged, IObjectSpaceLink
    { 
        private IObjectSpace objectSpace;
       

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        [Browsable(false)]
        public virtual Int32 ID => throw new Exception("This needs to be overriden");
        public abstract List<BaseNonPersistent> NPGetData(ViewTag tag);
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