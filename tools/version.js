const path = require('path');
const fs = require('fs');
const semver = require('semver');
const parser = require('args-parser')

function getPackageDescription(package) {
    let packagePath = path.join(getPackagePath(package), "package.json")
    if (fs.existsSync(packagePath)) {
        let packageJSON = JSON.parse(fs.readFileSync(packagePath));
        return packageJSON;
    }
    throw new Error("package " + package + " does not exist");
}

// returns a set of package dependencies for a local package
function getPackageVersion(package) {
    return getPackageDescription(package).version
}

function extractDependencies(description) {
    let dependencies = new Set()
    for (const [dependency, version] of Object.entries(description.dependencies)) {
        if (!dependency.startsWith("com.sonosthesia")) {
            continue;
        }
        dependencies.add(dependency);
    }
    return dependencies
}

// returns a set of local paths to dependencies for a unity project manifest as object
function getPackageDependencies(package) {
    return extractDependencies(getPackageDescription(package))
}

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

function getPackageNames(rootPath) {
    return path.join(__dirname, "..", "packages")
}


function getPackagePath(package) {
    return path.join(__dirname, "..", "packages", package)
}

function getPackageNames() {
    let packagesPath = path.join(__dirname, "..", "packages")
    return fs.readdirSync(packagesPath, { withFileTypes: true })
        .filter(dirent => dirent.isDirectory())
        .map(dirent => dirent.name)
        .filter(name => name.startsWith("com.sonosthesia."))
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
