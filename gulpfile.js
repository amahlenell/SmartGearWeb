const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const cleanCSS = require('gulp-clean-css');
const rename = require('gulp-rename');
const concat = require('gulp-concat');
const terser = require('gulp-terser');

const paths = {
    scssWatch: 'wwwroot/scss/**/*.scss',
    scssEntry: 'wwwroot/scss/site.scss',
    cssDest: 'wwwroot/css',
    js: 'wwwroot/js/site.js',
    jsDest: 'wwwroot/js'
};

// Compiles SASS -> CSS
function compileSass() {
    return gulp.src(paths.scssEntry)
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(paths.cssDest));
}

// Minifies the compiled CSS into site.min.css
function minifyCss() {
    return gulp.src(`${paths.cssDest}/site.css`)
        .pipe(cleanCSS())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.cssDest));
}

// Bundles and minifies JavaScript into site.min.js
function minifyJs() {
    return gulp.src(paths.js)
        .pipe(concat('site.min.js'))
        .pipe(terser())
        .pipe(gulp.dest(paths.jsDest));
}

const build = gulp.series(compileSass, minifyCss, minifyJs);

// Watches SCSS and JS source files, rebuilding automatically on save
function watchFiles() {
    gulp.watch(paths.scssWatch, gulp.series(compileSass, minifyCss));
    gulp.watch(paths.js, minifyJs);
}

exports.sass = compileSass;
exports.minifyCss = minifyCss;
exports.minifyJs = minifyJs;
exports.build = build;
exports.watch = gulp.series(build, watchFiles);
exports.default = build;
