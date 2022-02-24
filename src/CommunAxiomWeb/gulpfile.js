const { watch, src, task, series, parallel, dest, env } = require('gulp'),
    sass = require('gulp-sass')(require('node-sass'))
    cssmin = require("gulp-cssmin")
    rename = require("gulp-rename");

var ts = require('gulp-typescript');
var tsProject = ts.createProject('./tsconfig.json');


task('min', function (done) {
    src('assets/scss/style.scss')
        .pipe(sass().on('error', sass.logError))
        //.pipe(cssmin())
        .pipe(rename({
            suffix: ".min"
        }))
        .pipe(dest('wwwroot/assets/css'));
    done();
});


task("serve", parallel(["min"]));
task("default", series("serve"));

task("watch", function (cb1) {
    watch(['assets/scss/**/*.scss', "assets/ts/**/*.ts"], { }, function (cb) {
        series("serve")(cb);
    });
});