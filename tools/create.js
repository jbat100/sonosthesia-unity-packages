const path = require('path');
const fs = require('fs');
const prettyjson = require('prettyjson');
const parser = require('args-parser');

function copyTemplateFile(templateDirectoryPath, packageDirectoryPath, fileName) {
    const templateFilePath = path.join(templateDirectoryPath, fileName);
    const packageFilePath = path.join(packageDirectoryPath, fileName);
    console.log(`Copying template ${fileName} at : ${packageFilePath}`);
    fs.copyFileSync(templateFilePath, packageFilePath);
}

function createPackage(displayName) {

    const templateDirectoryPath = path.normalize(path.join(__dirname, '..', 'template'));
    const templatePackageJSONPath = path.join(templateDirectoryPath, 'package.json');
    const templateAssembyPath = path.join(templateDirectoryPath, 'Package.asmdef');

    const package = JSON.parse(fs.readFileSync(templatePackageJSONPath));
    
    const packageName = 'com.sonosthesia.' + displayName.replace(/ /g, '').toLowerCase();
    const packageDirectoryPath = path.normalize(path.join(__dirname, '..', 'packages', packageName));

    if (fs.existsSync(packageDirectoryPath)) {
        throw new Error(`Package directory already exists : ${packageDirectoryPath}`);
    }

    console.log(`Creating package directory : ${packageDirectoryPath}`);
    fs.mkdirSync(packageDirectoryPath);

    const runtimeDirectoryPath = path.join(packageDirectoryPath, 'Runtime');
    console.log(`Creating runtime directory : ${runtimeDirectoryPath}`);
    fs.mkdirSync(runtimeDirectoryPath);

    // copy unchanged files

    copyTemplateFile(templateDirectoryPath, packageDirectoryPath, 'README.md');
    copyTemplateFile(templateDirectoryPath, packageDirectoryPath, 'LICENSE');

    // generate package.json

    package.name = packageName;
    package.description = 'Sonosthesia unity ' + displayName.toLowerCase();
    package.displayName = 'Sonosthesia ' + displayName;

    const packageJSONPath = path.join(packageDirectoryPath, 'package.json');
    console.log(`Creating package.json at : ${packageJSONPath}\n${prettyjson.render(package)}`);
    fs.writeFileSync(packageJSONPath, JSON.stringify(package, null, 2));

    // generate .asmdef

    const assembly = JSON.parse(fs.readFileSync(templateAssembyPath));

    const assembyName = 'Sonosthesia.' + displayName.replace(/ /g, '');
    assembly.name = assembyName;
    assembly.rootNamespace = assembyName;
    
    const assemblyPath = path.join(runtimeDirectoryPath, assembyName + '.asmdef');
    console.log(`Creating .asmdef at : ${assemblyPath}\n${prettyjson.render(assembly)}`);
    fs.writeFileSync(assemblyPath, JSON.stringify(assembly, null, 2));

}

function run() {

    let args = parser(process.argv);

    // name should be capitalised with spaces and without Sonosthesia preffix

    if (args.name) {
        createPackage(args.name)
    } else {
        throw new Error('Missing name argument');
    }

}

run();