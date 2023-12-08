const execSync = require('child_process').execSync;
const parser = require('args-parser');

const { orderedDependencies } = require('./dependencies');
const { getPackagePath } = require('./packages');

const blacklist = new Set([
    "com.sonosthesia.fractal",
    "com.sonosthesia.shape",
    "com.sonosthesia.visualflow",
    "com.sonosthesia.voxel",
]);

// Publish sequence:
// - Ensure no build errors on Unity project
// - Run node version.js to bump and align versions
// - Push dev, merge to main 
// - Create release on github with vx.x.x tag
// - Run publish.js to publish all the packages to npm and check for errors

function publish(package, dry) {
    const packagePath = getPackagePath(package);
    if (dry) {
        console.log(`Dry publish : ${package}...`);
    } else {
        console.log(`Publishing ${package}...`);
        execSync('npm publish', { cwd: packagePath, stdio: 'inherit' });
        console.log('Succeeded');
    }
}

function run() {
    let args = parser(process.argv);
    const packages = orderedDependencies();
    
    const errors = {};

    for (const package of packages) {
        if (blacklist.has(package)) {
            console.log(`Ignoring blacklisted package : ${package}.`);
            continue;
        }
        try {
            publish(package, args.dry);
        } catch (error) {
            console.error(`Failed to publish ${package}.`, error);
            errors[package] = error;
        }
    }

    for (let package in errors) {
        console.error(`Error publishing package : ${package}\n${errors[package]}`);
    }
}

run()