using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
namespace Toys.Module.BusinessObjects
{
    [DomainComponent]
    [DefaultClassOptions]
    [NavigationItem("1 Main")]
    public class
        NPToy : NonPersistentObjectBase //INonPersistent, IObjectSpaceLink, INotifyPropertyChanged, IXafEntityObject, IToggleRHS
    {
        //private int _BrandId;
        //[Browsable(false)]
        //[ModelDefault("AllowEdit", "False")]
        //public int BrandId {
        //    get => _BrandId;
        //    set
        //    {
        //        _BrandId = value;
        //        OnPropertyChanged();
        //    }
        //}
        private BabyToy _babyToy;
        private string _name;
        private PreSchoolToy _preSchoolToy;
        private ToddlerToy _toddlerToy;

        //[Browsable(false)]
        private int _toyCategory;

        //[DevExpress.ExpressApp.Data.Key]
        //[ModelDefault("AllowEdit", "False")]
        //public int Id { get; set; }
        [Browsable(false)] public int CacheIndex { get; set; }
        //[ImmediatePostData]
        public string ToyName
        {
            get => _name;
            set => SetPropertyValue(nameof(ToyName), ref _name, value);
        }
        public ToyCategoryEnum ToyCategoryNum
        {
            get => (ToyCategoryEnum) _toyCategory;
            set => SetPropertyValue(nameof(ToyCategoryNum), ref _toyCategory, (int) value);
        }
        public int ToyCategory
        {
            get => _toyCategory;
            set => SetPropertyValue(nameof(ToyCategory), ref _toyCategory, value);
        }
        [Appearance("IsBabyToy", Visibility = ViewItemVisibility.Hide, Criteria = "[ToyCategoryNum] != 1")]
        [VisibleInListView(false)]
        [NotMapped]
        public BabyToy BabyToy
        {
            get => ToyCategoryNum != ToyCategoryEnum.Baby ? null : _babyToy;
            set => _babyToy = value;
        }
        [Appearance("IsPreSchoolToy", Visibility = ViewItemVisibility.Hide, Criteria = "[ToyCategoryNum] != 3")]
        [VisibleInListView(false)]
        [NotMapped]
        public PreSchoolToy PreSchoolToy
        {
            get => ToyCategoryNum != ToyCategoryEnum.PreSchool ? null : _preSchoolToy;
            set => _preSchoolToy = value;
        }
        [Appearance("IsToddlerToy", Visibility = ViewItemVisibility.Hide, Criteria = "[ToyCategoryNum] != 2")]
        [VisibleInListView(false)]
        [NotMapped]
        public ToddlerToy ToddlerToy
        {
            get => ToyCategoryNum != ToyCategoryEnum.Toddler ? null : _toddlerToy;
            set => _toddlerToy = value;
        }

        //[NotMapped]

        // IObjectSpace persistentObjectSpace => ((NonPersistentObjectSpace)ObjectSpace)?.AdditionalObjectSpaces?.FirstOrDefault();

        //[DataSourceProperty("Brands")]
        //[NotMapped]
        //[ImmediatePostData]
        //public virtual Brand Brand
        //{
        //    get => persistentObjectSpace?.GetObjectByKey<Brand>(BrandId);
        //    set
        //    {
        //        BrandId = value.Id; 
        //        OnPropertyChanged();
        //    }
        //}

        //[Browsable(false)] public IList<Brand> Brands => persistentObjectSpace?.GetObjects<Brand>(CriteriaOperator.Parse("[Id] > 0"));

        //private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    if (PropertyChanged == null) return;
        //    if (ObjectSpace == null) return;
        //    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}
        [Browsable(false)] public string SearchText { get; set; }
        [Browsable(false)] [Key] public int Id { get; set; }

        public List<NonPersistentObjectBase> GetData(IObjectSpace os)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var connect = new ToysDbContext(connectionString))
            {
                var parameters = new List<SqlParameter>();
                var sql =
                    "select t.Id, t.Name as ToyName, b.Id as BrandId, t.ToyCategory, b.Name as BrandName from toys t inner join brands b on t.Brand_Id = b.Id";
                if (SearchText?.Length > 0)
                {
                    sql += " where t.name like @name";
                    parameters.Add(new SqlParameter("@name", $"%{SearchText}%"));
                }

                var results = connect.Database.SqlQuery<NPToy>(sql, parameters.ToArray()).ToList();
                var npresults = results.ConvertAll(x => (NonPersistentObjectBase) x);
                //var index = 0;
                //foreach (NonPersistentObjectBase np in npresults)
                //{
                //    np.CacheIndex = index;
                //    //((IObjectSpaceLink) np).ObjectSpace = os;
                //    index++;
                //}
                return npresults;
            }
        }

        public void NPOnSaving(IObjectSpace os)
        {
            //var brand = os.FindObject<Brand>(CriteriaOperator.Parse("[Id] = ?", BrandId));
            //if (brand == null)
            //{
            //    throw new Exception($"Category {BrandId} was not found");
            //}
            //    toy.Brand = brand;
            var toy = os.FindObject<Toy>(CriteriaOperator.Parse("[Id] = ?", Id));
            toy.Name = ToyName;
            toy.ToyCategory = _toyCategory;
            os.SetModified(toy); // remember to call os.CommitChanges()
        }

        //[NotMapped]
        //[Browsable(false)]
        //public IObjectSpace ObjectSpace { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetModified()
        {
            ObjectSpace.SetModified(this);
        }

        public void OnCreated()
        {
            //throw new NotImplementedException();
        }

        public void OnSaving()
        {
            Console.WriteLine("Saving");
            //throw new NotImplementedException();
        }

        public void OnLoaded()
        {
            var os = ((NonPersistentObjectSpace) ObjectSpace).AdditionalObjectSpaces.FirstOrDefault();
            switch (ToyCategoryNum)
            {
                case ToyCategoryEnum.Baby:
                    BabyToy = FindOrMakeToyInfo<BabyToy>(os);
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

        private T FindOrMakeToyInfo<T>(IObjectSpace os) where T : IToyType
        {
            var toy = os.FindObject<T>(CriteriaOperator.Parse("[Id]=?", Id));
            if (toy != null) return toy;
            toy = os.CreateObject<T>();
            toy.Id = Id;
            return toy;
        }

        public void SetKey(int id)
        {
            Id = id;
        }
    }
}