app.controller('CourseDetailController', ["$scope", "$window", "$routeParams", "UserService", "CourseService","ConstService",
    function ($scope, $window, $routeParams, UserService, CourseService, ConstService) {
        'use strict';
        var initialize = function () {
            $scope.quizIndexes = ConstService.quizIndexes;
            localStorage.setItem('courseId', JSON.stringify($routeParams.Id));
            CourseService.getCourseDetailWithUserId($routeParams.Id, UserService.getUserData().id)
                .then(function (response) {
                    $scope.course = response;
                    $scope.quizFlags = CourseService.getDisabledQuizzes($scope.course.WordSuites);
                });
        };
        initialize();
    }]);