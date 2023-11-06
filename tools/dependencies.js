const path = require('path');
const fs = require('fs');
const parser = require('args-parser');

const { 
    getPackageNames,
    getPackageVersion,
    getPackageDependencies
} = require('./packages');


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
    
}

run()

module.exports = {
    orderedDependencies
}
