const parser = require('args-parser');

const { listChangedPackages } = require('./utils');

function run() {
    let args = parser(process.argv);
    let packages = listChangedPackages(args.tag);
    console.log(packages);
}

run()
