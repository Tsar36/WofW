app.controller('UsersConfiguringController', ["$scope", "$window", "UserService", "ConstService", 
    function ($scope, $window, UserService, ConstService) {
    $scope.transformUsers = function (usersInServerForm) {
        var result = [];
        for (var i = 0; i < usersInServerForm.length; ++i) {
            result.push({
                Id: usersInServerForm[i].Id,
                Name: usersInServerForm[i].Name,
                Email: usersInServerForm[i].Email,
                isAdmin: $scope.checkIfIsAdmin(usersInServerForm[i]),
                isStudent: $scope.checkIfIsStudent(usersInServerForm[i]),
                isTeacher: $scope.checkIfIsTeacher(usersInServerForm[i]),
                isBeingEdited: false
            });
        }
        return result;
    }

    $scope.init = function () {
        $scope.tempRole = 0;
        $scope.selectedPage = 1;
        $scope.valueToSearch = '';
        $scope.USERS_ON_PAGE = ConstService.USERS_ON_PAGE;
        $scope.ID_OF_ADMIN_ROLE = ConstService.ID_OF_ADMIN_ROLE;
        $scope.ID_OF_STUDENT_ROLE = ConstService.ID_OF_STUDENT_ROLE;
        $scope.ID_OF_TEACHER_ROLE = ConstService.ID_OF_TEACHER_ROLE;
        $scope.usersWhichAreBeingEdited = [];
        UserService.getAmountOfUsersByRoleId($scope.tempRole)
        .then(function (response) {
            $scope.amountOfUsersByTempRole = response;
            UserService.getUsersFromInterval(($scope.selectedPage - 1) * ConstService.USERS_ON_PAGE, Math.min($scope.selectedPage * ConstService.USERS_ON_PAGE, $scope.amountOfUsersByTempRole), $scope.tempRole)
            .then(function (response) {
                $scope.UserList = $scope.transformUsers(response);
            });
        });
    };

    $scope.search = function () {
        if ($scope.valueToSearch.length < 1) {
            $scope.changeDataToShow();
        } else {
            UserService.searchUserByName($scope.valueToSearch, $scope.tempRole)
            .then(function (response) {
                $scope.amountOfUsersByTempRole = response.length;
                $scope.UserList = $scope.transformUsers(response);
            });
        }
    }

    $scope.roleChanged = function () {
        $scope.selectedPage = 1;
        $scope.changeDataToShow();
    }

    $scope.changeDataToShow = function () {
        UserService.getAmountOfUsersByRoleId($scope.tempRole)
        .then(function (response) {
            $scope.amountOfUsersByTempRole = response;
            UserService.getUsersFromInterval(($scope.selectedPage - 1) * ConstService.USERS_ON_PAGE, Math.min($scope.selectedPage * ConstService.USERS_ON_PAGE, $scope.amountOfUsersByTempRole), $scope.tempRole)
            .then(function (response) {
                $scope.UserList = $scope.transformUsers(response);
            });
        });
    }

    $scope.checkIfIsAdmin = function (user) {
        for (var i = 0; i < user.Roles.length; ++i) {
            if (user.Roles[i].Name === ConstService.ADMIN_ROLE_NAME)
                return true;
        }
        return false;
    }

    $scope.checkIfIsTeacher = function (user) {
        for (var i = 0; i < user.Roles.length; ++i) {
            if (user.Roles[i].Name === ConstService.TEACHER_ROLE_NAME)
                return true;
        }
        return false;
    }

    $scope.checkIfIsStudent = function (user) {
        for (var i = 0; i < user.Roles.length; ++i) {
            if (user.Roles[i].Name === ConstService.STUDENT_ROLE_NAME)
                return true;
        }
        return false;
    }

    $scope.addToListOfUsersWhichAreBeingEdited = function (index) {
        $scope.UserList[index].isBeingEdited = true;
    }

    var changeUserToServerModel = function (user) {
        var roles = [];
        if (user.isAdmin === true) {
            roles.push({ Id: ConstService.ID_OF_ADMIN_ROLE, Name: ConstService.ADMIN_ROLE_NAME });
        }
        if (user.isTeacher === true) {
            roles.push({ Id: ConstService.ID_OF_TEACHER_ROLE, Name: ConstService.TEACHER_ROLE_NAME });
        }
        if (user.isStudent === true) {
            roles.push({ Id: ConstService.ID_OF_STUDENT_ROLE, Name: ConstService.STUDENT_ROLE_NAME });
        }
        return { Id: user.Id, Email: user.Email, Name: user.Name, Roles: roles };
    }

    $scope.removedFromListOfUsersWhichAreBeingChanged = function (index) {
        UserService.changeRolesOfUser(changeUserToServerModel($scope.UserList[index]))
        .then(function (response) { $scope.UserList[index].isBeingEdited = false; }, function (response) { ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageErrorOnServerSide, false); });
    }

    $scope.init();
}]);
