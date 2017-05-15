app.service('EditUserProfileSevice',["$q","$http","HttpRequest",
    function ($q,
        $http,
        HttpRequest) {
        this.getUserName = function () {
            var deferred = $q.defer();
            HttpRequest.get('api/EditUserProfile/GetName', deferred);
            return deferred.promise;
        };
        this.editUserName = function (newName) {
            var deferred = $q.defer();
            HttpRequest.post('api/EditUserProfile/EditName?editName=' + newName, null, deferred);
            return deferred.promise;
        };
        this.editUserPassword = function (newPassword) {
            var deferred = $q.defer();
            HttpRequest.post('api/EditUserProfile/EditPassword?newPassword=' + newPassword, null, deferred);
            return deferred.promise;
        };
        this.passwordIsInBase = function (password) {
            var deferred = $q.defer();
            HttpRequest.post('api/EditUserProfile/CheckPassword?checkPassword=' + password, null, deferred);
            return deferred.promise;
        };
        this.nameIsInBase = function (name) {
            var deferred = $q.defer();
            HttpRequest.post('api/EditUserProfile/CheckName?checkName=' + name, null, deferred);
            return deferred.promise;
        };
        this.getUserNameById = function (userId) {
            var deferred = $q.defer();
            HttpRequest.get('api/EditUserProfile/GetNameById?userId=' + userId, deferred);
            return deferred.promise;
        };
       
    }]);