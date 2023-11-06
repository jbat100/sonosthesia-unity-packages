const execSync = require('child_process').execSync;
const parser = require('args-parser');

const { orderedDependencies } = require('./dependencies');
const { getPackagePath } = require('./packages')

function publish(package, dry) {
    try {
        const packagePath = getPackagePath(package);
        if (dry) {
            console.log(`Dry publish : ${package}...`);
        } else {
            console.log(`Publishing ${package}...`);
            execSync('npm publish', { cwd: packagePath, stdio: 'inherit' });
            console.log('Succeeded');
        }

    } catch (error) {
        console.error(`Failed to publish ${package}.`, error);
    }
}

function run() {
    let args = parser(process.argv)
    const packages = orderedDependencies();
    
    for (const package of packages) {
        publish(package, args.dry);
    }

}

run()