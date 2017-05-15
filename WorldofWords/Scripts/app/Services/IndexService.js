app.service('IndexService',["UserService",function (UserService) {
        this.isLoggedInto = function () {
            return Boolean(UserService.getUserData());
        };
    }]);