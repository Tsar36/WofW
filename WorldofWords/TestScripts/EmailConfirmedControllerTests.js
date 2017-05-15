describe("EmailConfirmedController", function () {
    beforeEach(module("MyApp"));

    var controller, scope, http;

    beforeEach(function () {
        constFakeService = {
            wrongEmailOrPassword: "wrong data",
            successMessage: 'All fine',
            mayLoginMessage: 'Your login!',
            failureMessage: 'F*ck, fail!!!',
            tryAgainMessage: 'Try again'
        }
    });

    describe('sucsess http get', function () {
        beforeEach(inject(function ($controller, $rootScope, $httpBackend, ConstService) {
            scope = $rootScope.$new();
            controller = $controller('EmailConfirmedController', { $scope: scope, ConstService: constFakeService });
            $httpBackend.when('GET', '/api/register/ConfirmEmail').respond();
            $httpBackend.flush();
        }));

        it('Equal scope message and message', function () {
            expect(scope.actionMessage).toBeDefined();
            expect(scope.stateMessage).toBeDefined();
            expect(scope.actionMessage).toEqual(constFakeService.mayLoginMessage);
            expect(scope.stateMessage).toEqual(constFakeService.successMessage);
        });
    });

    describe('failure http get', function () {
        beforeEach(inject(function ($controller, $rootScope, $httpBackend) {
            scope = $rootScope.$new();
            controller = $controller('EmailConfirmedController', { $scope: scope, ConstService: constFakeService });
            $httpBackend.when('GET', '/api/register/ConfirmEmail').respond(500);
            $httpBackend.flush();
        }));

        it('Equal scope scope.message and const.messages', function () {
            expect(scope.actionMessage).toBeDefined();
            expect(scope.stateMessage).toBeDefined();
            expect(scope.actionMessage).toEqual(constFakeService.tryAgainMessage);
            expect(scope.stateMessage).toEqual(constFakeService.failureMessage);
        });
    });
});