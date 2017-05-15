app.controller('StudentProgressController', ["$scope", "$window", "$routeParams", "GroupService", "CourseService", "EditUserProfileSevice", 
    function ($scope, $window, $routeParams, GroupService, CourseService, EditUserProfileSevice) {
    $scope.init = function () {
        $scope.isPassedValueValid = isInteger($routeParams.groupId) && isInteger($routeParams.userId);
        if ($scope.isPassedValueValid) {
            EditUserProfileSevice.getUserNameById($routeParams.userId)
                .then(function (result) {
                    $scope.userName = result;
                    GroupService.getGroupById($routeParams.groupId)
                        .then(function (response) {
                            $scope.group = response;
                            if ($scope.group == null) {
                                $scope.isPassedValueValid = false;
                            }
                            else {
                                CourseService.getCourseDetailWithUserId($scope.group.CourseId, $routeParams.userId)
                                .then(function (response) {
                                    $scope.course = response;
                                });
                            }
                        });
                }, function (error) {
                    $scope.isPassedValueValid = false;
                });
            $scope.userId = $routeParams.userId;
        }
    }

    var isInteger = function (nVal) {
        var reg = /^\d+$/;
        return reg.test(nVal);
    };

    $scope.init();
}]);