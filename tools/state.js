const path = require('path');
const fs = require('fs');
const semver = require('semver');
const parser = require('args-parser');
const { execSync } = require('child_process');

function listChangedFiles(tag) {
    // Run the git diff command synchronously
    const stdout = execSync(`git diff --name-only ${tag}`, { encoding: 'utf-8' });
    // Split the stdout into an array of file names
    const files = stdout.trim().split('\n');
    return files;
}

function listChangedPackages(tag) {
    let files = listChangedFiles(tag);
    let packages = new Set();
    for (const file of files) {
        if (!file.startsWith('packages/')) {
            continue;
        }
        const filePath = path.parse(file);
        const directoryComponents = filePath.dir.split(path.posix.sep);
        if (directoryComponents.length < 2) {
            continue;
        }
        const package = directoryComponents[1];
        packages.add(package);
    }
    return packages;
}

function run() {
    let args = parser(process.argv);
    let packages = listChangedPackages(args.tag);
    console.log(packages);
}

// run()

module.exports = {
    listChangedPackages,
    listChangedPackages
};