describe('UnauthorizeModalController', function () {

    // arrange
    beforeEach(module('MyApp'));

    var scope,
        controller,
        modalInstance = {},
        rootScope,
        modal,
        ConstServiceJasmine = {};

    beforeEach(angular.mock.inject(function($controller, $rootScope,$ingector){
        scope = $rootScope.$new();
        rootScope = $rootScope;
        modalInstance = {
            close : jasmine.createSpy('modalInstance.close'),
        };


        ConstServiceJasmine = Object.create($ingector.get("ConstService"));
        modal = {
            open: jasmine.createSpy('modal.open'),
        };

        controller = $controller('UnauthorizeModalController', {
            $scope: scope,
            $modalInstanse: modalInstance,
            $rootScope: rootScope
        });

    }));

    describe('scope.loginButton', function () {
        
        it('login button test', function () {
            // act
            scope.loginButton();
            expect(rootScope.isPrivete).toBe(true);
            expect(modal.open).toHaveBeenCalled();
        });
   });

    describe('scope.registerButton', function () {
        it('register button test', function () {
            // act     
            scope.registerButton();

            //assert
            expect(rootScope.isPrivete).toBe(true);
            expect(modal.open).toHaveBeenCalled();
        });
    });

    describe('scope.closeModal', function () {
        it('modal to be closed', function () {
            // act     
            scope.closeModal();

            //assert
            expect(modalInstance.close).toHaveBeenCalled();
        });
    });
});