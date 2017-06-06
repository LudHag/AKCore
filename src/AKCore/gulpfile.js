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
gulp.task('scripts', function () {
    return gulp.src([
        "Scripts/jquery-ui.js",
        "Scripts/jquery.multi-select.js",
        "Scripts/general.js",
        "Scripts/menuedit.js",
        "Scripts/pageedit.js",
        "Scripts/profile.js",
        "Scripts/users.js",
        "Scripts/media.js",
        "Scripts/events.js",
        "Scripts/upcomming.js",
        "Scripts/memberlist.js",
        "Scripts/recruits.js",
        "Scripts/album.js",
        "Scripts/musicplayer.js",
        "Scripts/youtubegallerywall.js"
    ])
	.pipe(sourcemaps.init())
	.pipe(concat('main.js'))
	.pipe(uglify())
	.pipe(sourcemaps.write('./'))
	.pipe(gulp.dest('./wwwroot/js/'));
});

gulp.task('watch', function () {
    gulp.watch('./Scripts/*.js', ['scripts']);
    gulp.watch('./Styles/*.less', ['less']);
});