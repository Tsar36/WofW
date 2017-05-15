app.directive('showwords', [function () {
    return {
        restrict: 'E',
        scope: {
            data: "=data",
            isReadOnly: '=readonly',
            withId: '=withid',
            remove: "&remove",
        },
       templateUrl: '../Views/SynonymDirect.html'
    };
}]);

app.directive('showrecords', [function () {
    return {
        restrict: 'E',
        scope: {
            data: "=data",
            exists: "=exists",
            remove: '&remove'
        },
        templateUrl: '../Views/ShowRecordDirect.html'
    };
}]);