app.controller('PublicGroupsController', ["$modal", "$scope", "$modal", "GroupService", "UserService", "ConstService", "ModalService", "TicketService", function ($modal, $scope, $modal, GroupService, UserService, ConstService, ModalService, TicketService) {
    $scope.init = function () {
        $scope.currentPage = 1;
        $scope.itemsPerPage = 5;
        $scope.groupsToShowCount = 0;
        $scope.groups = [];
        $scope.groupsToShow = [];
        $scope.orderBy = 'Date';
        GroupService.getPublicGroupsByUserId(UserService.getUserData().id)
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


    $scope.sendRequest = function (groupId) {
        var requestForSend = {
            Subject: "SubscriptionRequest",
            Description: "SubscriptionRequest",
            OwnerId: UserService.getUserData().id,
            GroupId: groupId
        };
        TicketService.createRequest(requestForSend)
                .then(function (ok) {
                    ModalService.showResultModal('Request', 'request was successfuly sent', true);
                }, function (badRequest) {
                    ModalService.showResultModal('Request', ConstService.messageErrorOnServerSide, false);
                });

    };

    $scope.init();
}]);