const path = require('path');
const execSync = require('child_process').execSync;

function getDuration(filePath) {
    const directory = path.dirname(filePath);
    const fileName = path.basename(filePath);
    const command = `ffprobe -i ${fileName} -show_entries format=duration -v quiet -of csv="p=0"`;
    const duration = parseFloat(execSync(command, { cwd: directory }));
    console.log(`Got duration ${duration} with command : ${command}`);
    return duration;
}

function createGIF(filePath, start, duration, scale) {

    scale = scale ?? 480

    // https://engineering.giphy.com/how-to-make-gifs-with-ffmpeg/
    // ffmpeg -ss 61.0 -t 2.5 -i StickAround.mp4 -filter_complex "[0:v] fps=12,scale=480:-1,split [a][b];[a] palettegen [p];[b][p] paletteuse" SmallerStickAround.gif

    const directory = path.dirname(filePath);
    const fileName = path.basename(filePath);
    const extension = path.extname(filePath);

    const outputFileName = fileName.replace(extension, '') + '_short.gif';

    const command = `ffmpeg -ss ${start} -t ${duration} -i ${fileName} -filter_complex "[0:v] fps=24,scale=${scale}:-1,split [a][b];[a] palettegen [p];[b][p] paletteuse" ${outputFileName}`;

    console.log(command);

    execSync(command, { cwd: directory, stdio: 'inherit' });

    return path.join(directory, outputFileName);
}

module.exports = {
    getDuration,
    createGIF
}