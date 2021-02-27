using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Xpo.DB;
using Toys.Module.BusinessObjects;
namespace Toys.Module
{
    public class NPClient
    {
        static NPClient()
        {
            GlobalServiceProvider<NPClient >.AddService(() => new NPClient());
        }
        public Dictionary<Type, DataStoreMapping> Mappings { get; set; }
        public IDataStore DataStore { get; set; }



        public NPClient()
        {
            this.DataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema, false);
            this.Mappings = new Dictionary<Type, DataStoreMapping>();
            var mToy = new DataStoreMapping { Table = new DBTable("NPToys") };
            mToy.Table.AddColumn(new DBColumn("Id", true, null, 32, DBColumnType.Int32));
            mToy.Table.AddColumn(new DBColumn("Name", false, null, 1024, DBColumnType.String));
            mToy.Create = () => new Toy();
            mToy.Load = (obj, values, omap) => {
                ((NPToy)obj).SetKey((string)values[0]);
                ((NPToy)obj).Name = (string)values[1];
            };
            mToy.Save = (obj, values) => {
                values[0] = ((NPToy)obj).Id;
                values[1] = ((NPToy)obj).Name;
            };
            mToy.GetKey = (obj) => ((NPToy)obj).Id;
            mToy.RefColumns = Enumerable.Empty<DataStoreMapping.Column>();
            Mappings.Add(typeof(Toy), mToy);
            DataStore.UpdateSchema(false, mToy.Table);
            CreateDemoData((InMemoryDataStore)DataStore);
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
            var gen = new GenHelper();
            var idsAccount = new List<string>();
            var dtToys = ds.Tables["NPToys"];
            //for (int i = 0; i < 200; i++)
            //{
            //    var id = gen.MakeTosh(20);
            //    idsAccount.Add(id);
            //    dtAccounts.Rows.Add(id, gen.GetFullName());
            //}
            var o = new NPToy();
            var data =o.GetData(null).Cast<NPToy>();
             
            foreach (var rec in data)
            {
                
                dtToys.Rows.Add(rec.Id,rec.Name);
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