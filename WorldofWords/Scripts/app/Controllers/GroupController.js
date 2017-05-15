app.controller('GroupController', ["$scope", "$modal", "GroupService", 
    function ($scope, $modal, GroupService) {
    $scope.init = function () {
        $scope.currentPage = 1;
        $scope.itemsPerPage = 5;
        $scope.groupsToShowCount = 0;
        $scope.groups = [];
        $scope.groupsToShow = [];
        $scope.orderBy = 'Date';

        GroupService.getAllGroups()
        .then(function (response) {
            $scope.groups = response;
            $scope.showGroups();
        });
    };

    var orderBy = function (property) {
        return function (a, b) {
            return result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
        }
    }

    $scope.showGroups = function () {
        var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
            end = begin + $scope.itemsPerPage;

        if ($scope.orderBy === 'Date') {
            $scope.groups.sort(orderBy('Id'));
        }
        if ($scope.orderBy === 'Name') {
            $scope.groups.sort(orderBy('Name'));
        }

        if ($scope.searchName) {
            var groupsContainingName = [];
            for (var i = 0; i < $scope.groups.length; i++) {
                if ($scope.groups[i].Name.toLowerCase().indexOf($scope.searchName.toLowerCase()) != -1) {
                    groupsContainingName.push($scope.groups[i]);
                }
            }
            $scope.groupsToShow = groupsContainingName.slice(begin, end);
            $scope.groupsToShowCount = groupsContainingName.length;
        }
        else {
            $scope.groupsToShow = $scope.groups.slice(begin, end);
            $scope.groupsToShowCount = $scope.groups.length;
        }
    };

    $scope.deleteGroupById = function (groupId) {
        var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Delete Group';
                },
                bodyText: function () {
                    return 'Are you sure? Some students may be enrolled to this group.';
                }
            }
        });

        modalInstance.result.then(function (answer) {
            if (answer) {
                GroupService.deleteGroupById(groupId)
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

    $scope.init();
}]);
