const execSync = require('child_process').execSync;
const parser = require('args-parser');

const { orderedDependencies } = require('./dependencies');
const { getPackagePath } = require('./packages');

const blacklist = new Set([
    "com.sonosthesia.fractal",
    "com.sonosthesia.shape",
    "com.sonosthesia.visualflow",
    "com.sonosthesia.voxel",
]);

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
        if (blacklist.has(package)) {
            console.log(`Ignoring blacklisted package : ${package}.`);
            continue;
        }
        publish(package, args.dry);
    }

}

run()