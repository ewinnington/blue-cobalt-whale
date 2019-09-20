# blue-cobalt-whale
Docker repository for composed applications

## Redis
docker run --name redis-1 -p 7001:6379 -d redis 

Runs a redis image (redis:latest) in a container named redis-1, -p mapping local port 7001 to container port 6379, in -d detached mode so that it runs in the background. If you don't use -p only containers can access it. 

To connect to that redis container instance, tell docker you want to expose it with --link redis-1:redis

If you have a custom redis.conf file, you can load it at container launch by adding the -v flag to the docker run command:

 -v [path to custom redis.conf file]:/usr/local/etc/redis/redis.conf

## Rabbitmq & management
docker run -d --hostname rabbitmq --name rabbit-1 rabbitmq:3.7.18 

Runs a rabbitmq bus image, not externally available.

docker run -d --hostname rabbitmq --name rabbit-mgmt-1 -p 9191:15672 rabbitmq:3.7.18-management

Runs a management web gui and makes it accessible to outside at http://localhost:9191
user: guest
password: guest

## Postgresql

## Ms-Sql server

## Oracle database 


# Installing Node
node-v10-16.3 from nodejs.org

## Create a npm application 
Create a folder
mkdir hello-redis-js
cd hello-redis-js
npm init 
npm install redis --save

create app.js file



# Installing a database management tool (or visual studio)