const fs = require('fs');
const path = require('path');
const execSync = require('child_process').execSync;
const parser = require('args-parser');

function selectSnapshotFileName(filePath, suffix) {
    const directory = path.dirname(filePath);
    const fileName = path.basename(filePath);
    const extension = path.extname(filePath);
    const fileNameNoExtension = fileName.replace(extension, '');
    let i = 0;
    while (true && i < 100) {
        const snapshotFileName = fileNameNoExtension + suffix + '_' + i + '.png';
        if (!fs.existsSync(path.join(directory, snapshotFileName))) {
            return snapshotFileName;
        }
        i++;
    }
    throw new Error('could not select snapshot name');
}

function extractSnapshot(filePath, time, suffix) {
    const fileName = path.basename(filePath);
    const directory = path.dirname(filePath);
    const snapshotFileName = selectSnapshotFileName(filePath, suffix);
    const command = `ffmpeg -ss ${time} -i ${fileName} -frames:v 1 ${selectSnapshotFileName(filePath, suffix)}`;
    console.log(command);
    execSync(command, { cwd: directory, stdio: 'inherit' });
}

function run() {
    let args = parser(process.argv);
    let filePath = args.file;
    args.suffix = args.suffix ?? '_frame';
    if (args.time) {
        extractSnapshot(filePath, args.time, args.suffix);
    }
}

run();