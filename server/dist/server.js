"use strict";
const http = require('http');
const fs = require('fs');
const path = require('path');
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
            res.writeHead(404, { 'Content-Type': 'text/plain' });
            res.write('404 Not Found\n');
            res.end();
        }
        else {
            res.writeHead(200, { 'Content-Type': contentType });
            res.write(data);
            res.end();
        }
    });
});
const PORT = process.env.PORT || 3000;
server.listen(PORT, () => console.log(`Server running on port ${PORT}`));
//# sourceMappingURL=server.js.map