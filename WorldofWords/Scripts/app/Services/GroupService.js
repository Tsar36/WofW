app.service('GroupService', ["$http","$q","HttpRequest", function ($http, $q, HttpRequest) {
    this.getAllGroups = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/Group', deferred);
        return deferred.promise;
    };
    this.getGroupById = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Group?groupId=' + id, deferred);
        return deferred.promise;
    }

    this.getPublicGroupsByUserId = function (userId) {
        var deferred = $q.defer();
        HttpRequest.get('api/Group/AllToSubscribe/' + userId, deferred);
        return deferred.promise;
    }
    this.createGroup = function (groupInfo) {
        var deferred = $q.defer();
        HttpRequest.post('/api/Group', groupInfo, deferred);
        return deferred.promise;
    }
    this.deleteGroupById = function (groupId) {
        var deferred = $q.defer();
        HttpRequest.delete('/api/Group?groupId=' + groupId, deferred);
        return deferred.promise;
    };
    this.getAllCourses = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/Group/getCourses', deferred);
        return deferred.promise;
    }
}]);