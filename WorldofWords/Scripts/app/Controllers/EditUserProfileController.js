/// <reference path="IndexController.js" />
app.controller('EditUserProfileController',["$rootScope", "$scope", "$modal", "EditUserProfileSevice", "TicketService", "ConstService", "HubService", "toastr",
    function ($rootScope, $scope, $modal, EditUserProfileSevice, TicketService, ConstService, HubService, toastr) {
    var checkName;

    var clearPasswordFields = function () {
        $scope.userCurrentPassword = null;
        $scope.userNewPassword = null;
        $scope.userConfirmNewPassword = null;
    };

    var clearNameFields = function () {
        $scope.userNewName = null;
    };

    var toHideLabel = function () {
        $scope.showLabel = false;
        $scope.message = ConstService.savedSettings;
    };

    var toSetLabel = function (string) {
        $scope.message = string;
        $scope.showLabel = true;
    };

    var checkPasswords = function () {
        if (sessionStorage.strength >= ConstService.strength) {
            return ($scope.userCurrentPassword
                && ($scope.userNewPassword === $scope.userConfirmNewPassword)
                && ($scope.userNewPassword));
        }
        return false;
    };

    var nameStrength = function (nameAndSurname) {
        return (nameAndSurname[ConstService.name].length > ConstService.nameLength)
            && (nameAndSurname[ConstService.surname].length > ConstService.nameLength);
    }

    $scope.$watch('userNewPassword', function () {
        $scope.showStrenght = $scope.userNewPassword ? false : true;
    });

    $scope.$watch('userNewName', function () {
        if ($scope.userNewName) {
            var nameAndSurname = $scope.userNewName.split(' ');
            checkName = nameStrength(nameAndSurname) ? true : false;
        };
    });

    $scope.userName = EditUserProfileSevice.getUserName()
        .then(function (result) {
            $scope.userName = result;

        }, function () {
            $scope.userName = ConstService.nameNotFound;
        });

    $scope.hideData = function () {
        $scope.showName = false;
        $scope.showPassword = false;
        clearPasswordFields();
        clearNameFields();
        toHideLabel();
    };
    $scope.hideNameTitle = function () {
        $scope.showName = true;
        $scope.showPassword = false;
        clearNameFields();
        toHideLabel();
    };
    $scope.hidePasswordTitle = function () {
        $scope.showPassword = true;
        $scope.showName = false;
        clearPasswordFields();
        toHideLabel();
    };

    var userNameIsInBase = function () {
        EditUserProfileSevice.editUserName($scope.userNewName)
            .then(function () {
                $scope.showName = false;
                $scope.userName = $scope.userNewName;
                $scope.$parent.userName = $scope.userNewName;
                clearNameFields();
                toSetLabel(ConstService.savedSettings);
            },
                function () {
                    clearNameFields();
                    toSetLabel(ConstService.wrongName);
                });
        $scope.showName = false;
        toHideLabel();
    };
    var userPasswordIsInBase = function () {
        EditUserProfileSevice.passwordIsInBase($scope.userCurrentPassword)
            .then(function () {
                EditUserProfileSevice.editUserPassword($scope.userNewPassword)
            .then(function () {
                $scope.showPassword = false;
                clearPasswordFields();
                toSetLabel(ConstService.savedSettings);
            });

            },
                function () {
                    clearPasswordFields();
                    toSetLabel(ConstService.wrongPassword);
                });
        $scope.showPassword = false;
        toHideLabel();
    };

    $scope.submitChangeName = function () {
        if (checkName) {
            userNameIsInBase();
        } else {
            toSetLabel(ConstService.invalidName);
        };
    };

    $scope.submitChangePassword = function () {
        if (checkPasswords()) {
            userPasswordIsInBase();
        } else toSetLabel(ConstService.invalidConfirmPassword);
    };

    $scope.openSendRequestModal = function () {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'Views/SendRequest.html',
            controller: 'SendRequestController',
            size: 'md'
        });
    }

    $scope.getTickets = function () {
        TicketService.getUserTickets().then(
        function (response) {
            $scope.ticketsToShow = response;
        },
        function (error) {
            alert("error");
        });
    };
        
    function activate() {
        HubService.initialize();
    }

    activate();
    $scope.$on('updateTicketTable', function (e) {
        $scope.$apply(function () {
            $scope.getTickets();
        })
    });

    $scope.showTickets = function () {
        $scope.isExpanded = $scope.isExpanded == true ? false : true;
        if ($scope.isExpanded) {
            $scope.getTickets();
        }
    };
}]);