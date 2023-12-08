const path = require('path');
const fs = require('fs');
const parser = require('args-parser');

const { getDuration, createGIF } = require('./utils');

function run() {

    let args = parser(process.argv);
    let file = args.file;
    let filePaths = [];

    const extensions = ['.mov'];

    if (fs.lstatSync(file).isDirectory()) {
        fs.readdirSync(file).forEach(content => {
            if (extensions.includes(path.extname(content))) {
                console.log(content);
                filePaths.push(path.join(file, content));
            }
          });
    } else {
        filePaths.push(file);
    }

    for (let filePath of filePaths) {
        let start = args.start ?? 0.0;
        let duration = args.duration ?? getDuration(filePath);
        createGIF(filePath, start, duration, args.scale);
    }
}

run();
