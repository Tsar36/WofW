app.service('LoginService', ["$http","$q", function ($http, $q) {
    var userTokenEncoded = {
        id: null,
        email: null,
        roles: null,
        encode: function (token) {
            var items = window.atob(token.EmailAndIdToken).split(' ');
            userTokenEncoded.id = items[0];
            userTokenEncoded.email = items[1];
            userTokenEncoded.roles = window.atob(token.RolesToken).split('|');
            userTokenEncoded.hashToken = token.HashToken;
        }
    };

    this.login = function (userInfo) {
        var deferred = $q.defer();
        $http.post('api/login', userInfo)
            .success(function (token) {
                userTokenEncoded.encode(token);
                deferred.resolve(userTokenEncoded);
            })
            .error(function (error) {
                deferred.reject();
            });
        return deferred.promise;
    };
}]);

