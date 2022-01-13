"use strict";

const serverPort = 3000;
const express = require("express");
const http = require("http");
const path = require('path');
const WebSocket = require("ws");
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json({ type: 'application/json' }));
app.use(bodyParser.json({ type: 'application/*+json' }));

// Serve static files
app.use(express.static(path.join(__dirname, 'webclient/dist')));

app.get('/', function (_req, res) {
    res.sendFile(path.join(__dirname, 'webclient/dist', 'index.html'));
});

const server = http.createServer(app);
const websocketServer = new WebSocket.Server({ server });

app.get('/dapr/subscribe', (_req, res) => {
    res.json([
        {
            pubsubname: "pubsub",
            topic: "tweets",
            route: "tweets"
        }
    ]);
});

app.post('/tweets', (req, res) => {
    console.log("Tweets: ", req.body);
    // webSocketClient.send("TEST");
    websocketServer.clients.forEach(client => {
        client.send(JSON.stringify(req.body));
    });
    res.sendStatus(200);
});

//when a websocket connection is established
websocketServer.on("connection", (webSocketClient) => {
    console.log("WebSocket client connected");
    // send feedback to the incoming connection
    // let time = new Date();
    // webSocketClient.send("The time is: " + time.toTimeString());
    // setInterval(() => {
    //     let time = new Date();
    //     webSocketClient.send("The time is: " + time.toTimeString());
    // }, 1000);
});

//start the web server
server.listen(3000, () => {
    console.log("Websocket server started on port 3000");
});