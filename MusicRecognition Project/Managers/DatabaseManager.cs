using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBreeze;
using DBreeze.DataTypes;
using Newtonsoft.Json;

namespace MusicRecognition_Project
{
    class DatabaseHandler
    {
        /// <summary>
        /// This is the Database code.
        /// 
        /// </summary>

        private static DBreezeEngine engine;

        //Table names are : Songs and Recordings
        private const string TableSongs = @"Songs";
        private const string TableHashes = @"Hashes";

        public void InitDatabase()
        {
            if (engine == null)
            {
                Console.WriteLine(Application.CommonAppDataPath);
                engine = new DBreezeEngine(Application.CommonAppDataPath + @"\Music Recogition\");

                DBreeze.Utils.CustomSerializator.Serializator = JsonConvert.SerializeObject;
                DBreeze.Utils.CustomSerializator.Deserializator = JsonConvert.DeserializeObject;
            }
        }

        public void ReleaseDatabase()
        {
            engine?.Dispose();
            engine = null;
        }

        public void InsertSong(int songID, Dictionary<int, int> d) 
        {
            using (var tran = engine.GetTransaction())
            {
                try
                {
                    tran.SynchronizeTables(TableSongs);
                    tran.Insert<int, DbMJSON<Dictionary<int, int>>>(TableSongs, songID, d);
                    tran.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public Dictionary<int, int> GetSong(int songID)
        {
            using (var tran = engine.GetTransaction())
            {
                try
                {
                    tran.SynchronizeTables(TableSongs);
                    var row = tran.Select<int, DbMJSON<Dictionary<int, int>>>(TableSongs, songID);

                    if (row.Exists)
                    {
                        return row.Value.Get;
                    }
                    else
                    {
                        return new Dictionary<int, int>();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }

        private int FUZ_FACTOR = 2;

        public long hash(long[] data)
        {
            return (data[3] - (data[3] % FUZ_FACTOR)) * 100000000 + (data[2] - (data[2] % FUZ_FACTOR))
                    * 100000 + (data[1] - (data[1] % FUZ_FACTOR)) * 100
                    + (data[0] - (data[0] % FUZ_FACTOR));
        }

        public void InsertDataPoint(long hash, List<DataPoint> list)
        {
            using (var tran = engine.GetTransaction())
            {
                try
                {
                    tran.SynchronizeTables(TableHashes);
                    tran.Insert<long, DbMJSON<List<DataPoint>>>(TableSongs, hash, list);
                    tran.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public List<DataPoint> GetDataPoints(long hash)
        {
            using (var tran = engine.GetTransaction())
            {
                try
                {
                    tran.SynchronizeTables(TableHashes);
                    List<DataPoint> list = null;

                    var row = tran.Select<long, DbMJSON<List<DataPoint>>>(TableHashes, hash);
                    if (row.Exists)
                    {
                        list = row.Value.Get;
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return null;
        }


    }
}
