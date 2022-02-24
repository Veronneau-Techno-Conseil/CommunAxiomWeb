var browserify = require('browserify');
var tsify = require('tsify');
var watchify = require('watchify');
var fs = require('fs');
browserify()
    .add('./assets/ts/carousel.ts') // main entry of an application
    .plugin(tsify, { noImplicitAny: true })
    //.plugin(watchify)
    .bundle()
    .on('error', function (error) { console.error(error.toString()); })
    .pipe(fs.createWriteStream("./wwwroot/assets/js/carousel.js"));