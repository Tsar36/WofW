app.controller('UserCoursesController', ["$modal", "$scope", "$window", "CourseService", 
    function ($modal, $scope, $window, CourseService) {

    $scope.topCount = 3;

    $scope.confirmRemoving = function (course) {
        $scope.courseId = course.Id;
        $scope.open('confirmModal', $scope.removeCourse);
    }
    $scope.removeCourse = function () {
        CourseService.removeCourse($scope.courseId)
                .then(function (response) {
                    initialize();
                },
            function (error) {
                $scope.open('errorModal', $scope.goToCourses, (error.ExceptionMessage || error.Message || "Unknown error"));
            });
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

    var initialize = function () {
        CourseService.getUserCourses()
        .then(function (response) {
            $scope.courses = response;
        });
    };

    initialize();



}]);

