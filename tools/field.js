const path = require('path');
const fs = require('fs');
const parser = require('args-parser');

const { 
    getPackageDescription, 
    getPackagePath, 
    getPackageNames
} = require('./packages');

function updatePackageField(package, field, value) {
    let description = getPackageDescription(package);
    if (value) {
        description[field] = value;
    } else {
        delete description[field];
    }
    let packagePath = path.join(getPackagePath(package), "package.json");
    fs.writeFileSync(packagePath, JSON.stringify(description, null, 2));
}

function run() {
    let args = parser(process.argv);
    let packageNames = getPackageNames();
    console.log(packageNames);

    if (args.field) {
        let value = args.value ? String(args.value) : args.value;
        for (const package of packageNames) {
            updatePackageField(package, args.field, value)
        }
    }
}

run()
