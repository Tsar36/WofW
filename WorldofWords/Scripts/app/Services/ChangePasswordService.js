app.service('ChangePasswordService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {

    this.changePassword = function (userInfo) {
        var deferred = $q.defer();
        HttpRequest.post('api/register/ChangePassword', userInfo, deferred);
        return deferred.promise;
    };
}]);