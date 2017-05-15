/// <reference path="../../libs/sha512.js" />
app.service('HttpRequest', ["$http", "$q", "UserService", "$location",
    function ($http,$q,UserService, $location) {

        var createToken = function () {
            var user = UserService.getUserData();
            if (user) {
                var idToken = SHA256(user.id.toString());
                var hashedToken = user.hashToken;
                $http.defaults.headers.common.Authorization = 'Basic ' + hashedToken + ' ' + idToken + ' ' + user.roles.join(' ');
            } else {
                $http.defaults.headers.common.Authorization = null;
            };
        };

        this.get = function (url, deferred) {
            createToken();
            $http.get(url)
                .success(function (response) {
                    deferred.resolve(response);
                })
                .error(function (error, status) {
                    if (status === 401) {
                        UserService.setUserData(null);
                        $location.replace('Index#/Home');
                    };
                    deferred.reject(error);
                });
        };

        this.post = function (url, data, deferred) {
            createToken();
            $http.post(url, data)
                .success(function (response) {
                    deferred.resolve(response);
                })
                .error(function (error) {
                    if (status === 401) {
                        UserService.setUserData(null);
                        $location.replace('Index#/Home');
                    };
                    deferred.reject(error);
                });
        };

        this.delete = function (url, deferred) {
            createToken();
            $http.delete(url)
                .success(function (response) {
                    deferred.resolve(response);
                })
                .error(function (error) {
                    if (status === 401) {
                        UserService.setUserData(null);
                        $location.replace('Index#/Home');
                    };
                    deferred.reject(error);
                });
        };

        this.put = function (url, data, deferred) {
            createToken();
            $http.put(url, data)
                .success(function (response) {
                    deferred.resolve(response);
                })
                .error(function (error) {
                    if (status === 401) {
                        UserService.setUserData(null);
                        $location.replace('Index#/Home');
                    };
                    deferred.reject(error);
                });
        };
    }]);