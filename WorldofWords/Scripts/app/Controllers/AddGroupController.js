app.controller('AddGroupController', ["$scope", "$window", "GroupService", "UserService",
    function ($scope, $window, GroupService, UserService) {
    $scope.showSuccess = false;
    $scope.showFailure = false;
    $scope.showFailureEmptyName = false;
    $scope.failureEmptyNameMessage = 'Please enter the name for your group!';

    $scope.init = function () {
        GroupService.getAllCourses()
        .then(function (response) {
            $scope.names = response;
            if ($scope.names.length > 0) {
                $scope.courseIdValue = $scope.names[0].Id;
            }
        });
    }

    $scope.submitButtonClick = function () {
        $scope.hideSuccess();
        $scope.hideFailure();
        $scope.hideFailureEmptyName();
        var groupModel = {
            name: $scope.nameValue,
            courseId: $scope.courseIdValue,
            ownerId: UserService.getUserData().id
        };
        if (groupModel.name) {
            GroupService.createGroup(groupModel)
            .then(function (ok) {
                $scope.notifySuccess();
            }, function (badRequest) {
                $scope.notifyFailure(badRequest.Message);
            });
        }
        else {
            $scope.notifyFailureEmptyName();
        }
    }

    $scope.notifySuccess = function () {
        $scope.successMessage = 'Group ' + $scope.nameValue + ' has been added.';
        $scope.showSuccess = true;
    }

    $scope.notifyFailure = function (failureMessage) {
        if (failureMessage == null) {
            $scope.failureMessage = 'SomethingWrong';            
        }
        else {
            $scope.failureMessage = failureMessage;
        }
        $scope.showFailure = true;
    }

    $scope.notifyFailureEmptyName = function () {
        $scope.showFailureEmptyName = true;
    }

    $scope.hideSuccess = function () {
        $scope.showSuccess = false;
    }

    $scope.hideFailure = function () {
        $scope.showFailure = false;
    }
     
    $scope.hideFailureEmptyName = function () {
        $scope.showFailureEmptyName = false;
    }

    $scope.init();
}]);
