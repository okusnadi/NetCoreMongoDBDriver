using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDB.Test
{
    public class MongoDBUtils
    {

        private readonly IMongoClient mongoClient = null;


        public MongoDBUtils(string connectionString)
        {
            try
            {
                mongoClient = new MongoClient(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public IMongoCollection<T> GetCollection<T>() where T : EntityBase
        {
            var dbName = DbConfig.DbName;
            var colName = DbConfig.CollectionDics[typeof(T)];
            try
            {

                var db = mongoClient.GetDatabase(dbName);
                if (db != null)
                {
                    try
                    {
                        if (db.ListCollections().ToList().Find(x => x.GetElement(0).Value == colName).Count() <= 0)
                        {
                            db.CreateCollection(colName);
                        }
                    }
                    catch
                    {
                        try
                        {
                            db.CreateCollection(colName);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    return db.GetCollection<T>(colName);

                }
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        public bool InsertOne<T>(T t) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                cols.InsertOne(t);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertOneAsync<T>(T t) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                await cols.InsertOneAsync(t);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateOne<T>(T t) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                var fieldList = new List<UpdateDefinition<T>>();
                foreach (var pi in typeof(T).GetProperties())
                {
                    if (pi.Name.ToUpper() != "ID")
                    {
                        var update = Builders<T>.Update.Set(pi.Name, pi.GetValue(t));
                        fieldList.Add(update);
                    }
                }
                var resultUpdate = Builders<T>.Update.Combine(fieldList);
                var result = cols.UpdateOne(x => x.Id == t.Id, resultUpdate);
                return result != null && result.IsAcknowledged;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOneAsync<T>(T t) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                var fieldList = new List<UpdateDefinition<T>>();
                foreach (var pi in typeof(T).GetProperties())
                {
                    if (pi.Name.ToUpper() != "ID")
                    {
                        var update = Builders<T>.Update.Set(pi.Name, pi.GetValue(t));
                        fieldList.Add(update);
                    }
                }
                var resultUpdate = Builders<T>.Update.Combine(fieldList);
                var result = await cols.UpdateOneAsync(x => x.Id == t.Id, resultUpdate);
                return result != null && result.IsAcknowledged;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteOne<T>(T t) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                cols.DeleteOne(x => x.Id == t.Id);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteOneAsync<T>(T t) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                await cols.DeleteOneAsync(x => x.Id == t.Id);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public IList<T> Query<T>(Expression<Func<T, bool>> filter) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                return cols.Find(filter).ToList();
            }
            catch
            {
                return new List<T>();
            }
        }

        public async Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> filter) where T : EntityBase
        {
            try
            {
                var cols = GetCollection<T>();
                return await cols.Find(filter).ToListAsync();
            }
            catch
            {
                return new List<T>();
            }
        }

    }

    internal class DbConfig
    {
        public const string DbName = "MyTest";

        public static readonly Dictionary<Type, string> CollectionDics = new Dictionary<Type, string>()
        {
            {typeof(User),"Users" },
        };
    }
}
