describe('ConfirmModalController', function () {

    beforeEach(module('MyApp'));

    var controller;
    var scope;
    var modalInstance;

    beforeEach(inject(
      function ($controller, $rootScope) {     
          scope = $rootScope.$new();
          modalInstance = {                    
              close: jasmine.createSpy('modalInstance.close')
          };
          controller = $controller('ConfirmModalController', {
              $scope: scope,
              $modalInstance: modalInstance,
              titleText: "Fake titleText",
              bodyText: "Fake bodyText"
          });
      })
    );

    describe('Dissmiss close and title', function () {
        it('should instantiate the controller properly', function () {
            expect(controller).not.toBeUndefined();
        });

        it('should close the modal with result true', function () {
            scope.yes();
            expect(modalInstance.close).toHaveBeenCalledWith(true);
        });

        it('should close the modal with result false', function () {
            scope.no();
            expect(modalInstance.close).toHaveBeenCalledWith(false);
        });

        it('scope.titleText shuold be equal "fake titleText" and scope.bodyText shuold be equal "fake bodyText"', function () {
            expect(scope.titleText).toEqual('Fake titleText');
            expect(scope.bodyText).toEqual("Fake bodyText");
        });
    });
});