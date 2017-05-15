app.directive("passwordStrength", function () {
    return {
        restrict: "A",
        link: function (scope, element, attrs) {
            var strong = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
            var medium = new RegExp("^(((?=.*[a-z])(?=.*[A-Z]))|((?=.*[a-z])(?=.*[0-9]))|((?=.*[A-Z])(?=.*[0-9])))(?=.{6,})");
            scope.strengthStyle = { "color": "red" };
            scope.$watch(attrs.passwordStrength, function (password) {
                if (angular.isDefined(password)) {
                    if (password.length === 0) {
                        scope.strongEnough = false;
                        scope.errorMessage = "";
                        scope.strengthStyle = { "color": "black" };
                    } else if (password.length < 5) {
                        scope.strongEnough = false;
                        scope.errorMessage = "Password length must be at least 6 symbols";
                        scope.strengthStyle = { "color": "red" };
                    } else if (strong.test(password)) {
                        scope.strongEnough = true;
                        scope.errorMessage = "Strong";
                        scope.strengthStyle = { "color": "green" };
                    } else if (medium.test(password)) {
                        scope.strongEnough = true;
                        scope.errorMessage = "Medium";
                        scope.strengthStyle = { "color": "orange" };
                    } else {
                        scope.strongEnough = true;
                        scope.errorMessage = "Weak";
                        scope.strengthStyle = { "color": "red" };
                    }
                }
            });
        }
    };
});