app.controller('LoginController',["$location","$scope","$modal","$modalInstance","$window","LoginService","IndexService","UserService",
    "ConstService", function ($location,$scope,$modal,$modalInstance,$window,LoginService,IndexService,UserService,ConstService) {
        var onceClicked = ConstService.zero;
        var contain = function (role, obj) {
            for (var i = ConstService.zero; i < role.length; i++) {
                if (role[i] === obj) {
                    return true;
                };
            };
            return false;
        };
        $scope.closeModal = function () {
            $modalInstance.close();
        };
        $scope.loginHideAndRedirectToResetPassword = function () {
            $modalInstance.close();
            location.replace('/Index#/ForgotPassword');
        };
        $scope.openForgotPasswordModal = function () {
            $modalInstance.close();
            $modal.open({
                templateUrl: '../Views/ForgotPasswordModal.html',
                controller: 'ResetPasswordController',
                size: ConstService.small
            });
        };
        $scope.loginButtonClick = function () {
            $scope.loading = true;
            if (!onceClicked) {
                onceClicked++;
                $scope.errorMessage = '';
                var onceReloading = $scope.onceReloading;
                var userInfo = {
                    email: $scope.email,
                    password: $scope.password
                };
                var loginHideAndRedirect = function (token) {
                    UserService.setUserData(token);
                    $scope.isLoggedIn = IndexService.isLoggedInto();
                    if (!contain(token.roles, ConstService.AdminRole)) {
                        if (!contain(token.roles, ConstService.TeacherRole)) {
                            location.replace('/Index#/Courses/');
                        } else location.replace('/Index#/TeacherManager/');
                    } else location.replace('/Index#/Users/');
                };
                var isReloadedAndRedirected = function (token) {
                    if (!onceReloading) {
                        onceReloading++;
                        loginHideAndRedirect(token);
                        location.reload(true);
                    } else {
                        onceReloading = ConstService.zero;
                        loginHideAndRedirect(token);
                    };
                };
                if ((userInfo.email) && (userInfo.password)) {
                    LoginService
                        .login(userInfo)
                        .then(function (token) {
                                if (token) {
                                    isReloadedAndRedirected(token);
                                } else {
                                    $scope.loading = false;
                                    onceClicked = ConstService.zero;
                                    $scope.errorMessage = ConstService.wrongEmailOrPassword;
                                };
                            },
                            function () {
                                $scope.loading = false;
                                onceClicked = ConstService.zero;
                                $scope.errorMessage = ConstService.wrongEmailOrPassword;
                            });
                } else {
                    $scope.loading = false;
                    onceClicked = ConstService.zero;
                    $scope.errorMessage = ConstService.wrongEmailOrPassword;
                }
            } else {
                $scope.loading = false;
                //$scope.errorMessage = ConstService.pleaseWaitMessage;
            };
        };

        $scope.loading = false;
    }]);