app.controller('WordSuiteProgressController', ["$scope",function ($scope) {
    $scope.progressBarStyle = {
        width: $scope.ws.progress + '%'
    };
    var progress = $scope.ws.progress;
    $scope.progressBarClass =
          progress < 25 ? 'progress-bar-danger'
        : progress < 50 ? 'progress-bar-warning'
        : progress < 75 ? 'progress-bar-info'
        : 'progress-bar-success';

}]);