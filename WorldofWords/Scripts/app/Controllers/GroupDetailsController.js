app.controller('GroupDetailsController', ["$scope", "$modal", "$routeParams", "EnrollmentService", "GroupService", 
    function ($scope, $modal, $routeParams, EnrollmentService, GroupService) {
    $scope.init = function () {
        $scope.currentPage = 1;
        $scope.itemsPerPage = 3;
        $scope.enrollmentsWithProgressesToShowCount = 0;
        $scope.enrollmentsWithProgresses = [];
        $scope.enrollmentsWithProgressesToShow = [];

        $scope.isPassedValueValid = isInteger($routeParams.groupId);

        if ($scope.isPassedValueValid) {
            $scope.userModels = [];
            $scope.userConfig = {
                scrollableHeight: '250px',
                scrollable: true,
                enableSearch: true,
                displayProp: 'Name',
                idProp: 'Id',
                externalIdProp: '',
                smartButtonMaxItems: 3,
                smartButtonTextConverter: function (itemText, originalItem) {
                    return itemText;
                }
            };
            GroupService.getGroupById($routeParams.groupId)
            .then(function (response) {
                $scope.group = response;
                if ($scope.group == null) {
                    $scope.isPassedValueValid = false;
                }
                else {
                    EnrollmentService.getEnrollmentsByGroupId($routeParams.groupId)
                    .then(function (response) {
                        $scope.enrollmentsWithProgresses = response;
                        $scope.showEnrollments();
                    });
                    EnrollmentService.getUsersNotBeongingToGroup($routeParams.groupId)
                    .then(function (response) {
                        $scope.users = response;
                    });
                }
            });
        }
    };

    $scope.showEnrollments = function () {
        var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
            end = begin + $scope.itemsPerPage;

        if ($scope.searchName) {
            var enrollmentsContainingName = [];
            for (var i = 0; i < $scope.enrollmentsWithProgresses.length; i++) {
                if ($scope.enrollmentsWithProgresses[i].Enrollment.User.Name.toLowerCase().indexOf($scope.searchName.toLowerCase()) != -1) {
                    enrollmentsContainingName.push($scope.enrollmentsWithProgresses[i]);
                }
            }
            $scope.enrollmentsWithProgressesToShow = enrollmentsContainingName.slice(begin, end);
            $scope.enrollmentsWithProgressesToShowCount = enrollmentsContainingName.length;
        }
        else {
            $scope.enrollmentsWithProgressesToShow = $scope.enrollmentsWithProgresses.slice(begin, end);
            $scope.enrollmentsWithProgressesToShowCount = $scope.enrollmentsWithProgresses.length;
        }
    };

    $scope.submitButtonClick = function () {
        EnrollmentService.enrollUsersToGroup($scope.userModels, $routeParams.groupId)
        .then(function (ok) {
            $scope.init();
        }, function (badrequest) {
            window.alert((badrequest.ExceptionMessage || badrequest.Message || "Unknown error"));
        });
    };

    $scope.deleteEnrollmentById = function (enrollmentId) {
        var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Are you sure?';
                },
                bodyText: function () {
                    return 'This person may be a great student...';
                }
            }
        });

        modalInstance.result.then(function (answer) {
            if (answer) {
        EnrollmentService.deleteEnrollmentById(enrollmentId)
                    .then(function (success) {
            $scope.init();
                    }, function (badrequest) {
                        var modalInstance = $modal.open({
                            templateUrl: 'messageModal',
                            controller: 'MessageModalController',
                            size: 'sm',
                            resolve: {
                                titleText: function () {
                                    return 'Error!';
                                },
                                bodyText: function () {
                                    return (badrequest.ExceptionMessage || badrequest.Message || "Unknown error");
                                },
                                success: function () {
                                    return false;
                                }
                            }
                        });
                    });
            }
        });
    };

    var isInteger = function(nVal) {
        var reg = /^\d+$/;
        return reg.test(nVal);
    };

    $scope.init();
}]);
