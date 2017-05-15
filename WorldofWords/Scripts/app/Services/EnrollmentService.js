app.service('EnrollmentService', ["$http","$q","HttpRequest", function ($http, $q, HttpRequest) {
    this.getEnrollmentsByGroupId = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Enrollment/getEnrollmentsByGroupId?groupId=' + id, deferred);
        return deferred.promise;
    };
    this.getUsersNotBeongingToGroup = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Enrollment/getUsersNotBelongingToGroup?groupId=' + id, deferred);
        return deferred.promise;
    };
    this.enrollUsersToGroup = function (users, id) {
        var deferred = $q.defer();
        var data = {
            userModels: users,
            groupId: id
        };
        HttpRequest.post('api/Enrollment/enrollUsersToGroup/', data, deferred);
        return deferred.promise;
    };
    this.deleteEnrollmentById = function (enrollmentId) {
        var deferred = $q.defer();
        HttpRequest.delete('/api/Enrollment?enrollmentId=' + enrollmentId, deferred);
        return deferred.promise;
    };
}]);