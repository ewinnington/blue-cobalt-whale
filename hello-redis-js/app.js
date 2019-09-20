const os = require('os');

var redis = require("redis"), 
    client = redis.createClient({host:"rabbitmq", port:7001});

// if you'd like to select database 3, instead of 0 (default), call
// client.select(3, function() { /* ... */ });

client.on("error", function (err) {
    console.log("Error " + err);
});

client.set("host1", os.hostname(), redis.print);