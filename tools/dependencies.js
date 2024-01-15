const path = require('path');
const fs = require('fs');
const chalk = require('chalk');
const prettyjson = require('prettyjson');
const parser = require('args-parser');

const { 
    getPackageNames,
    getPackageVersion,
    getPackageDependencies,
    getPackageAsmdefDescriptions,
    getPackageDescription,
    getPackageDependencyTree
} = require('./packages');

// gives the dependencies in resolution order (lowest first) 
function orderedDependencies() {
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
}

// runs a series of sanity checks concerning packages and dependencies
function checkDependencies() {
    const packageNames = getPackageNames();
    // first cycle through all packages and fetch descriptions 
    const allAsmdefDescriptions = {};
    const allDependencyTrees = {};
    for (const package of packageNames) {
        try {
            const descriptions = getPackageAsmdefDescriptions(package);
            //console.log(`Package ${package} asmdefs:\n${prettyjson.render(descriptions)}`);
            Object.assign(allAsmdefDescriptions, descriptions);
            allDependencyTrees[package] = getPackageDependencyTree(package);
        } catch (error) {
            console.log(`Error getting package ${package} asmdefs: ${error}`);
        }
    }
    
    // console.log(`All descriptions:\n${prettyjson.render(allDescriptions)}`);
    // then for each package  
    for (const package of packageNames) {
        const packageDescription = getPackageDescription(package);
        const asmdefDescriptions = getPackageAsmdefDescriptions(package);
        const specifiedDependencies = Object.keys(packageDescription.dependencies);
        const specifiedDependencyTree = getPackageDependencyTree(package);
        for (const description of Object.values(asmdefDescriptions)) {
            const references = description.asmdef.references;
            for (const reference of references) {
                const referenceDescription = allAsmdefDescriptions[reference.replace('GUID:', '')];
                // if it is a sonosthesia dependency
                if (referenceDescription) {
                    // then check that it is specified in the package dependencies
                    const name = referenceDescription.package.name;
                    // if the reference points to its own package then it's fine, bail out
                    if (name == packageDescription.name) {
                        continue;
                    }
                    if (!specifiedDependencies.includes(name)) {
                        if (specifiedDependencyTree.has(name)) {
                            console.error(chalk.yellow(`Package ${package} asmdef ${description.file} references ${reference} from ${referenceDescription.package.name} which is not specified but in dependency tree`));
                        } else {
                            console.error(chalk.red(`Package ${package} asmdef ${description.file} references ${reference} from ${referenceDescription.package.name} which is not in dependency tree`));
                        }
                    }
                }
            }
        }
    }

    for (const package of packageNames) {
        const description = getPackageDescription(package);
        for (let dependency in description.dependencies) {
            if (!dependency.startsWith('com.sonosthesia.')) {
                continue;
            }
            const dependencyVersion = description.dependencies[dependency];
            const currentVersion = getPackageVersion(dependency);
            if (dependencyVersion != currentVersion) {
                console.error(chalk.red(`Package ${package} dependency ${dependency} version ${dependencyVersion} is not aligned with current ${currentVersion}`));
            }
        }
    }
}

function run() {
    let args = parser(process.argv)
    let packageNames = getPackageNames()
    //console.log(packageNames)

    if (args.list) {
        for (const package of packageNames) {
            const dependencies = getPackageDependencies(package);
            console.log("Package : " + package + ", version " + getPackageVersion(package) + ", "
            + dependencies.size + " dependencies \n  " 
            + [...dependencies].join("\n  ") );
        }
    }

    if (args.order) {
        const resolved = orderedDependencies();
        console.log(resolved);
    }

    if (args.check) {
        checkDependencies();
    }
    
}

run()

module.exports = {
    orderedDependencies
}
