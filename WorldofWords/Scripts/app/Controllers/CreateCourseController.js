app.controller('CreateCourseController',["$scope", "$window", "$modal", "$q", "UserService", "WordSuiteService", "LanguageService", "CourseService",
    function ($scope, $window, $modal, $q, UserService, WordSuiteService, LanguageService, CourseService) {

    $scope.dropSuccessHandler = function ($event, index, array) {
        array.splice(index, 1);
    };

    $scope.onDrop = function ($event, $data, array) {
        array.push($data);
    };

    $scope.enableTooltip = function () {
        return !(Boolean($scope.course.Name) &&
            Boolean($scope.course.Language) &&
            Boolean($scope.course.WordSuites.length));
    };

    $scope.goToCourses = function () {
        $window.location.href = 'Index#/UserCourses';
    };

    $scope.open = function (url, successCallback, data, errorCallback) {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: url,
            controller: 'ModalController',
            size: 'sm',
            resolve: {
                data: function () {
                    return data;
                }
            }
        });
        modalInstance.result.then(successCallback, errorCallback);
    };

    $scope.getWordSuitesForCurrentLanguage = function () {
        return $q(function (resolve) {
            WordSuiteService.getWordSuitesByLanguageID($scope.course.Language.Id)
                .then(function (response) {
                    $scope.wordSuites = response;
                    resolve($scope.wordSuites);
                });
        });
    };

    $scope.saveCurrentState = function () {
        localStorage.setItem('course', JSON.stringify($scope.course));
        localStorage.setItem('Location', window.location.href);
        $window.location.href = 'Index#/CreateWordSuite';
    }

    $scope.saveCourse = function() {
        if (isCorrectInput()) {
            var course = {
                Name: $scope.course.Name,
                LanguageId: $scope.course.Language.Id,
                OwnerId: UserService.getUserData().id,
                WordSuites: $scope.course.WordSuites
            }
            CourseService
                .createCourse(course)
                .then(function(ok) {
                        $scope.open('Modal', $scope.goToCourses, { header: 'Create Course', message: 'Course successfully added' }, $scope.goToCourses);
                    },
                    function(badRequest) {
                        $scope.open('Modal', $scope.isCorrectInput, { header: 'Create Course', message: (error.ExceptionMessage || error.Message || "Unknown error") });
                    });
        }
    };

    var contain = function (a, obj) {
        for (var i = 0; i < a.length; i++) {
            if (a[i].Id === obj.Id) {
                return true;
            }
        }
        return false;
    };

    var isCorrectInput = function () {
        $scope.isEnteredName = Boolean($scope.course.Name);
        $scope.isEnteredLanguage = Boolean($scope.course.Language);
        $scope.isSelectedWordSuites = Boolean($scope.course.WordSuites.length);
        return $scope.isEnteredName && $scope.isEnteredLanguage && $scope.isSelectedWordSuites;
    };

    var inilialize = function () {
        $scope.course = {
            Name: null,
            Language: null,
            WordSuites: []
        };
        $scope.wordSuites = [];
        LanguageService.getAllLanguages()
            .then(function (response) {
                $scope.languages = response;
            });
        if (localStorage.getItem('Location')) {
            localStorage.removeItem('Location');
            $scope.course = JSON.parse(localStorage.getItem('course'));
            if ($scope.course.Language) {
                $scope.getWordSuitesForCurrentLanguage()
                    .then(function () {
                        if (JSON.parse(localStorage.getItem('course')).WordSuites.length) {
                            $scope.wordSuites = $scope.wordSuites.filter(function (obj) {
                                return !contain($scope.course.WordSuites, obj);
                            });
                        }
                    });
            };
        };
    };

    inilialize();
}]);
