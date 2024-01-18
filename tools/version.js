const path = require('path');
const fs = require('fs');
const prettyjson = require('prettyjson');
const chalk = require('chalk');
const semver = require('semver');
const parser = require('args-parser');

const { 
    getPackageDescription, 
    getPackagePath, 
    getPackageNames, 
    getPackageVersion, 
    getPackageDependencies 
} = require('./packages');

const {
    listChangedPackages
} = require('./state');

function updatePackageDependencies(package, updated) {
    let description = getPackageDescription(package);
    description.version = updated;
    for (const [dependency, version] of Object.entries(description.dependencies)) {
        if (!dependency.startsWith("com.sonosthesia")) {
            continue;
        }
        description.dependencies[dependency] = updated;
    }
    let packagePath = path.join(getPackagePath(package), "package.json");
    fs.writeFileSync(packagePath, JSON.stringify(description, null, 2));
}

function run() {
    let args = parser(process.argv);
    let packageNames = getPackageNames();

    // args.update can be major, minor, patch

    if (args.update) {

        if (args.all) {
            let packageVersions = packageNames.map(name => getPackageVersion(name));
            let highestVersion = packageVersions.sort(semver.rcompare)[0];
            console.log(highestVersion);
            let updatedVersion = semver.inc(highestVersion, args.update);
            console.log(updatedVersion);
            console.log('All');
            for (const package of packageNames) {
                updatePackageDependencies(package, updatedVersion);
            }
        }

        if (args.tag) {
            let changedPackages = listChangedPackages(args.tag);
            console.log(chalk.green(`Updating changed packages:\n${[...changedPackages].join('\n')}`));
            let changedPackageDescriptions = {};
            for (const changedPackage of changedPackages) {
                const version = getPackageVersion(changedPackage);
                const changedPackageDescription = getPackageDescription(changedPackage);
                const updatedVersion = semver.inc(version, args.update);
                changedPackageDescription.version = updatedVersion
                changedPackageDescriptions[changedPackage] = changedPackageDescription;
                console.log(chalk.yellow(`Updating ${changedPackage} to ${updatedVersion}`));
            }
            // console.log(prettyjson.render(changedPackageDescriptions));
            // now that we have the new versions, update dependencies
            for (const changedPackage of changedPackages) {
                const changedPackageDescription = changedPackageDescriptions[changedPackage];
                for (const dependency in changedPackageDescription.dependencies) {
                    const changedDependencyDescription = changedPackageDescriptions[dependency];
                    if (changedDependencyDescription) {
                        changedPackageDescription.dependencies[dependency] = changedDependencyDescription.version;
                    }
                }
                let packagePath = path.join(getPackagePath(changedPackage), "package.json");
                fs.writeFileSync(packagePath, JSON.stringify(changedPackageDescription, null, 2));
            }
            // console.log(prettyjson.render(changedPackageDescriptions));

        }
    }
}

run()
