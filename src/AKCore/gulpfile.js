/// <binding BeforeBuild='sass, scripts' />
var gulp = require('gulp');
var less = require('gulp-less');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var autoprefixer = require('gulp-autoprefixer');
var path = require('path');
var plumber = require('gulp-plumber');

gulp.task('sass', function () {
    return gulp.src('./Styles/akstyle.scss')
        .pipe(plumber())
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer())
        .pipe(gulp.dest('./wwwroot/css/'));
});
gulp.task('scripts', ['generalscripts','adminscripts']);

gulp.task('generalscripts', function () {
    return gulp.src([
        "Scripts/Vendor/jquery-ui.js",
        "Scripts/Vendor/jquery.multi-select.js",
        "Scripts/Vendor/youtubegallerywall.js",
        "Scripts/general.js",
        "Scripts/profile.js",
        "Scripts/upcomming.js",
        "Scripts/memberlist.js",
        "Scripts/musicplayer.js"
    ])
        .pipe(sourcemaps.init())
        .pipe(concat('main.js'))
        .pipe(uglify())
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./wwwroot/js/'));
});

gulp.task('adminscripts', function () {
    return gulp.src([
        "Scripts/Admin/menuedit.js",
        "Scripts/Admin/pageedit.js",
        "Scripts/Admin/users.js",
        "Scripts/Admin/media.js",
        "Scripts/Admin/events.js",
        "Scripts/Admin/recruits.js",
        "Scripts/Admin/hires.js",
        "Scripts/Admin/album.js"
    ])
        .pipe(sourcemaps.init())
        .pipe(concat('admin.js'))
        .pipe(uglify())
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./wwwroot/js/'));
});

gulp.task('watch', function () {
    gulp.watch('./Scripts/**/*.js', ['scripts']);
    gulp.watch('./Styles/*.scss', ['sass']);
});