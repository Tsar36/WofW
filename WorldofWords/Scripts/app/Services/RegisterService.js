app.service('RegisterService', ["$http","$q", function ($http, $q) {
    var userFromPost = {
        id: null,
        email: null,
        roles: null
    };
    this.registerUser = function (userInfo) {
        var deferred = $q.defer();
        $http.post('api/register', userInfo)
            .success(function (token) {
                decode(token);
                deferred.resolve(userFromPost);
            })
            .error(function (error) {
                deferred.reject(error);
            });
        return deferred.promise;
    };

    var decode = function (token) {
        userFromPost.id = window.atob(token.EmailAndIdToken).replace(/[a-zA-Z@.]/g, '');
        userFromPost.email = window.atob(token.EmailAndIdToken).replace(/[0-9]/g, '');
        userFromPost.claims = window.atob(token.RolesToken).split('|');
    };
}]);