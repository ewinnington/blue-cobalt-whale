# blue-cobalt-whale
Docker repository for composed applications

## Redis
docker run --name redis-1 -p 7001:6379 -d redis 

Runs a redis image (redis:latest) in a container named redis-1, -p mapping local port 7001 to container port 6379, in -d detached mode so that it runs in the background. If you don't use -p only containers can access it. 

To connect to that redis container instance, tell docker you want to expose it with --link redis-1:redis

If you have a custom redis.conf file, you can load it at container launch by adding the -v flag to the docker run command:

 -v [path to custom redis.conf file]:/usr/local/etc/redis/redis.conf

 ### Path with the config files 

 I modifed the Redis to disable the save command, to bind to all ip addresses (not recommended for prod) and added a strong password to the config file. We could also add it in the command line or even build a new image with the config file inside the container. 

 Added the absolute path to the config file.

 docker run --name redis-1 -p 6379:6379  -v c:\Repos\blue-cobalt-whale\redis\redis.conf:/usr/local/etc/redis/redis.conf -d redis redis-server /usr/local/etc/redis/redis.conf

## Rabbitmq & management
docker run -d --hostname rabbitmq --name rabbit-1 rabbitmq:3.7.18 

Runs a rabbitmq bus image, not externally available.

docker run -d --hostname rabbitmq --name rabbit-mgmt-1 -p 9191:15672 rabbitmq:3.7.18-management

Runs a management web gui and makes it accessible to outside at http://localhost:9191
user: guest
password: guest

## Postgresql

docker run -d -p 5432:5432 --name pgdb-1 -e POSTGRES_PASSWORD=XdccDa85_JK postgres

Create a connection in the Visual studio code extension SQLTools

To execute: 
Ctrl-Shift-P SQLTools Connection Run this File

CREATE TABLE testdb.counterparty 
    (id serial PRIMARY KEY,
    cp_name VARCHAR(100) NOT NULL,
    created_on TIMESTAMP NOT NULL);

## Ms-Sql server

docker run -e ACCEPT_EULA=Y -e SA_PASSWORD="XdccDa85_JK" -e MSSQL_PID="Developer" -p 1433:1433 -d --name mssqldb-1 mcr.microsoft.com/mssql/server:2017-latest 

## Oracle database 

The oracle DB in express is not available from the docker repository. So we have to follow a few steps to get it up and running. 

git clone https://github.com/oracle/docker-images.git --depth 1 --single-branch --progress

read https://github.com/oracle/docker-images/tree/master/OracleDatabase/SingleInstance

Download From Oracle the db and put it in the 18.4.0
oracle-database-xe-18c-1.0-1.x86_64.rpm

OracleDatabase\SingleInstance\dockerfiles\18.4.0

docker build -t oracle/database:18.4.0-xe -f Dockerfile.xe .

Run it!

docker run --rm -it -p 1521:1521/tcp -p 5500:5500/tcp -p 8080:8080/tcp --name oracledb-1 oracle/database:18.4.0-xe

Run it as a 
docker run --name oracledb-1 -d -p 1521:1521/tcp -p 5500:5500/tcp -p 8080:8080/tcp oracle/database:18.4.0-xe


docker run --name <container name> \
-p <host port>:1521 -p <host port>:5500 \
-e ORACLE_PWD=<your database passwords> \
-e ORACLE_CHARACTERSET=<your character set> \
-v [<host mount point>:]/opt/oracle/oradata \
oracle/database:18.4.0-xe

Parameters:
   --name:        The name of the container (default: auto generated)
   -p:            The port mapping of the host port to the container port.
                  Two ports are exposed: 1521 (Oracle Listener), 8080 (APEX)
   -e ORACLE_PWD: The Oracle Database SYS, SYSTEM and PDB_ADMIN password (default: auto generated)
   -e ORACLE_CHARACTERSET:
                  The character set to use when creating the database (default: AL32UTF8)
   -v /opt/oracle/oradata
                  The data volume to use for the database.
                  Has to be writable by the Unix "oracle" (uid: 54321) user inside the container!
                  If omitted the database will not be persisted over container recreation.
   -v /opt/oracle/scripts/startup | /docker-entrypoint-initdb.d/startup
                  Optional: A volume with custom scripts to be run after database startup.
                  For further details see the "Running scripts after setup and on startup" section below.
   -v /opt/oracle/scripts/setup | /docker-entrypoint-initdb.d/setup
                  Optional: A volume with custom scripts to be run after database setup.
                  For further details see the "Running scripts after setup and on startup" section below.
				  
Install the Oracle Instant client to C:\oracle\instantclient_19_3
Add it to the system path 

## MongoDB 

What's life without a NoSql db? 



# Installing Node
node-v10-16.3 from nodejs.org
node-v12.10.0 from nodejs.org

## Create a npm application 
Create a folder
mkdir hello-redis-js
cd hello-redis-js
npm init 
npm install redis --save

create app.js file

create Dockerfile

Build a Docker Container from the app 
docker build -t hello-redis-js .

Run the application and link it to the redis 
docker run --link redis-1:redis hello-redis-js 

# Applications

## .NET 

### Console .net 

### ASP.NET 

### WPF

### Workflow foundation 

## Java 

### Spring boot

### Tomcat 

### Console

### JavaFX

## Node

## C++

## Go 

# Calling endpoints for fun and profit

## CLI with curl 

## UI with Postman 


# Installing a database management tool (or visual studio)

## VsCode integrated
Install VSCode extension: SQLTools - Database tools by Matheus Teixeira
Create a connection
Open the DB 
Start a session 
Work on the DB !

## DBeaver (desktop client)
Install DBeaver from the microsoft store 
Connect to the databases 

# Next steps!

## Tagging an image in Docker and pushing it to your repository
So to save the images you have prepared to share them easily, you have to tag them with your repository information. 

docker tag oracle/database:18.4.0-xe dynamicwhalesquirrel/blue-cobalt-whale:oracle-18.4.0-xe

This tells docker where to store the image in your repository (I called mine blue-cobale-whale after this project).

First you probably have to use docker to login with your account and password
docker login --username=dynamicwhalesquirrel

docker push dynamicwhalesquirrel/blue-cobalt-whale:oracle-18.4.0-xe

This pushes the image up to the docker repository (poor them, the oracle image is 2.5 GB!)

If you mistag your image (eg - here I added a path that was too long, if the image has another tag, you can untag it)
docker rmi dynamicwhalesquirrel/blue-cobalt-whale/database:18.4.0-xe

## Startup scripts for each of the databases to create a schema every time the container starts up

### pgsql 
### oracle
### mssql 

## Have persistant volumes for the databases 

If you want to have the database engine inside docker, but the database files and data outside of docker, so that they are preseved from one start to the next, here's how you do it: 

### pgsql 
### oracle 
### mssql 

# Using docker for tests 

Now we have databases that are initialized either to empty, or to a fresh schema script every time they start. We could use the to do some integration tests of our applications. 


# Docker compose 

We are going to put all these nice images together and deliver an application composed of different images. 


# What about Kubernetes?

You can install minikube and the kubernetes cli to use kubernetes on your local machine. 


# What's this about functions, where can I get some? 

We are going to be using azure functions from microsoft, but hosting them ourselves in the docker environment. 
https://www.npmjs.com/package/azure-functions-core-tools


## In my docker ? 

https://medium.com/faun/azure-functions-in-a-docker-container-56e625da3243


## In my local kubernetes?

There's several platforms for functions available.

### Azure


### kubeless

https://docs.bitnami.com/kubernetes/how-to/get-started-serverless-computing-kubeless/