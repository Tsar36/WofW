app.controller('TinyPlayerController', ['$scope', 'TinyPlayerService','RecordsService', function ($scope, TinyPlayerService, RecordsService) {
    $scope.buttonClass = "btn btn-default";
    if ($scope.use === 'wordEdit')
        $scope.buttonStyle = "padding: 2px 10px";

    var once = function (id) {
        if ($scope.data === null) {
            RecordsService.getRecordByWordId(id)
            .then(function (record) {
                $scope.data = record.Content;
                $scope.comment = record.Description;
                initialize();
                TinyPlayerService.play();
            });
        } else {
            initialize();
        }
    };
    var initialize = function () {
        if ($scope.data !== null) {
            var toGo = [];
            toGo.push(new Float32Array($scope.data));
            toGo.push(new Float32Array($scope.data));
            TinyPlayerService.initializeRecord(toGo);
        } else {
            RecordsService.isThereRecord($scope.id)
           .then(function (ok) {
               $scope.isDisabled = !ok;
               $scope.comment = "Press to listen pronunciation and to see the comment";
           })
        };
    };

    $scope.playButtonPressed = function () {
        once($scope.id);
        TinyPlayerService.play();
    };

    initialize();
}]);