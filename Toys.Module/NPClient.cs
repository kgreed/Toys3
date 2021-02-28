using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.XtraSpreadsheet.Model;
using Toys.Module.BusinessObjects;
namespace Toys.Module
{
    public class NPClient
    {
        static NPClient()
        {
            GlobalServiceProvider<NPClient>.AddService(() => new NPClient());
        }
        public Dictionary<Type, DataStoreMapping> Mappings { get; set; }
        public IDataStore DataStore { get; set; }

        public NPClient()
        {
            InitData();
        }

        private void InitData()
        {
            this.DataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema, false);
            this.Mappings = new Dictionary<Type, DataStoreMapping>();
            var mToy = new DataStoreMapping {Table = new DBTable("NPToy")};
            var mToyKey = new DBColumn("Id", true, null, 0, DBColumnType.Int32) {IsIdentity = true};
            mToy.Table.AddColumn(mToyKey);
            mToy.Table.AddColumn(new DBColumn("Name", false, null, 1024, DBColumnType.String));
            mToy.Table.AddColumn(new DBColumn("ToyCategory", false, null, 32, DBColumnType.Int32));
            mToy.Table.PrimaryKey = new DBPrimaryKey(new object[] {mToyKey});
            mToy.Create = () => new NPToy();
            mToy.SetKey = (obj, key) => { ((NPToy) obj).SetKey((int) key); };
            mToy.GetKey = (obj) => ((NPToy) obj).Id;
            mToy.Load = (obj, values, omap) =>
            {
                var o = (NPToy) obj;
                o.SetKey((int) values[0]);
                o.ToyName = (string) values[1];
                o.ToyCategory = (int) values[2];
            };
            mToy.Save = (obj, values) =>
            {
                values[0] = ((NPToy) obj).Id;
                values[1] = ((NPToy) obj).ToyName;
                values[2] = ((NPToy) obj).ToyCategory;
            };
            mToy.RefColumns = Enumerable.Empty<DataStoreMapping.Column>();
            Mappings.Add(typeof(NPToy), mToy);
            DataStore.UpdateSchema(false, mToy.Table);
            CreateDemoData((InMemoryDataStore) DataStore);
        }

        public void RefreshDemoData()
        {
           InitData();
        }

        private void CreateDemoData(InMemoryDataStore inMemoryDataStore)
        {
            var ds = new System.Data.DataSet();
            using (var ms = new System.IO.MemoryStream())
            {
                using (var writer = System.Xml.XmlWriter.Create(ms))
                {
                    inMemoryDataStore.WriteXml(writer);
                    writer.Flush();
                }
                ms.Flush();
                ms.Position = 0;
                ds.ReadXml(ms);
            }
            // var gen = new GenHelper();
           
            var dtToys = ds.Tables["NPToy"];
             
            var o = new NPToy();
            var data = o.GetData(null);
             
            foreach (var rec in data)
            {
                var toy = rec as NPToy;
                dtToys.Rows.Add(toy.Id,toy.ToyName, toy.ToyCategory);
            }
            ds.AcceptChanges();
            using (var ms = new System.IO.MemoryStream())
            {
                ds.WriteXml(ms, System.Data.XmlWriteMode.WriteSchema);
                ms.Flush();
                ms.Position = 0;
                using (var reader = System.Xml.XmlReader.Create(ms))
                {
                    inMemoryDataStore.ReadXml(reader);
                }
            }
        }

        private static T GetReference<T>(ObjectMap map, object key)
        {
            return (key == null) ? default(T) : map.Get<T>(key);
        }
    }
}