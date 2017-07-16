// grab our gulp packages
var gulp  = require('gulp'),
    gutil = require('gulp-util');

// create a default task and just log a message
gulp.task('default', function() {
  
  // copy any js files in dist/ to public/js/lib/angular
  gulp.src([
    './dist/**/*.*'
])
.pipe(gulp.dest('../wwwroot/instructor/dist'));
  return gutil.log('Gulp has finished copying files!')
});
