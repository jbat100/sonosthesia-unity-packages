const path = require('path');
const fs = require('fs');
const parser = require('args-parser');
const { getPackageVersion } = require('./packages');

// memoize
let packageDependencyCache = {}

// returns a set of package dependencies for a local package
function getPackageDependencies(package) {
    if (package in packageDependencyCache) {
        return packageDependencyCache[package]
    }
    let dependencies = new Set()
    let packagePath = path.join(getPackagePath(package), "package.json")
    if (fs.existsSync(packagePath)) {
        let packageJSON = JSON.parse(fs.readFileSync(packagePath));
        for (const [dependency, version] of Object.entries(packageJSON.dependencies)) {
            if (!dependency.startsWith("com.sonosthesia")) {
                continue
            }
            dependencies.add(dependency)
            for (const child of getPackageDependencies(dependency)) {
                dependencies.add(child)
            }
        }
    }
    packageDependencyCache[package] = Array.from(dependencies)
    return packageDependencyCache[package]
}

// returns a set of local paths to dependencies for a unity project manifest as object
function getManifestDependencies(unityJSON) {
    let dependencies = new Set()
    for (const [dependency, version] of Object.entries(unityJSON.dependencies)) {
        if (!dependency.startsWith("com.sonosthesia")) {
            continue
        }
        dependencies.add(dependency)
        for (const subdependency of getPackageDependencies(dependency)) {
            dependencies.add(subdependency)
        }
    }
    return dependencies
}

function localizeDependencies(manifest) {
    console.log("Switching to localDependencies " + manifest)
    if (!fs.existsSync(manifest)) {
        console.log("bailing out...")
        return
    }
    let unityJSON = JSON.parse(fs.readFileSync(manifest));
    let manifestDependencies = getManifestDependencies(unityJSON);
    let manifestDirectory = path.dirname(manifest);
    for (const dependency of manifestDependencies) {
        console.log("processing " + dependency)
        // tried to use path.posix.relative but things get messy
        unityJSON.dependencies[dependency] = "file:" + path.relative(
            manifestDirectory, 
            getPackagePath(dependency)
        ).replaceAll(path.sep, "/"); 
    }
    console.log(unityJSON)
    fs.writeFileSync(manifest, JSON.stringify(unityJSON, null, 2));
}

function versionDependencies(manifest) {
    console.log("Switching to versionDependencies " + manifest)
    if (!fs.existsSync(manifest)) {
        console.log("bailing out...")
        return
    }
    let unityJSON = JSON.parse(fs.readFileSync(manifest));
    let manifestDependencies = getManifestDependencies(unityJSON);
    for (const dependency of manifestDependencies) {
        console.log("processing " + dependency)
        // tried to use path.posix.relative but things get messy
        unityJSON.dependencies[dependency] = getPackageVersion(dependency); 
    }
    console.log(unityJSON)
    fs.writeFileSync(manifest, JSON.stringify(unityJSON, null, 2));
}

function getManifestPath(projectPath) {
    return path.join(projectPath, "Packages", "manifest.json")
}

function getDefaultProjectPath() {
    return path.join(__dirname, "..", "unity", "UnityProject")
}

function getPackagePath(package) {
    return path.join(__dirname, "..", "packages", package)
}

function getPackages() {
    let packagesPath = path.join(__dirname, "..", "packages")
    return readdirSync(packagesPath, { withFileTypes: true })
        .filter(dirent => dirent.isDirectory())
        .map(dirent => dirent.name)
}

function run() {
    let args = parser(process.argv)
    let project = args.project ?? getDefaultProjectPath()
    let manifest = getManifestPath(project)

    if (args.local) {
        localizeDependencies(manifest)
    } else if (args.version) {
        versionDependencies(manifest)
    }
}

run()
