app.controller('TicketController', ["$scope", "$modal", "TicketService", "HubService", function ($scope, $modal, TicketService, HubService) {
    $scope.init = function () {
        $scope.currentPage = 1;
        $scope.itemsPerPage = 5;
        $scope.ticketsToShowCount = 0;
        $scope.tickets = [];
        $scope.ticketsToShow = [];
        $scope.selectedStatusIndex = 0;
        $scope.searchName = "";
        $scope.orderBy = 'Date';
        activate();
    };

    $scope.getTickets = function () {
        TicketService.getAllTickets().then(
            function (response) {
                $scope.tickets = response;
                $scope.showTickets();
            },
            function (error) {
                $scope.title = error;
            });
    }

    $scope.showTickets = function () {
        
        var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
            end = begin + $scope.itemsPerPage;

        if ($scope.searchName) {
            var ticketsContainingName = [];
            for (var i = 0; i < $scope.tickets.length; i++) {
                if ($scope.tickets[i].UserEmail.toLowerCase().indexOf($scope.searchName.toLowerCase()) != -1) {
                    ticketsContainingName.push($scope.tickets[i]);
                }
            }
            $scope.ticketsToShow = ticketsContainingName.slice(begin, end);
            $scope.ticketsToShowCount = ticketsContainingName.length;
        }
        else {
            $scope.ticketsToShow = $scope.tickets.slice(begin, end);
            $scope.ticketsToShowCount = $scope.tickets.length;
        }
    };

    $scope.removeTicket = function (id, ownerId) {
        TicketService.removeTicket(id, ownerId).then(
            function (success) {
                $scope.getTickets();
                HubService.updateTicketTable(ownerId);
                HubService.updateUnreadTicketCounterForAdmin();
                HubService.updateUnreadTicketCounterForUser(ownerId);
            },
            function (error) {
                $scope.title = error;
            });
    }

    $scope.updateTicket = function (ticket) {
        ticket.CloseDate = new Date();
        TicketService.updateTicket(ticket).then(
            function (success) {
                if ($scope.isAdmin) {
                    HubService.notifyAboutChangeTicketState(ticket.OwnerId, ticket.Subject, ticket.ReviewStatusString);
                } else if (!$scope.isAdmin) {
                    HubService.updateUnreadTicketCounterForUser(ticket.OwnerId);
                    HubService.updateTicketTable(ticket.OwnerId);
                }
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
        if (!isAdmin && !ticket.IsReadByUser) {
            ticket.IsReadByUser = true;
            $scope.updateTicket(ticket);
        } else if (ticket.ReviewStatus === 0 && isAdmin) {
            ticket.ReviewStatus = 1;
            ticket.IsReadByAdmin = true;
            ticket.IsReadByUser = false;
            ticket.ReviewStatusString = "InProgress";
            $scope.updateTicket(ticket);
        } else if (isAdmin && !ticket.IsReadByAdmin) {
            ticket.IsReadByAdmin = true;
            $scope.updateTicket(ticket);
        }
    };

    $scope.init();
    $scope.getTickets();

    function activate() {
        HubService.initialize();
    }

    $scope.$on('updateTicketTable', function (e) {
        $scope.$apply(function () {
            $scope.getTickets();
        })
    });
}]);
