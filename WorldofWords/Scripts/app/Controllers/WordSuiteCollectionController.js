app.controller('WordSuiteCollectionController', ["$scope", "CourseService", function ($scope, CourseService) {
    CourseService.getEnrollCourses()
    .then(function (courses) {
        $scope.wordsuites = [];
        var courseCount = courses.length;
        for (var i = 0; i < courseCount; i++) {
            var courseId = courses[i].Id;
            CourseService.getCourseDetail(courseId)
            .then(function (courseInfo) {
                for (var j = 0; j < courseInfo.WordSuites.length; j++) {
                    var newWordSuite = {
                        title: courseInfo.WordSuites[j].Name,
                        progress: courseInfo.WordSuites[j].Progress
                    };
                    $scope.wordsuites.push(newWordSuite);
                }
            });
        }
    });
}]);