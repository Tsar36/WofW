app.controller('MessageWithActionModalController', ["$scope", "$modalInstance", "titleText", "bodyText", "success", "actionButtonText",
    function ($scope, $modalInstance, titleText, bodyText, success, actionButtonText) {
    $scope.titleText = titleText;
    $scope.bodyText = bodyText;
    $scope.success = success;
    $scope.actionButtonText = actionButtonText;

    $scope.ok = function () {
        $modalInstance.close(false);
    }

    $scope.action = function () {
        $modalInstance.close(true);
    }
}]);