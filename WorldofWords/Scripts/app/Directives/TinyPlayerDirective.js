app.directive('tinyPlayer', [
    function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                data: "=data",
                comment: "=comment",
                id: "@id",
                use: "@use"
            },
            templateUrl: '../Views/TinyPlayerView.html',
            controller: 'TinyPlayerController'
        }
    }
]);