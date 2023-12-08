const path = require('path');
const execSync = require('child_process').execSync;
const parser = require('args-parser');

const { getDuration } = require('./utils');
const { createGIF } = require('./gif');

function fade(filePath, fadeDuration) {

    // https://dev.to/dak425/add-fade-in-and-fade-out-effects-with-ffmpeg-2bj7

    const fileDuration = getDuration(filePath)
    const directory = path.dirname(filePath);
    const fileName = path.basename(filePath);
    const extension = path.extname(filePath);
    const outputFileName = fileName.replace(extension, '') + '_faded' + extension;


    let command = `ffmpeg -i ${fileName} `;
    command += `-vf "fade=t=in:st=0:d=${fadeDuration},fade=t=out:st=${fileDuration-fadeDuration}:d=${fadeDuration}" `;
    command += `-af "afade=t=in:st=0:d=${fadeDuration},afade=t=out:st=${fileDuration-fadeDuration}:d=${fadeDuration}" `;
    command += ` ${outputFileName}`;
    
    console.log(command);

    execSync(command, { cwd: directory, stdio: 'inherit' });

    return path.join(directory, outputFileName);
}

function truncate(filePath, end) {

    const directory = path.dirname(filePath);
    const fileName = path.basename(filePath);
    const extension = path.extname(filePath);
    const outputFileName = fileName.replace(extension, '') + '_truncated' + extension;

    const command = `ffmpeg -i ${fileName} -c copy -t ${end} ${outputFileName}`;

    console.log(command);

    execSync(command, { cwd: directory, stdio: 'inherit' });

    return path.join(directory, outputFileName);
}

function delayAudio(filePath, seconds) {

    const directory = path.dirname(filePath);
    const fileName = path.basename(filePath);
    const extension = path.extname(filePath);
    const outputFileName = fileName.replace(extension, '') + '_delayed' + extension;
    
    const command = `ffmpeg -i "${fileName}" -itsoffset ${seconds} -i "${fileName}" -map 0:v -map 1:a -c copy "${outputFileName}"`;

    // ffmpeg -i "Movie_008.mp4" -itsoffset 0.12 -i "Movie_008.mp4" -map 0:v -map 1:a -c copy "Movie_008_delayed.mp4"

    console.log(command);

    execSync(command, { cwd: directory, stdio: 'inherit' });

    return path.join(directory, outputFileName);
}


function run() {
    
    let args = parser(process.argv);

    let filePath = args.file;

    console.log(`Processing file : ${filePath}`);

    if (args.pipeline) {
        switch (args.pipeline) {
            case "unity":
                args.delay = args.delay ?? 0.1;
                args.gif_start = args.gif_start ?? 60.0;
                args.gif_duration = args.gif_duration ?? 8.0;
                break;
            default:
                break;
        }
    }

    if (args.delay) {
        filePath = delayAudio(filePath, args.delay);
    }

    if (args.truncate) {
        filePath = truncate(filePath, args.truncate);
    }

    if (args.fade) {
        filePath = fade(filePath, args.fade);
    }

    console.log(`Processed file : ${filePath}`);

    if (args.gif_start && args.gif_duration) {
        filePath = createGIF(filePath, args.gif_start, args.gif_duration)
        console.log(`GIF file : ${filePath}`);
    }
}

run()