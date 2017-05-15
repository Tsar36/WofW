app.controller('RequestsToSubscribe', function ($scope, $q, $modal, TicketService, GroupService, EnrollmentService) {
    var TicketStatus = {
        Active: 0,
        InProgres: 1,
        Rejected: 2,
        Done: 3
    };

    function initialization() {
        $scope.currentPage = 0;
        $scope.itemsPerPage = 5;

        TicketService.getGroupSubscriptionRequests()
        .then(function(response) {
            $scope.requests = response;

            // To wait on loading data for all array
            var promises = [];

            angular.forEach($scope.requests, function (request) {
                promises.push(
                    GroupService.getGroupById(request.GroupId)
                        .then(function (response) {
                            request.GroupName = response.Name;
                            request.CourseName = response.CourseName;
                        })
                );
            });

            $q.all(promises).then(function () {
                $scope.showRequests();
            });
        });
    }

    function removeRequestFromArray(request) {
        for (var i in $scope.requests)
            if ($scope.requests[i].TicketId == request.TicketId) {
                $scope.requests.splice(i, 1);
                break;
            }
    }

    function openModal(titleText, bodyText) {
        return $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return titleText;
                },
                bodyText: function () {
                    return bodyText;
                }
            }
        });
    }

    $scope.showRequests = function () {
        var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
            end = begin + $scope.itemsPerPage;

        if ($scope.searchName || $scope.searchCourse || $scope.searchGroup) {
            var searchResult = [];
            for (var i in $scope.requests) {
                var match = true;
                if (!(($scope.searchName && $scope.requests[i].UserName.toLowerCase().indexOf($scope.searchName.toLowerCase()) == -1)
                    || ($scope.searchGroup && $scope.requests[i].GroupName.toLowerCase().indexOf($scope.searchGroup.toLowerCase()) == -1)
                    || ($scope.searchCourse && $scope.requests[i].CourseName.toLowerCase().indexOf($scope.searchCourse.toLowerCase()) == -1)))
                        searchResult.push($scope.requests[i]);
            }

            $scope.requestsToShow = searchResult.slice(begin, end);
            $scope.requestsToShowCount = searchResult.length;
        } else {
            $scope.requestsToShow = $scope.requests.slice(begin, end);
            $scope.requestsToShowCount = $scope.requests.length;
        }
    }

    $scope.acceptRequest = function (request) {
        openModal("Are you sure?", "Are you sure that you want to add the student into the group?")
        .result.then(function (answer) {
            if (answer) {
                EnrollmentService.enrollUsersToGroup([{ Id: request.OwnerId, Name:"unused" }], request.GroupId)
                .then(function (response) {
                    request.ReviewStatus = TicketStatus.Done;
                    TicketService.updateTicket(request);

                    removeRequestFromArray(request);
                    $scope.showRequests();
                });
            }
        });

    }

    $scope.rejectRequest = function (request) {
        openModal("Are you sure?", "Are you sure that you want to reject the request?")
        .result.then(function (answer) {
            if (answer) {
                request.ReviewStatus = TicketStatus.Rejected;
                TicketService.updateTicket(request);

                removeRequestFromArray(request);
                $scope.showRequests();
            }
        });
    }

    initialization();
});