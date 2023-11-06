const path = require('path');
const fs = require('fs');

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

module.exports = {
    getPackageDescription,
    getPackageVersion,
    extractDependencies,
    getPackageDependencies,
    getPackageNames,
    getPackagePath,
    getPackageNames
};
