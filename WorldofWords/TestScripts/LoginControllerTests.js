/// <reference path="../node_modules/karma-jasmine/lib/jasmine.js" />
describe('LoginController', function () {
    beforeEach(module('MyApp'));

    var $controller;
    var $scope, $q;
    var modalInstance;
    var loginFakeService = {};

    beforeEach(function () {
        loginFakeService = {
            login: function (userInfo) {
                return {
                    then: function (callback) { return callback([{ some: "thing", hoursInfo: { isOpen: true } }]); }
                }
            },
            loginHideAndRedirect : function(){}
        };
    });

    beforeEach(function () {
        constFakeService = {
            wrongEmailOrPassword: "wrong data",
            zero : 0
        }
    });

    beforeEach(inject(function (_$controller_, _$q_, $rootScope) {
        $controller = _$controller_;
        $scope = $rootScope.$new();
        $q = _$q_;
    }));

    describe('$scope.loginButtonClick', function () {
        it('sets errorMessage to "Wrong user e-mail or password" if passwords mismatch', function () {
            $scope.email = '';
            $scope.password = '';
            var controller = $controller('LoginController', {
                $scope: $scope,
                $modalInstance: modalInstance,
                LoginService: loginFakeService,
                ConstService: constFakeService

            });

            $scope.loginButtonClick();
            $scope.$root.$digest();

            expect($scope.errorMessage).toEqual("wrong data");
        });
    });
});