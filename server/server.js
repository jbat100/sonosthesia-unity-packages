const http = require('http');
const fs = require('fs');
const path = require('path');
const WebSocket = require('ws');
const msgpack = require('@msgpack/msgpack');

const server = http.createServer((req, res) => {
    // Set the content type header based on the file extension
    const contentType = {
        '.html': 'text/html',
        '.css': 'text/css',
        '.js': 'text/javascript'
    }[path.extname(req.url)] || 'text/plain';

    // Read the requested file and write it to the response
    fs.readFile(__dirname + req.url, (err, data) => {
        if (err) {
            res.writeHead(404, {'Content-Type': 'text/plain'});
            res.write('404 Not Found\n');
            res.end();
        } else {
            res.writeHead(200, {'Content-Type': contentType});
            res.write(data);
            res.end();
        }
    });
});

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => console.log(`Server running on port ${PORT}`));

const WSPORT = 8080;

const wsServer = new WebSocket.Server({ port: WSPORT });
console.log(`WebSocket running on port ${WSPORT}`)

wsServer.on('connection', (socket) => {
  console.log('Client connected to ws');

  // Send a welcome message to the client
  //const welcomeMessage = 'Welcome to the WebSocket server!';
  //socket.send(msgpack.encode(welcomeMessage));

  let serverCounter = 0;

  // Send a counter message every second from the server
  const serverInterval = setInterval(() => {
    serverCounter++;
    const counterMessage = { counter: serverCounter, from: 'server' };
    socket.send(msgpack.encode(counterMessage));
  }, 1000);

  // Listen for messages from the client
  socket.on('message', (data) => {
    //const message = msgpack.decode(data);
    //console.log(`Received message: ${JSON.stringify(message)}`);
    console.log(`Received message: ${data}`);
  });

  // Listen for the socket to close
  socket.on('close', () => {
    clearInterval(serverInterval);
    console.log('Client disconnected');
  });
});


