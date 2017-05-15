app.directive("changeTheme", function () {
    return {
        restrict: "E",
        controller: "IndexController",
        replace: true,
        template: '<link rel="stylesheet" href="dist/{{ defaultTheme }}"/>',
    }

});