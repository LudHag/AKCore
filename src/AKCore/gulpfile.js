/// <binding BeforeBuild='less, scripts' />
var gulp = require('gulp');
var less = require('gulp-less');
var sourcemaps = require('gulp-sourcemaps');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var autoprefixer = require('gulp-autoprefixer');
var path = require('path');
var plumber = require('gulp-plumber');

gulp.task('less', function () {
    return gulp.src('./Styles/akstyle.less')
    .pipe(plumber())
    .pipe(less({
        paths: [path.join(__dirname, 'less', 'includes')]
    }))
    .pipe(autoprefixer())
    .pipe(gulp.dest('./wwwroot/css/'));
});
gulp.task('scripts', function () {
    return gulp.src([
        "Scripts/jquery-2.2.3.min.js",
        "Scripts/bootstrap.min.js",
        "Scripts/jquery-ui.js",
        "Scripts/general.js",
        "Scripts/menuedit.js",
        "Scripts/pageedit.js",
        "Scripts/profile.js",
        "Scripts/users.js",
        "Scripts/media.js",
        "Scripts/events.js",
        "Scripts/upcomming.js",
        "Scripts/album.js",
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