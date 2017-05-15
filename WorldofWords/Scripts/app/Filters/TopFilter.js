app.filter('top', [function () {
    return function (arr, div) {
        return arr.filter(function (item, index) {
            return index < div;
        });
    };
}]);