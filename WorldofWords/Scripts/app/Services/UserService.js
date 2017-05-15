/// <reference path="../../libs/sha512.js" />
app.service('UserService',["$http","$q","ConstService", function ($http,$q,ConstService) {
    this.getUserData = function () {
        return JSON.parse(localStorage.getItem(ConstService.userData));
    };
    this.setUserData = function (token) {
        token = JSON.stringify(token);
        localStorage.setItem(ConstService.userData, token);
    };
    this.getUserDataForConfiguringAsAdmin = function () {
        var deferred = $q.defer();
        var user = this.getUserData();
        var idToken = SHA256(user.id.toString());
        var hashedToken = user.hashToken;
        $http.defaults.headers.common.Authorization = 'Basic ' + hashedToken + ' ' + idToken + ' ' + user.roles.join(' ');
        $http.get('/api/UsersList')
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (error) {
                deferred.reject(error);
            });
        return deferred.promise;
    };

    this.getAmountOfUsersByRoleId = function (roleId) {
        var deferred = $q.defer();
        $http.get('/api/UsersList?roleId=' + roleId)
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (error) {
                deferred.reject(error);
            });
        return deferred.promise;
    }

    this.getUsersFromInterval = function (start, end, roleid) {
        var deferred = $q.defer();
        $http.get('/api/UsersList?start=' + start + '&end=' + end + '&roleid=' + roleid)
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (error) {
                deferred.reject(error);
            });
        return deferred.promise;
    }

    this.changeRolesOfUser = function (user) {
        var deferred = $q.defer();
        $http.put('api/UsersList', user)
        .success(function (response) {
            deferred.resolve(response);
        })
        .error(function (error) {
            deferred.reject(error);
        });
        return deferred.promise;
    }

    this.searchUserByName = function (searchvalue, roleid) {
        var deferred = $q.defer();
        $http.get('api/UsersList?namevaluetosearch=' + searchvalue + '&roleid=' + roleid)
        .success(function (response) {
            deferred.resolve(response);
        })
        .error(function (error) {
            deferred.reject(error);
        });
        return deferred.promise;
    };
}]);