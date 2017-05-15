app.directive('ticketTable', [function () {
    return {
        restrict: 'E',
        scope : {
            tickets: '=data',
            isAdmin: '=isAdmin'
        },
        controller: 'TicketController',
        templateUrl: '../Views/TicketTable.html'
    };
}]);