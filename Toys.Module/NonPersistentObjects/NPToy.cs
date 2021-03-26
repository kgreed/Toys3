using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.XtraScheduler.Outlook.Interop;
using Toys.Module.DTO;
using Exception = System.Exception;


namespace Toys.Module.BusinessObjects
{
    [DomainComponent]
    [DefaultClassOptions]
    [NavigationItem("1 Main")]
    [ListViewFilter("Toy type 1", "false", "Toy type 1", false, Index = 1)]
    [ListViewFilter("Toy type 2", "true", "Toy type 2", false, Index = 2)]
    public class NPToy :BaseNonPersistent  
    {
       

        private string _name;
        [ImmediatePostData]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                base.RaisePropertyChanged(nameof(Name));
            }
        }

        public string Info { get; set; }

        //[Browsable(false)]
        private int ToyCategory { get; set; }
        [ImmediatePostData] public ToyCategoryEnum ToyCategoryNum { get =>(ToyCategoryEnum) ToyCategory;
            set
            {
                ToyCategory = (int) value;
                base.RaisePropertyChanged(nameof(ToyCategoryNum));
            }  
        }

       
        private int _BrandId;
        [Browsable(false)]
        [ModelDefault("AllowEdit", "False")]
        public int BrandId {
            get => _BrandId;
            set
            {
                _BrandId = value;
                base.RaisePropertyChanged();
                // OnPropertyChanged();
            }
        }

        private BabyToy _babyToy;
        [Appearance("IsBabyToy", Visibility = ViewItemVisibility.Hide, Criteria =  "[ToyCategoryNum] != 1" )]
        [VisibleInListView(false)]
        [NotMapped]
        public BabyToy BabyToy {
            get => ToyCategoryNum != ToyCategoryEnum.Baby ? null : _babyToy;
            set => _babyToy = value;
        }

        private PreSchoolToy _preSchoolToy;

        [Appearance("IsPreSchoolToy", Visibility = ViewItemVisibility.Hide, Criteria = "[ToyCategoryNum] != 3")]
        [VisibleInListView(false)]
        [NotMapped]
        public PreSchoolToy PreSchoolToy
        {
            get => ToyCategoryNum != ToyCategoryEnum.PreSchool? null : _preSchoolToy;
            set => _preSchoolToy = value;
        }

        private ToddlerToy _toddlerToy;
        [Appearance("IsToddlerToy", Visibility = ViewItemVisibility.Hide, Criteria = "[ToyCategoryNum] != 2")]
        [VisibleInListView(false)]
        [NotMapped]
        public ToddlerToy ToddlerToy
        {
            get => ToyCategoryNum != ToyCategoryEnum.Toddler ? null : _toddlerToy;
            set => _toddlerToy = value;
        }

        [NotMapped]

        IObjectSpace persistentObjectSpace => ((NonPersistentObjectSpace)ObjectSpace)?.AdditionalObjectSpaces?.FirstOrDefault();

        [DataSourceProperty("Brands")]
        [NotMapped]
        [ImmediatePostData]
        public virtual Brand Brand
        {
            get => persistentObjectSpace?.GetObjectByKey<Brand>(BrandId);
            set
            {
                BrandId = value.Id;
                //  OnPropertyChanged();
                base.RaisePropertyChanged();
            }
        }

        [Browsable(false)] public IList<Brand> Brands => persistentObjectSpace?.GetObjects<Brand>(CriteriaOperator.Parse("[Id] > 0"));

     

        [Browsable(false)]
        public string SearchText { get; set; }
        public List<NPToy> GetData(ViewTag tag)
        {
            // todo make use of tag to set filter
            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var connect = new ToysDbContext(connectionString))
            {
                var parameters = new List<SqlParameter>();
                var sql = "select t.Id as ToyId, t.Name, t.Info,b.Id as BrandId, t.ToyCategory, b.Name as BrandName from toys t inner join brands b on t.Brand_Id = b.Id";
            
                if (SearchText?.Length > 0)
                {
                    sql += " where t.name like @name";
                    parameters.Add( new SqlParameter("@name",$"%{SearchText}%"));
                }
                var results = connect.Database.SqlQuery<NPToy>(sql,parameters.ToArray()).ToList();
                return results;
                // return results.ConvertAll(x => (BaseNonPersistentClass)x);

            }
        }

        [Browsable(false)]
        public int ToyId { get; set; }
        public override int ID => ToyId;
        public override List<BaseNonPersistent> NPGetData(ViewTag tag)
        {
            var objs = GetData(tag);
            var r = objs.Cast<BaseNonPersistent>().ToList();
            return r;
        }

        public override BaseNonPersistent Clone(IObjectMap map) {
            var clone = (NPToy)Activator.CreateInstance(this.GetType());
            CopyTo(clone, map);
            map.AcceptObject(clone);
            return clone;
        }
        public override void CopyTo(BaseNonPersistent target, IObjectMap map) {
            var tclone = (NPToy)target;
            tclone.ToyId = ToyId;
            tclone.BrandId = BrandId;
            tclone.Name = Name;
            tclone.ToyCategory = ToyCategory;
        }

        public override void NPOnSaving(IObjectSpace np_os)
        {
           
            var os = ((NonPersistentObjectSpace)np_os).AdditionalObjectSpaces.FirstOrDefault();  // why cant I use this instead of passing in as a parameter?
         
            var brand = os.FindObject<Brand>(CriteriaOperator.Parse("[Id] = ?", BrandId));
            if (brand == null)
            {
                throw new Exception($"Category {BrandId} was not found");
            }

            // update persistent data
            var toy = os.FindObject<Toy>(CriteriaOperator.Parse("[Id] = ?", ID));
            toy.Name = Name;
            toy.Brand = brand;
            toy.ToyCategory = ToyCategory;
            os.SetModified(toy);
            os.CommitChanges();

             
           // var npOS = np_os.FindObject<NPToy>(CriteriaOperator.Parse("[Id] = ?", Id)); this will be correct
            

        }

        public void SetModified()
        {
            ObjectSpace.SetModified(this);
        }

        public void OnLoaded()
        {
            var os = ((NonPersistentObjectSpace)ObjectSpace).AdditionalObjectSpaces.FirstOrDefault();

            switch (ToyCategoryNum)
            {
                case ToyCategoryEnum.Baby:
                    BabyToy =FindOrMakeToyInfo<BabyToy>(os);
                    break;
                case ToyCategoryEnum.Toddler:
                    ToddlerToy = FindOrMakeToyInfo<ToddlerToy>(os);
                    break;
                case ToyCategoryEnum.PreSchool:
                    PreSchoolToy = FindOrMakeToyInfo<PreSchoolToy>(os);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private T FindOrMakeToyInfo<T>(IObjectSpace os) where T: IToyType
        {
           var toy = os.FindObject<T>(CriteriaOperator.Parse("[Id]=?", ID));
            if (toy != null) return toy;
            toy = os.CreateObject<T>();
            toy.Id = ID;
            return toy;
        }
    }

    internal interface IToyType
    {
        int Id { get; set; }
    }
}