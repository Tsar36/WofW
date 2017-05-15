app.controller('ChangePasswordController',["$routeParams","$scope","$window","ChangePasswordService","ConstService",
    function ($routeParams,
        $scope,
        $window,
        ChangePasswordService,
        ConstService) {
        var id = $routeParams.id;
        $scope.changePasswordClick = function () {
            $scope.errorMessage = '';
            var value = {
                login: $scope.nameValue,
                password: $scope.passwordValue,
                email: $scope.emailValue,
                id: id
            };
            if (!$scope.passwordValue || !$scope.repeatPasswordValue) {
                $scope.errorMessage = ConstService.invalidPassword;
            } else if ($scope.passwordValue !== $scope.repeatPasswordValue) {
                $scope.errorMessage = ConstService.invalidConfirmPassword;
            } else {
                ChangePasswordService
                .changePassword(value)
                    .then(function (userData) {
                        if (userData) {
                            window.location.replace('/Index#/PasswordChanged');
                        } else {
                            $scope.errorMessage = ConstService.invalidInput;
                        };
                    },
                    function () {
                        $scope.errorMessage = ConstService.cantHandleRequest;
                    });
            }
        };
}]);