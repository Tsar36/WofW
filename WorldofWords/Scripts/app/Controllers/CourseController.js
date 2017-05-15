app.controller('CourseController', ["$modal", "$scope", "$window", "CourseService", "UserService", "ConstService",
    function ($modal, $scope, $window, CourseService, UserService, ConstService) {
        var contain = function (role, obj) {
            for (var i = ConstService.zero; i < role.length; i++) {
                if (role[i] === obj) {
                    return true;
                };
            };
            return false;
        };

        var initialize = function () {
            $scope.topCount = 5;
            var userData = UserService.getUserData();
            if (userData)
                CourseService.getEnrollCourses()
                    .then(function (response) {
                        $scope.enrollCourses = response;
                        if (!response.length && contain(userData.roles, ConstService.StudentRole)
                            && !contain(userData.roles, ConstService.TeacherRole)
                            && !contain(userData.roles, ConstService.AdminRole)) {
                                location.replace('Index#/UnassignedModal');
                        };
                    });
        };

        initialize();
    }]);