// Karma configuration
// Generated on Mon Jun 16 2014 15:04:49 GMT+1000 (AUS Eastern Standard Time)

module.exports = function(config) {
  config.set({

      plugins: [
      'karma-jasmine', 
      'jasmine-core',
      'karma-phantomjs-launcher',
      'karma-xml-reporter'
       ],

      // frameworks to use , 'browserify'
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
      frameworks: ['jasmine'],

    // list of files / patterns to load in the browser
    files: [
      'Scripts/angular/angular.js',
      'Scripts/isteven-multi-select.js',
      'Scripts/angular/angular-mocks.js',
      'Scripts/angular/angular-route.js',
      'Scripts/angular-ui/ui-bootstrap.js',
      'Scripts/libs/angular-virtual-keyboard.js',
      'Scripts/libs/vki-layouts.js',
      'Scripts/libs/vki-deadkeys.js',
      'Scripts/jquery/jquery-2.1.4.js',
      'Scripts/jquery.signalR-2.2.0.js',
      'Scripts/angular/angular-toastr.js',
      'Scripts/app/app.js',
      'Scripts/app/*/*.js',
      'TestScripts/*.js',
    ],

    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress', 'xml'],

    // web server port
    port: 9876,

    // enable / disable colors in the output (reporters and logs)
    colors: true,

    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,

    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: false,

    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['PhantomJS'],

    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: true
  });
};
