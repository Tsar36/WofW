app.controller('EmailConfirmedController',["$scope","$routeParams","$http","ConstService",
    function ($scope,
        $routeParams,
        $http,
        ConstService) {
    var code = $routeParams.code;
    var id = $routeParams.id;
    $http.get('/api/register/ConfirmEmail', {
        params: {
            code: code,
            userId: id
        }
    })
        .success(function () {
            $scope.stateMessage = ConstService.successMessage;
            $scope.actionMessage = ConstService.successMessage;
        })
        .error(function () {
            $scope.stateMessage = ConstService.successMessage;
            $scope.actionMessage = ConstService.successMessage;
        });
}]);