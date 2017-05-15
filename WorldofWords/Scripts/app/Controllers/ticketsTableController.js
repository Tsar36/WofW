app.controller('ticketsTableController', ["$scope", "$modal", "TicketService", "HubService", function ($scope, $modal, TicketService, HubService) {
    $scope.init = function () {
        activate();
    };

    $scope.removeTicket = function (id) {
        TicketService.removeTicket(id).then(
            function (success) {
                $scope.getTickets();
            },
            function (error) {
                $scope.title = error;
            });
    }

    $scope.updateTicket = function (ticket) {
        ticket.CloseDate = new Date();
        TicketService.updateTicket(ticket).then(
            function (success) {
                //$scope.toggleDetail(lastIndex, ticket, false);
                $scope.getTickets();
            },
            function (error) {
                $scope.title = error;
            });
    }

    var lastIndex = 0;
    $scope.toggleDetail = function (index, ticket, isAdmin) {
        lastIndex = index;
        $scope.activePosition = $scope.activePosition == index ? -1 : index;
        if (!isAdmin) {
            ticket.IsReadByUser = true;
            $scope.updateTicket(ticket);
        }
        else {
            ticket.IsReadByAdmin = true;
            ticket.IsReadByUser = false;
        }
        if (ticket.ReviewStatus === 0 && isAdmin === true) {
            ticket.ReviewStatus = 1;
            ticket.ReviewStatusString = "InProgress";
            $scope.updateTicket(ticket);
        }
    };

    $scope.init();

    function activate() {
        HubService.initialize();
    }
    $scope.$on('updateTicketTable', function (e) {
        $scope.$apply(function () {
            $scope.getTickets();
        })
    });
}]);