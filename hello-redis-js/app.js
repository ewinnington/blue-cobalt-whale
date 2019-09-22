const os = require('os');

var redis = require("redis"), 
    client = redis.createClient({host:"redis-1", port:6379});

// if you'd like to select database 3, instead of 0 (default), call
// client.select(3, function() { /* ... */ });

client.on("error", function (err) {
    console.log("Error " + err);
});

client.AUTH("lk462drMuxA4fgr_2Pdlwfncxs^3");
client.set("host1", os.hostname(), redis.print);
console.log(client.get("host1", redis.print));