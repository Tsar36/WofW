app.controller('UnassignedModalController',["$scope", "$modalInstance",function ($scope,$modalInstance) {
        $scope.actionResult = function () {
            $modalInstance.close();
        };
    }]);
