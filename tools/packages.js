const path = require('path');
const fs = require('fs');
const glob = require('glob');
const YAML = require('yaml');
const memoizee = require('memoizee');

// gives the dependencies in resolution order (lowest first) 
const orderedDependencies = memoizee(() => {
    let packageNames = getPackageNames()
    const packageDependencies = {}
    for (const package of packageNames) {
        packageDependencies[package] = Array.from(getPackageDependencies(package));
    }
    const resolved = [];
    const resolvedSet = new Set();
    const unresolved = new Set(packageNames);
    while (true) {
        const resolvedSize = resolved.length;
        const unresolvedCopy = [...unresolved];;
        for (const package of unresolvedCopy) {
            const dependencies = packageDependencies[package];
            const isResolved = dependencies.every(dependency => resolvedSet.has(dependency));
            if (isResolved) {
                resolvedSet.add(package);
                resolved.push(package);
                unresolved.delete(package);
            }
        }
        if (resolved.length == resolvedSize) {
            break;
        }
    } 
    return resolved;
});

// returns all sonosthesia dependencies for a package (including descendents)
const getPackageDependencyTree = memoizee((package) => {
    let dependencies = new Set();
    for (const dependency of getPackageDependencies(package)) {
        dependencies.add(dependency)
        for (const child of getPackageDependencies(dependency)) {
            dependencies.add(child)
        }
    }
    return dependencies;
});

// returns the package.json for the given package as an object
const getPackageDescription = memoizee((package) => {
    let packagePath = path.join(getPackagePath(package), "package.json")
    if (fs.existsSync(packagePath)) {
        let packageJSON = JSON.parse(fs.readFileSync(packagePath));
        return packageJSON;
    }
    throw new Error("package " + package + " does not exist");
});

// returns a set of package dependencies for a local package
function getPackageVersion(package) {
    const description = getPackageDescription(package);
    return description.version
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

function getPackageAsmdefDescriptions(package) {
    let descriptions = {};
    const packagePath = getPackagePath(package);
    const packageDescription = getPackageDescription(package);
    const asmdefFiles = glob.sync(`**/*.asmdef`, { cwd: packagePath, nodir: true });
    //console.log(asmdefFiles);
    for (const asmdefFile of asmdefFiles) {
        try {
            const asmdef = JSON.parse(fs.readFileSync(path.join(packagePath, asmdefFile), 'utf8'));
            const meta = YAML.parse(fs.readFileSync(path.join(packagePath, asmdefFile + '.meta'), 'utf8'));
            descriptions[meta.guid] = {
                file : asmdefFile,
                asmdef : asmdef,
                meta : meta,
                package : packageDescription
            }
        } catch (error) {
            console.error(`Failed to get description for ${asmdefFile}`);
            //throw error;
        }
    }
    return descriptions;
}


module.exports = {
    orderedDependencies,
    getPackageDependencyTree,
    getPackageDescription,
    getPackageVersion,
    extractDependencies,
    getPackageDependencies,
    getPackageNames,
    getPackagePath,
    getPackageNames,
    getPackageAsmdefDescriptions
};
