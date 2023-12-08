const fs = require('fs');
const path = require('path');
const execSync = require('child_process').execSync;
const parser = require('args-parser');

function extractClip(filePath, start, duration) {

    const fileName = path.basename(filePath);
    const directory = path.dirname(filePath);
    const extension = path.extname(filePath);

    const outputFileName = fileName.replace(extension, '') + '_clip' + 
        `_${start}`.replace('.', '-') + 
        `_${start + duration}`.replace('.', '-') + 
        extension;

    const command = `ffmpeg -ss ${start} -i ${fileName} -c copy -t ${duration} ${outputFileName}`;
    console.log(command);
    execSync(command, { cwd: directory, stdio: 'inherit' });
}

function run() {
    let args = parser(process.argv);
    let filePath = args.file;
    if (args.start && args.duration) {
        extractClip(filePath, args.start, args.duration);
    }
}

run();