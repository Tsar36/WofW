app.directive('recorder', [
    function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                input: "="
            },
            templateUrl: '../Views/Recorder.html',
            controller: 'RecorderController'
        }
    }
]);