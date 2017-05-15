app.controller('UnauthorizeModalController',["$modalInstance","$rootScope","$scope","$modal","ConstService",
    function ($modalInstance,$rootScope,$scope,$modal,ConstService) {
        $scope.loginButton = function () {
            $rootScope.isPrivate = true;
            $modal.open({
                templateUrl: '../Views/LoginModal.html',
                controller: 'LoginController',
                size: ConstService.small
            });
        };
        $scope.closeModal = function () {
            $modalInstance.close();
        };
        $scope.registerButton = function () {
            $rootScope.isPrivate = true;
            $modal.open({
                templateUrl: '../Views/RegisterModal.html',
                controller: 'RegisterController',
                size: ConstService.small
            });
        };
    }]);
