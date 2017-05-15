app.factory('HubService', ['$rootScope', '$q', 'UserService', 'toastr',
function ($rootScope, $q, UserService, toastr) {
    var proxy = null;
    var connection = $.hubConnection();
    var isHubConnected = false;

    var service = {
        initialize: initialize,
        stop: stopHub,
        notifyAboutChangeTicketState: notifyAboutChangeTicketState,
        updateTicketTable: updateTicketTable,
        notifyAdminsAboutNewTicket: notifyAdminsAboutNewTicket,
        updateUnreadTicketCounterForUser: updateUnreadTicketCounterForUser,
        updateUnreadTicketCounterForAdmin: updateUnreadTicketCounterForAdmin,
        notifyAboutSharedWordSuites: notifyAboutSharedWordSuites,
        notifyAboutCourseChange: notifyAboutCourseChange
    }

    return service;

    function initialize() {
        var defer = $q.defer();
        if (!isHubConnected) {
            this.proxy = connection.createHubProxy('ticketNotification');
           
            this.proxy.on('updateTicketTable', function () {                
                $rootScope.$broadcast('updateTicketTable');
            });
            this.proxy.on('notifyAboutNewTicket', function (subject) {
                toastr.info("Subject: " + subject, "You have new Ticket");
                $rootScope.$broadcast('updateTicketCounterForAdmin');
            });
            this.proxy.on('notifyAboutChangeTicketState', function (subject, reviewStatus) {
                toastr.info("Subject: " + subject + " Status: " + reviewStatus, "Your request was revised");
                $rootScope.$broadcast('updateTicketCounterForUser');
            });
            this.proxy.on('notifyAboutSharedWordSuites', function () {
                $rootScope.$broadcast('updateWordSuites');
                toastr.info("You have recieved new word suite");
            });
            this.proxy.on('updateUnreadTicketCounterForAdmin', function () {
                $rootScope.$broadcast('updateTicketCounterForAdmin');
            });
            this.proxy.on('updateUnreadTicketCounterForUser', function () {
                $rootScope.$broadcast('updateTicketCounterForUser');
            });

            this.proxy.on('showMessageCourseChanged', function (courseName) {
                toastr.info("Subject: " + courseName, "Your " + courseName + " course has been changed");
            })
            
            connectHub();
        }
        defer.resolve();
        return defer.promise;
    }

    function stopHub() {
        if (isHubConnected) {
            connection.stop();
            isHubConnected = false;
        }
    }

    function connectHub() {
        connection.logging = true;
        var user = UserService.getUserData();
        if (!isHubConnected && user !== null) {
            connection.qs = {
                "id": user.id,
                "role": user.roles.join(' '),
                "hashToken": user.hashToken
            }
            connection.start();
            isHubConnected = true;
        }
        connection.error(function (err) {
            console.log('An error occurred: ' + err);
        });
    }

    function notifyAboutChangeTicketState(ownerId, subject, reviewStatus) {
        this.proxy.invoke('NotifyAboutChangeTicketState', ownerId.toString(), subject, reviewStatus);
    }

    function updateTicketTable(ownerId) {
        this.proxy.invoke('UpdateTicketTable', ownerId);
    }

    function notifyAdminsAboutNewTicket(subject, ownerId) {
        this.proxy.invoke('NotifyAdminsAboutNewTicket', subject, ownerId);
    }

    function updateUnreadTicketCounterForAdmin() {
        this.proxy.invoke('UpdateUnreadTicketCounterForAdmin');
    }

    function updateUnreadTicketCounterForUser(ownerId) {
        this.proxy.invoke('UpdateUnreadTicketCounterForUser', ownerId);
    }

    function notifyAboutSharedWordSuites(teacherToShareId) {
        this.proxy.invoke('NotifyAboutSharedWordSuites', teacherToShareId);
    }

    function notifyAboutCourseChange(courseId, courseName) {
        this.proxy.invoke('NotifyAboutCourseChange',courseId, courseName);
    }
}]);