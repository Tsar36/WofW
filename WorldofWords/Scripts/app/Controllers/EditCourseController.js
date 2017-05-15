app.controller('EditCourseController', ["$modal", "$scope", "$window", "$routeParams", "$timeout", "WordSuiteService", "CourseService","HubService",
    function ($modal, $scope, $window, $routeParams, $timeout, WordSuiteService, CourseService, HubService) {

    $scope.dropSuccessHandler = function ($event, index, array) {
        array.splice(index, 1);
    };

    $scope.onDrop = function ($event, $data, array) {
        array.push($data);
    };

    $scope.saveCurrentState = function () {
        localStorage.setItem('course', JSON.stringify($scope.course));
        localStorage.setItem('Location', window.location.href);
        $window.location.href = 'Index#/CreateWordSuite';
    }

    $scope.goToCourses = function () {
        $window.location.href = 'Index#/UserCourses';
    }

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

    $scope.enableTooltip = function () {
        return !(Boolean($scope.course.Name) &&
            Boolean($scope.course.WordSuites.length));
    };

    $scope.saveCourse = function () {
        if (isCorrectInput()) {
            HubService.notifyAboutCourseChange($scope.course.Id, $scope.course.Name);
            CourseService
                .editCourse($scope.course)
                .then(function (ok) {
                    $scope.open('Modal', $scope.goToCourses, { header: 'Edit Course', message: 'Course successfully saved' }, $scope.goToCourses);
                },
                    function (badRequest) {
                        $scope.open('Modal', $scope.isCorrectInput, { header: 'Edit Course', message: (error.ExceptionMessage || error.Message || "Unknown error") });
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
        $scope.isSelectedWordSuites = Boolean($scope.course.WordSuites.length);
        return $scope.isEnteredName && $scope.isSelectedWordSuites;
    }

    var initialize = function () {
        HubService.initialize();
        CourseService.getCourse($routeParams.courseId)
            .then(function (response) {
                $scope.course = response;
            })
            .then(function () {
                WordSuiteService.getWordSuitesByLanguageID($scope.course.Language.Id)
                    .then(function (response) {
                        $scope.wordSuites = response.filter(function (obj) {
                            return (!contain($scope.course.WordSuites, obj));
                        });
                    })
                    .then(function () {
                        if (localStorage.getItem('Location')) {
                            localStorage.removeItem('Location');
                            $scope.course = JSON.parse(localStorage.getItem('course'));
                            if (JSON.parse(localStorage.getItem('course')).WordSuites.length) {
                                $scope.wordSuites = $scope.wordSuites.filter(function (obj) {
                                    return !contain($scope.course.WordSuites, obj);
                                });
                            }
                        };
                    });
            });
    };

    initialize();

}]);