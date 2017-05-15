app.directive('wordsuiteProgress', [function () {
    return {
        restrict: 'E',
        scope: {
            ws: '=wordsuite'
        },
        templateUrl: '../../../Views/WordSuiteProgress.html',
        controller: 'WordSuiteProgressController'
    };
}]);