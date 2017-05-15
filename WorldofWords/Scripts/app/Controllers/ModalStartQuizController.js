app.controller('ModalStartQuizController', ["$scope", "$modalInstance", "data", function ($scope, $modalInstance, data) {
    $scope.data = data;
    $scope.yes = function () {
        $modalInstance.close();
    }
    $scope.no = function () {
        $modalInstance.dismiss();
    }
}]);