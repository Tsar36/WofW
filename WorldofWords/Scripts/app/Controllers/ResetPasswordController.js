app.controller('ResetPasswordController',["$scope","$window","$modal","$modalInstance","ResetPasswordService","ConstService",
    function ($scope,$window,$modal,$modalInstance,ResetPasswordService,ConstService) {
        $scope.closeModal = function () {
            $modalInstance.close();
        };
        $scope.passwordResetClick = function () {
            $scope.errorMessage = "";
            var value = {
                login: $scope.nameValue,
                password: $scope.passwordValue,
                email: $scope.emailValue,
            };
            ResetPasswordService
                    .sendPasswordReset(value)
                    .then(function (newUserId) {
                        if (newUserId) {
                            $modalInstance.close();
                            window.location.replace("/Index#/EmailSentPage");
                        } else {
                            $scope.errorMessage = ConstService.somethingWrong;
                        };
                    }, 
                    function () {
                        $scope.errorMessage = ConstService.wrongEmail;
                    });
        };
}]);