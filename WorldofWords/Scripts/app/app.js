/// <reference path="../angular.min.js" />
var app = angular.module('MyApp', ['ngRoute', 'ui.bootstrap', 'ang-drag-drop', 'angular-virtual-keyboard', 'toastr', 'isteven-multi-select', 'ngMockE2E']);
app.config(["$routeProvider", function ($routeProvider) {
    $routeProvider

        // by default, redirect to home page
        .when('/', {
            redirectTo: '/Home',
            data: {
                privateData: true
            }
        })

        // route for the home page
        .when('/Home', {
            templateUrl: 'Views/Home.html',
            data: {
                privateData: true
            }
        })
        .when('ForgotPasswordModal', {
            templateUrl: 'Views/ForgotPasswordModal.html',
            controller: 'ResetPasswordController',
            data: {
                privateData: true
            }
        })
        // route for the courses page
        .when('/Courses', {
            templateUrl: 'Views/Courses.html',
            controller: 'CourseController',
            data: {
                privateData: false
            }
        })
        // route for the user courses page
        .when('/UserCourses', {
            templateUrl: 'Views/UserCourses.html',
            controller: 'UserCoursesController',
            data: {
                privateData: false
            }
        })
        // route for the create courses page
        .when('/CreateCourse', {
            templateUrl: 'Views/CreateCourse.html',
            controller: 'CreateCourseController',
            data: {
                privateData: false
            }
        })
        // route for the edit courses page
        .when('/EditCourse/:courseId', {
            templateUrl: 'Views/EditCourse.html',
            controller: 'EditCourseController',
            data: {
                privateData: false
            }
        })
        //route for my page
        .when('/MyPage', {
            templateUrl: 'Views/MyPage.html',
            controller: 'MyPageController',
            data: {
                privateData: false
            }
        })
        //route for email sent page
        .when('/EmailSentPage', {
            templateUrl: 'Views/EmailSentPage.html',
            data: {
                privateData: true
            }
        })
        //
        .when('/EmailConfirmed', {
            templateUrl: 'Views/EmailConfirmed.html',
            controller: 'EmailConfirmedController',
            data: {
                privateData: true
            }
        })
        //route for tutor page
        .when('/Tutor/:wordSuiteId', {
            templateUrl: 'Views/Tutor.html',
            controller: 'TutorController',
            data: {
                privateData: false
            }
        })
        //route for tutor with images page
        .when('/TutorPicture/:wordSuiteId', {
            templateUrl: 'Views/TutorPicture.html',
            controller: 'TutorController',
            data: {
                privateData: false
            }
        })
        //route for tutor with records page
        .when('/TutorRecord/:wordSuiteId', {
            templateUrl: 'Views/TutorRecord.html',
            controller: 'TutorController',
            data: {
                privateData: false
            }
        })
        //route for wordsuite words page
        .when('/WordSuiteWords/:wordSuiteId', {
            templateUrl: 'Views/WordSuiteWords.html',
            controller: 'WordSuiteWordsController',
            data: {
                privateData: false
            }
        })
        // route for the Create WordSuite page
        .when('/CreateWordSuite', {
            templateUrl: 'Views/CreateWordSuite.html',
            controller: 'CreateWordSuiteController',
            data: {
                privateData: false
            }
        })
        // route for the Edit WordSuite page
        .when('/EditWordSuite', {
            templateUrl: 'Views/EditWordSuite.html',
            controller: 'EditWordSuiteController',
            data: {
                privateData: false
            }
        })
        // route for configuring group
        .when('/AddGroup', {
            templateUrl: 'Views/AddGroup.html',
            controller: 'AddGroupController',
            data: {
                privateData: false
            }
        })
         .when('/PublicGroups', {
             templateUrl: 'Views/PublicGroups.html',
             controller: 'PublicGroupsController',
             data: {
                 privateData: false
             }
         })

        // route for showing words
        .when('/Words', {
            templateUrl: 'Views/Words.html',
            controller: 'WordsController',
            data: {
                privateData: false
            }
        })

        // route for adding words
        .when('/AddWord', {
            templateUrl: 'Views/AddWord.html',
            controller: 'WordsController',
            data: {
                privateData: false
            }
        })

        // route for the languages page
        .when('/Languages', {
            templateUrl: 'Views/Languages.html',
            controller: 'LanguageController',
            data: {
                privateData: false
            }
        })

        // route for the quiz page
        .when('/Quiz/:wordSuiteId', {
            templateUrl: 'Views/Quiz.html',
            controller: 'QuizController',
            data: {
                privateData: false
            }
        })
        // route for the synonym quiz page
        .when('/SynonymQuiz/:wordSuiteId', {
            templateUrl: 'Views/SynonymQuiz.html',
            controller: 'SynonymQuizController',
            data: {
                privateData: false
            }
        })
        // route for the mix quiz page
        .when('/MixQuiz/:wordSuiteId', {
            templateUrl: 'Views/QuizMix.html',
            controller: 'MixQuizController',
            data: {
                privateData: false
            }
        })
        // route for the description quiz page
        .when('/QuizDesc/:wordSuiteId', {
            templateUrl: 'Views/QuizDesc.html',
            controller: 'QuizDescController',
            data: {
                privateData: false
            }
        })
        // route for the picture quiz page
        .when('/PictureQuiz/:wordSuiteId', {
            templateUrl: 'Views/PictureQuiz.html',
            controller: 'PictureQuizController',
            data: {
                privateDate: false
            }
        })
        // route for the groups page 
        .when('/Groups', {
            templateUrl: 'Views/Groups.html',
            controller: 'GroupController',
            data: {
                privateData: false
            }
        })
        .when('/RequestsToSubscribe', {
            templateUrl: 'Views/RequestsToSubscribe.html',
            controller: 'RequestsToSubscribe',
            data: {
                privateData: false
            }
        })
        // route for the wordsuites page 
        .when('/WordSuites', {
            templateUrl: 'Views/WordSuites.html',
            controller: 'WordSuitesController',
            data: {
                privateData: false
            }
            //route for the import wordtranslations page
        }).when('/ImportWordTranslations', {
            templateUrl: 'Views/ImportWordTranslations.html',
            controller: 'ImportWordTranslationsController',
            data: {
                privateData: false
            }
        })
        //route for creating wordtranslations
        .when('/CreateWordTranslation', {
            templateUrl: 'Views/CreateWordTranslation.html',
            controller: 'CreateWordTranslationController'
        })
        // route for configuring group page
        .when('/Groups/:groupId', {
            templateUrl: 'Views/GroupDetails.html',
            controller: 'GroupDetailsController',
            data: {
                privateData: false
            }
        })
        // route for progress chart
        .when('/Groups/:groupId/:userId/chart', {
            templateUrl: 'Views/StudentProgressChart.html',
            controller: 'StudentProgressChartController',
            data: {
                privateData: false
            }
        })
        // route for extended word suite by student adding words over default suite
        .when('/ExtendedWordSuite/:groupId/:suiteId', {
            templateUrl: 'Views/ExtendedWordSuite.html',
            controller: 'ExtendedWordSuiteController',
            data: {
                privateData: false
            }
        })
        // route for progress of student
        .when('/Groups/:groupId/:userId', {
            templateUrl: 'Views/StudentProgress.html',
            controller: 'StudentProgressController',
            data: {
                privateData: false
            }
        })
        // route for editing wordsuite page
        .when('/EditWordSuite/:wordSuiteId', {
            templateUrl: 'Views/EditWordSuite.html',
            controller: 'EditWordSuiteController',
            data: {
                privateData: false
            }
        })
        .when('/EditUserProfile', {
            templateUrl: 'Views/EditUserProfile.html',
            controller: 'EditUserProfileController',
            data: {
                privateData: false
            }
        })
        //route for course detail
        .when('/CourseDetail/:Id', {
            templateUrl: 'Views/CourseDetail.html',
            controller: 'CourseDetailController',
            data: {
                privateData: false
            }
        })
        //route for list of users
        .when('/Users', {
            templateUrl: 'Views/Users.html',
            controller: 'UsersConfiguringController',
            data: {
                privateData: false
            }
        })
        //route for Global Dictionary
        .when('/GlobalDictionary', {
            templateUrl: 'Views/GlobalDictionary.html', //однакове
            controller: 'GlobalDictionaryController',
            data: {
                privateData: false
            }
        })
        .when('RegisterModal', {
            templateUrl: 'Views/RegisterModal.html',
            controller: 'RegisterController',
            data: {
                privateData: true
            }
        })
        .when('/ChangePassword', {
            templateUrl: 'Views/ChangePassword.html',
            controller: 'ChangePasswordController',
            data: {
                privateData: true
            }
        })
        //route for Global Dictionary
        .when('/GlobalDictionary', {
            templateUrl: 'Views/GlobalDictionary.html', //однакове
            controller: 'GlobalDictionaryController',
            data: {
                privateData: false
            }
        })
        .when('LoginModal', {
            templateUrl: 'Views/LoginModal.html',
            controller: 'LoginController',
            data: {
                privateData: true
            }
        })
        .when('UnassignedModal', {
            templateUrl: 'Views/UnassignedModal.html',
            controller: 'UnassignedModalController',
            data: {
                privateData: false
            }
        })
        .when('/PasswordChanged', {
            templateUrl: 'Views/PasswordChanged.html',
            data: {
                privateData: true
            }
        })
        .when('/UnassignedModal', {
            templateUrl: 'Views/UnassignedModal.html',
            data: {
                privateData: false
            }
        })
        .when('/TeacherManager', {
            templateUrl: 'Views/TeacherManager.html',
            controller: 'TeacherManagerController',
            data: {
                privateData: false
            }
        })
        .when('/Tickets', {
            templateUrl: 'Views/Tickets.html',
            controller: 'TicketController',
            data: {
                privateData: false
            }
        });
}])
.config(['VKI_CONFIG', function (VKI_CONFIG) {
    layoutConfig(VKI_CONFIG);
    deadKeysConfig(VKI_CONFIG);
    // Because of not custom element breaks the design
    VKI_CONFIG.customKeyboardInputInitiator = true;
    // The element that represents a button to initiate virtual keyboard input (shows keyboard) 
    VKI_CONFIG.keyboardInputInitiator =
            "<span class='input-group-btn'>"
                    + "<button class='btn btn-default btn-block'>"
                        + "<img src='../Content/keyboard.png'>"
                    + "</button>"
            + "</span>";
}])
.config(['toastrConfig', function (toastrConfig) {
    angular.extend(toastrConfig, {
        positionClass: 'toast-bottom-right',
        allowHtml: true,
        closeButton: true,
        closeHtml: '<button>&times;</button>',
        extendedTimeOut: 3000,
        iconClasses: {
            error: 'toast-error',
            info: 'toast-info',
            success: 'toast-success',
            warning: 'toast-warning'
        },
        messageClass: 'toast-message',
        onHidden: null,
        onShown: null,
        onTap: null,
        progressBar: false,
        tapToDismiss: true,
        templates: {
            toast: 'directives/toast/toast.html',
            progressbar: 'directives/progressbar/progressbar.html'
        },
        timeOut: 5000,
        titleClass: 'toast-title',
        toastClass: 'toast'
    });
}]);

app.run(["$rootScope", "$modal", "UserService", "ConstService", "$location", "HubService", "TicketService",
    function ($rootScope, $modal, UserService, ConstService, $location, HubService, TicketService) {
        $rootScope.$on("$routeChangeSuccess", function (event, next) {
            HubService.initialize();
            var userInfo = UserService.getUserData();
            if (!userInfo) {
                $location.replace("/Index#/Home");
                HubService.stop();
                $rootScope.isPrivate = next.data.privateData;
                $rootScope.$broadcast('hideSideBar');
                if (!$rootScope.isPrivate) {
                    $location.replace('/Index#/Home');
                    $modal.open({
                        templateUrl: '../Views/UnauthorizeModal.html',
                        controller: 'UnauthorizeModalController',
                        size: ConstService.small
                    });
                    $rootScope.isPrivate = true;
                } else {
                    $rootScope.isPrivate = true;
                };
            }
        });
    }]);


//// TODO : Delete when the partOfSpeech backEnd will be ready
app.run(["$httpBackend", function ($httpBackend) {
    //var partsOfSpeech = [
    //        {
    //            id: 1,
    //            name: 'noun',
    //            shortName: 'n',
    //            selected: true
    //        }, {
    //            id: 2,
    //            name: 'adverb',
    //            shortName: 'a',
    //            selected: true
    //        }, {
    //            id: 3,
    //            name: 'verb',
    //            shortName: 'v',
    //            selected: true
    //        }
    //];

    //$httpBackend.whenGET(new RegExp('\\/Language\\/partsOfSpeech\\?id=[0-9]+')).respond(partsOfSpeech);
    $httpBackend.whenPOST().passThrough();
    $httpBackend.whenGET().passThrough();
    $httpBackend.whenDELETE().passThrough();
    $httpBackend.whenPUT().passThrough();
}]);

