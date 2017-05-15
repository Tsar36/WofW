app.service('TicketService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {
    this.getAllTickets = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/Ticket', deferred);
        return deferred.promise;
    };

    this.getUserTickets = function () {
        var deferred = $q.defer();
        HttpRequest.get("/api/Ticket/UserTickets", deferred);
        return deferred.promise;
    };

    this.removeTicket = function (id, ownerId) {
        var deferred = $q.defer();
        HttpRequest.delete("/api/Ticket?id=" + id + "&ownerId=" + ownerId, deferred);
        return deferred.promise;
    };

    this.updateTicket = function (ticket) {
        var deferred = $q.defer();
        HttpRequest.put("/api/Ticket", ticket, deferred);
        return deferred.promise;
    };

    this.createRequest = function (request) {
        var deferred = $q.defer();
        HttpRequest.post("api/Ticket/createRequest", request, deferred);
        return deferred.promise;
    };

    this.getGroupSubscriptionRequests = function() {
        var deferred = $q.defer();
        HttpRequest.get("/api/Ticket/GetTicketsToTeacher", deferred);
        return deferred.promise;
    }

    this.getAmountOfUnreadTicketForUser = function () {
        var deferred = $q.defer();
        HttpRequest.get("/api/Ticket/UserAmountOfUnread", deferred);
        return deferred.promise;
    }

    this.getAmountOfUnreadTicketForAdmin = function () {
        var deferred = $q.defer();
        HttpRequest.get("/api/Ticket/AdminAmountOfUnread", deferred);
        return deferred.promise;
    }

}]);