using Docker.DotNet;
using Docker.DotNet.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace pg_tests
{
    public class DockerPgDb : IAsyncLifetime
    {
        public DbConnection Db { get; private set; }

        private DockerClient _client;
        private CreateContainerResponse _containerResponse;

        //MIT License Code from: https://github.com/Activehigh/Atl.GenericRepository/blob/master/Atl.Repository.Standard.Tests/Repositories/ReadRepositoryTestsWithNpgsql.cs
        //for initializing the docker client
        public async Task InitializeAsync()
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            //This is the windows only pipe "npipe://./pipe/docker_engine"
            //Unix pipe is "unix:///var/run/docker.sock" - don'tmknow if it works for OSX
            _client = new DockerClientConfiguration(new Uri(isWindows ? "npipe://./pipe/docker_engine" : "unix:///var/run/docker.sock")).CreateClient();

            var (containerRespose, port) = await GetContainer(_client, "postgres", "latest");
            _containerResponse = containerRespose;

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder() { ConnectionString = $"User ID=postgres;Password=password;Server=127.0.0.1;Port={port};Database=repotest;Integrated Security=true;Pooling=false;CommandTimeout=300" };
            Db = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
            Db.Open();

            await InitializeTableStructure(Db);
            await PopulateTable(Db);
        }

        private async Task<(CreateContainerResponse, string)> GetContainer(DockerClient client, string image, string tag)
        {
            var hostPort = new Random((int)DateTime.UtcNow.Ticks).Next(10000, 12000);
            //look for image
            var images = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                MatchName = $"{image}:{tag}",
            }, CancellationToken.None);

            //check if container exists
            var pgImage = images.FirstOrDefault();
            if (pgImage == null)
                throw new Exception($"Docker image for {image}:{tag} not found.");

            //create container from image
            var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                User = "postgres",
                Env = new List<string>()
                {
                    "POSTGRES_PASSWORD=password",
                    "POSTGRES_DB=repotest",
                    "POSTGRES_USER=postgres"
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>()
                {
                    ["5432"] = new EmptyStruct()
                },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        ["5432"] = new List<PortBinding>()
                            {new PortBinding() {HostIP = "0.0.0.0", HostPort = $"{hostPort}"}}
                    }
                },
                Image = $"{image}:{tag}",
            }, CancellationToken.None);

            if (!await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters()
            {
                DetachKeys = $"d={image}"
            }, CancellationToken.None))
            {
                throw new Exception($"Could not start container: {container.ID}");
            }

            var count = 10;
            Thread.Sleep(5000);
            var containerStat = await client.Containers.InspectContainerAsync(container.ID, CancellationToken.None);
            while (!containerStat.State.Running && count-- > 0)
            {
                Thread.Sleep(1000);
                containerStat = await client.Containers.InspectContainerAsync(container.ID, CancellationToken.None);
                
            }
            Thread.Sleep(10000); //I need some time for the DB to finish starting up so that my tests don't report the DB is starting up
            return (container, $"{hostPort}");
        }

        private async Task InitializeTableStructure(DbConnection db)
        {
            string CreateTable1 = "CREATE TABLE table1(id SERIAL CONSTRAINT id PRIMARY KEY, title VARCHAR(50))";

            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = CreateTable1;
                cmd.CommandType = System.Data.CommandType.Text;
                await cmd.ExecuteNonQueryAsync();
            }
       }

        private async Task PopulateTable(DbConnection db)
        {
            string Insert1 = "INSERT INTO table1(title) VALUES (@title)";

            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = Insert1;
                cmd.CommandType = System.Data.CommandType.Text;
                var param1 = cmd.CreateParameter();
                param1.ParameterName = "@title";
                param1.Value = "My life as a cat";
                cmd.Parameters.Add(param1);
                await cmd.ExecuteNonQueryAsync();
            }
    }

        public async Task DisposeAsync()
        {
            Db.Close();
            Db.Dispose();
            //stop container
            if (await _client.Containers.StopContainerAsync(_containerResponse.ID, new ContainerStopParameters(), CancellationToken.None))
            {
                //delete container
                await _client.Containers.RemoveContainerAsync(_containerResponse.ID, new ContainerRemoveParameters(), CancellationToken.None);
            }

            _client?.Dispose();
        }
    }

    public class Pg_unit_testing : IClassFixture<DockerPgDb>
    {
        DockerPgDb fixture; 
       
        
        public Pg_unit_testing(DockerPgDb fixture)
        {
            this.fixture = fixture; 
        }


        [Fact]
        public void The_DB_Exists()
        {
            Assert.Equal("repotest", fixture.Db.Database);
        }

        [Fact]
        public async void My_Life_As_A_Cat()
        {

            string getFirstTitle = "SELECT title FROM table1 ORDER BY id LIMIT 1";

            using (var cmd = fixture.Db.CreateCommand())
            {
                cmd.CommandText = getFirstTitle;
                cmd.CommandType = System.Data.CommandType.Text;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    Assert.True(dr.HasRows);
                    var _ = await dr.ReadAsync();
                    var title = dr.GetString(0);
                    Assert.Equal("My life as a cat", title);
                    //dr.Close(); //not necessary since the using will take care of it. For some reason, I still always want to write it in
                }
            }
        }
    }
}
