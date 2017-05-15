/// <vs BeforeBuild='dev-build' />
/// <binding BeforeBuild='dev-build' />
/// <vs BeforeBuild='dev-build' />
module.exports = function (grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),

        clean: {
            build: {
                src: ['dist']
            },
        },

        concat: {
            options: {
                // define a string to put between each file in the concatenated output
                separator: '\n;'
            },
            dev_build: {
                files: {
                    'dist/scripts-concat.js':
                        [
                            "Scripts/angular/angular.js", "Scripts/angular-ui/ui-bootstrap.js", "Scripts/jquery/jquery-2.1.4.js", "Scripts/nicescroll.js", "Scripts/libs/angular-virtual-keyboard.js",
                            "Scripts/libs/vki-layouts.js", "Scripts/libs/vki-deadkeys.js", "Scripts/libs/sha512.js", "Scripts/libs/Timer.js", 

                            "Scripts/jquery.signalR-2.2.0.js", "Scripts/angular/angular-toastr.js", "Scripts/angular/angular-toastr.tpls.js",

                            "Scripts/app/app.js", 'Scripts/app/Controllers/*.js', "Scripts/app/Directives/*.js", "Scripts/app/Filters/*.js", "Scripts/app/Services/*.js",

                            "Scripts/angular/angular-route.min.js", "Scripts/angular-ui/ui-bootstrap-tpls.min.js", "Scripts/jquery/jquery.scrollTo.min.js", "Scripts/libs/canvasjs.min.js",
                            "Scripts/libs/bootstrap.min.js", "Scripts/libs/lodash.min.js", "Scripts/isteven-multi-select.js", "Scripts/angular/angular-mocks.js"
                        ],

                    'dist/styles-concat-dark-theme.css':
                        ["Scripts/libs/angular-virtual-keyboard.css", "Content/drop-down-dark-theme.css", "Content/sidebar-dark-theme.css", "Content/bootstrap.min.css", "Content/bootstrap-theme.min.css", "Content/metro-bootstrap-dark-theme.min.css", "Content/angular-toastr.css", "Content/recorder.css", "Content/PictureQuizStyles.css", "Content/isteven-multi-select.css"],

                    'dist/styles-concat-blue-theme.css':
                    ["Scripts/libs/angular-virtual-keyboard.css", "Content/drop-down-blue-theme.css", "Content/sidebar-blue-theme.css", "Content/bootstrap.min.css", "Content/bootstrap-theme.min.css", "Content/metro-bootstrap-blue-theme.min.css", "Content/angular-toastr.css", "Content/recorder.css", "Content/isteven-multi-select.css", "Content/PictureQuizStyles.css"],
                }

            },

            prod_build: {
                options: {
                    sourceMap: true,
                    expand: true
                },
                files: {
                    "dist/scripts-concat.js":
                          [
                            "Scripts/angular/angular.js", "Scripts/angular-ui/ui-bootstrap.js", "Scripts/jquery/jquery-2.1.4.js", "Scripts/nicescroll.js", "Scripts/libs/angular-virtual-keyboard.js",
                            "Scripts/libs/vki-layouts.js", "Scripts/libs/vki-deadkeys.js", "Scripts/libs/sha512.js", "Scripts/libs/Timer.js",

                            "Scripts/jquery.signalR-2.2.0.js", "Scripts/angular/angular-toastr.js", "Scripts/angular/angular-toastr.tpls.js",

                            "Scripts/app/app.js", 'Scripts/app/Controllers/*.js', "Scripts/app/Directives/*.js", "Scripts/app/Filters/*.js", "Scripts/app/Services/*.js",

                            "Scripts/angular/angular-route.min.js", "Scripts/angular-ui/ui-bootstrap-tpls.min.js", "Scripts/jquery/jquery.scrollTo.min.js", "Scripts/libs/canvasjs.min.js",
                            "Scripts/libs/bootstrap.min.js", "Scripts/libs/lodash.min.js", "Scripts/isteven-multi-select.js", "Scripts/angular/angular-mocks.js"
                          ],
                    'dist/styles-concat-dark-theme.css':
                        ["Scripts/libs/angular-virtual-keyboard.css", "Content/drop-down-dark-theme.css", "Content/sidebar-dark-theme.css", "Content/bootstrap.min.css", "Content/bootstrap-theme.min.css", "Content/metro-bootstrap-dark-theme.min.css", "Content/angular-toastr.css", "Content/recorder.css", "Content/PictureQuizStyles.css", "Content/isteven-multi-select.css"],

                    'dist/styles-concat-blue-theme.css':
                    ["Scripts/libs/angular-virtual-keyboard.css", "Content/drop-down-blue-theme.css", "Content/sidebar-blue-theme.css", "Content/bootstrap.min.css", "Content/bootstrap-theme.min.css", "Content/metro-bootstrap-blue-theme.min.css", "Content/angular-toastr.css", "Content/recorder.css", "Content/isteven-multi-select.css", "Content/PictureQuizStyles.css"],                 
                }

            },
        },

        uglify: {
            options: {
                sourceMap: true
            },
            "scripts-minification": {
                files: {
                    "dist/prod_build.min.js": [
                        "Scripts/nicescroll.js", "Scripts/libs/angular-virtual-keyboard.js",
                        "Scripts/libs/vki-layouts.js", "Scripts/libs/vki-deadkeys.js", "Scripts/libs/sha512.js", "Scripts/libs/Timer.js",
                        "Scripts/app/app.js", 'Scripts/app/Controllers/*.js', "Scripts/app/Directives/*.js", "Scripts/app/Filters/*.js", "Scripts/app/Services/*.js"]
                }
            }
        },

        cssmin: {
            "css-minification": {
                files: {
                    "dist/prod_build.min.css": ["Scripts/libs/angular-virtual-keyboard.css","Content/sidebar.css"]
                }
            }
        },
        karma: {
            unit: {
                configFile: 'karma.conf.js'
            }
        },
        jshint: {
            options: {
                jshintrc: '.jshintrc',
                reporter: require('jshint-stylish')
            },
            all: {
                src: [
                  'Gruntfile.js',
                  'Scripts/app/{,*/}*.js'
                ]
            }
        }
    });
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks("grunt-contrib-concat");
    grunt.loadNpmTasks("grunt-contrib-cssmin");
    grunt.loadNpmTasks("grunt-contrib-uglify");
    grunt.loadNpmTasks("grunt-karma");

    grunt.registerTask("dev-build", ["clean", "concat:dev_build"]);

    grunt.registerTask("prod-build", ["clean", "uglify", "cssmin", "concat:prod_build"]);
};
