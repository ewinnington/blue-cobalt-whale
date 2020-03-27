using System;
using Npgsql;
using System.Text.Json;
using NpgsqlTypes;

namespace using_pg_as_integration_test_database
{
    class Program
    {
        static void Main(string[] args)
        {
            //{
            //    var connectionStringBuilder = new NpgsqlConnectionStringBuilder() { ConnectionString = $"User ID=postgres;Password=XdccDa85_JK;Server=127.0.0.1;Port=5432;Database=postgres;Integrated Security=true;Pooling=false;CommandTimeout=300" };
            //    var db = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
            //    db.Open();

            //    string createDatabase = "CREATE DATABASE mytest";
            //    using (var cmd = db.CreateCommand())
            //    {
            //        cmd.CommandText = createDatabase;
            //        cmd.CommandType = System.Data.CommandType.Text;
            //        cmd.ExecuteNonQueryAsync().Wait();
            //    }
            //    db.Close();
            //}

            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder() { ConnectionString = $"User ID=postgres;Password=XdccDa85_JK;Server=127.0.0.1;Port=5432;Database=mytest;Integrated Security=true;Pooling=false;CommandTimeout=300" };
                var db = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
                db.Open();

                string CreateTable1 = "CREATE TABLE IF NOT EXISTS table1(id SERIAL CONSTRAINT id PRIMARY KEY, title VARCHAR(50))";

                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = CreateTable1;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQueryAsync().Wait();
                }

                string Insert1 = "INSERT INTO table1(title) VALUES (@title)";

                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = Insert1;
                    cmd.CommandType = System.Data.CommandType.Text;
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "@title";
                    param1.Value = "My life as a cat";
                    cmd.Parameters.Add(param1);
                    cmd.ExecuteNonQueryAsync().Wait();
                }


                //CREATE TABLE orders (
                //ID serial NOT NULL PRIMARY KEY,
                //info json NOT NULL
                //);

                var a = new Asset()
                {
                    Name = "a",
                    Basins = new System.Collections.Generic.List<Basin>()
                    {
                    new Basin() { Name = "b1", Volume = 1000, MaxHeight = 1492, MinHeight = 1230 },
                    new Basin() { Name = "b2", Volume = 1000, MaxHeight = 1492, MinHeight = 1230 }
                    },
                    Plants = new System.Collections.Generic.List<Plant>()
                    {
                        new Plant()
                        {
                            Name = "s1",
                            Machines = new System.Collections.Generic.List<Machine>()
                                    {
                                        new Machine() { Name = "t1", Power = 10, Throughput = 5300}

                                    }
                        }
                    }
                };
                NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

                string Insert2 = "INSERT INTO orders(info) VALUES (@p)";

                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = Insert2;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Json) { Value = a });
                    cmd.ExecuteNonQueryAsync().Wait();
                }
                
                string readOne = "Select info from orders LIMIT 1";
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = readOne;
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var someValue = reader.GetFieldValue<Asset>(0);
                        Console.WriteLine(someValue.Name);
                    }
                }

                db.Close();

            }
        }
    }
}
