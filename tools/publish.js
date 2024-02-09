const execSync = require('child_process').execSync;
const chalk = require('chalk');
const parser = require('args-parser');

const { 
    listChangedPackages,
    orderedDependencies,
    getPackagePath } = require('./utils');

const blacklist = new Set([
    "com.sonosthesia.fractal",
    "com.sonosthesia.shape",
    "com.sonosthesia.visualflow",
    "com.sonosthesia.voxel",
]);

// Publish sequence:
// - Ensure no errors in unity project
// - Run node version.js to bump and align versions (all or changed since specific tag)
// - Run dependencies.js check to detect potential dependency/reference issues
// - Run publish.js to publish all the packages to npm and check for errors (specify tag if not all)
// - Ensure no build errors on Unity project
// - Push dev, merge to main 
// - Create release on github with vx.x.x tag corresponding to the highest package version


function publish(package, dry) {
    const packagePath = getPackagePath(package);
    if (dry) {
        console.log(chalk.yellow(`Dry publish : ${package}...`));
    } else {
        console.log(chalk.yellow(`Publishing ${package}...`));
        execSync('npm publish', { cwd: packagePath, stdio: 'inherit' });
        console.log(chalk.green('Succeeded'));
    }
}

function run() {
    let args = parser(process.argv);
    const packages = orderedDependencies();
    
    const errors = {};

    if (args.all) {
        for (const package of packages) {
            if (blacklist.has(package)) {
                console.log(chalk.gray(`Ignoring blacklisted package : ${package}.`));
                continue;
            }
            try {
                publish(package, args.dry);
            } catch (error) {
                console.error(chalk.red(`Failed to publish ${package}.`, error));
                errors[package] = error;
            }
        }
        for (let package in errors) {
            console.error(chalk.red(`Error publishing package : ${package}\n${errors[package]}`));
        }
    }

    if (args.tag) {
        const changed = listChangedPackages(args.tag);
        for (const package of packages) {
            if (blacklist.has(package)) {
                console.log(chalk.gray(`Ignoring blacklisted package : ${package}.`));
                continue;
            }
            if (!changed.has(package)) {
                console.log(chalk.gray(`Ignoring unchanged package : ${package}.`));
                continue;
            }
            try {
                publish(package, args.dry);
            } catch (error) {
                console.error(chalk.red(`Failed to publish ${package}.`, error));
                errors[package] = error;
            }
        }
        for (let package in errors) {
            console.error(chalk.red(`Error publishing package : ${package}\n${errors[package]}`));
        }
    }
}

run()