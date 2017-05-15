app.controller('MessageModalController', ["$scope", "$modalInstance", "titleText", "bodyText", "success",
    function ($scope, $modalInstance, titleText, bodyText, success) {
    $scope.titleText = titleText;
    $scope.bodyText = bodyText;
    $scope.success = success;

    $scope.ok = function () {
        $modalInstance.close();
    }
}]);