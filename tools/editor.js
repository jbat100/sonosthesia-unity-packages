const path = require('path');
const yaml = require('yaml');
const fs = require('fs');
const prettyjson = require('prettyjson');
const parser = require('args-parser');

function createEditor(displayName, dry) {

    const templateDirectoryPath = path.normalize(path.join(__dirname, '..', 'template'));
    const templatePackageJSONPath = path.join(templateDirectoryPath, 'package.json');
    const templateAsmdefPath = path.join(templateDirectoryPath, 'Package.asmdef');

    const package = JSON.parse(fs.readFileSync(templatePackageJSONPath));
    
    const packageName = 'com.sonosthesia.' + displayName.replace(/ /g, '').toLowerCase();
    const packageDirectoryPath = path.normalize(path.join(__dirname, '..', 'packages', packageName));
    const runtimeDirectoryPath = path.join(packageDirectoryPath, 'Runtime')
    const runtimeAsmdefName = 'Sonosthesia.' + displayName.replace(/ /g, '') + '.asmdef';
    const runtimeAsmdefPath = path.join(packageDirectoryPath, 'Runtime', runtimeAsmdefName);
    const runtimeAsmdefMetaPath = runtimeAsmdefPath + '.meta';
    const editorDirectoryPath = path.join(packageDirectoryPath, 'Editor');
    const editorAsmdefName = 'Sonosthesia.' + displayName.replace(/ /g, '') + '.Editor.asmdef';
    const editorAsmdefPath = path.join(packageDirectoryPath, 'Editor', editorAsmdefName);

    if (fs.existsSync(editorDirectoryPath)) {
        throw new Error(`Package editor directory already exists : ${editorDirectoryPath}`);
    }

    if (!fs.existsSync(runtimeAsmdefPath)) {
        throw new Error(`Package runtime asmdef does not exists : ${runtimeAsmdefPath}`);
    }

    console.log(`Creating editor directory : ${editorDirectoryPath}`);
    if (!dry) {
        fs.mkdirSync(editorDirectoryPath);
    }
 
    const asmdefString = fs.readFileSync(runtimeAsmdefPath).toString('utf8');;
    const asmdef = JSON.parse(asmdefString);

    const assembyName = 'Sonosthesia.' + displayName.replace(/ /g, '') + '.Editor';
    asmdef.name = assembyName;
    asmdef.rootNamespace = assembyName;
    asmdef.includePlatforms = ['Editor'];
    asmdef.excludePlatforms = [];

    if (!fs.existsSync(runtimeAsmdefMetaPath)) {
        throw new Error(`Package runtime asmdef meta does not exists : ${runtimeAsmdefMetaPath}`);
    }

    const runtimeAsmdefMetaString = fs.readFileSync(runtimeAsmdefMetaPath).toString('utf8');;
    const runtimeAsmdefMeta = yaml.parse(runtimeAsmdefMetaString);
    asmdef.references.push('GUID:' + runtimeAsmdefMeta.guid);

    console.log(`Creating editor asmdef at : ${editorAsmdefPath}\n${prettyjson.render(asmdef)}`);
    if (!dry) {
        fs.writeFileSync(editorAsmdefPath, JSON.stringify(asmdef, null, 2));
    }
}

function run() {

    let args = parser(process.argv);

    // name should be capitalised with spaces and without Sonosthesia preffix

    if (args.name) {
        createEditor(args.name, args.dry)
    } else {
        throw new Error('Missing name argument');
    }

}

run();