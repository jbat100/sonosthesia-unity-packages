const path = require('path');
const fs = require('fs');
const semver = require('semver');
const parser = require('args-parser')

const { 
    getPackageDescription, 
    getPackagePath, 
    getPackageNames, 
    getPackageVersion, 
    getPackageDependencies 
} = require('./packages')

function updatePackageDependencies(package, updated) {
    let description = getPackageDescription(package)
    description.version = updated
    for (const [dependency, version] of Object.entries(description.dependencies)) {
        if (!dependency.startsWith("com.sonosthesia")) {
            continue;
        }
        description.dependencies[dependency] = updated
    }
    let packagePath = path.join(getPackagePath(package), "package.json")
    fs.writeFileSync(packagePath, JSON.stringify(description, null, 2))
}

function run() {
    let args = parser(process.argv)
    let packageNames = getPackageNames()
    console.log(packageNames)
    let packageVersions = packageNames.map(name => getPackageVersion(name))
    console.log(packageVersions)
    let highestVersion = packageVersions.sort(semver.rcompare)[0]
    console.log(highestVersion)

    for (const package of packageNames) {
        const dependencies = getPackageDependencies(package);
        console.log("Package : " + package + ", version " + getPackageVersion(package) + ", "
        + dependencies.size + " dependencies " 
        + [...dependencies].join(", ") );
    }

    // args.update can be major, minor, patch

    if (args.update) {
        let updatedVersion = semver.inc(highestVersion, args.update)
        console.log(updatedVersion)
        if (args.all) {
            console.log('All')
            for (const package of packageNames) {
                updatePackageDependencies(package, updatedVersion)
            }
        }
    }
}

run()
