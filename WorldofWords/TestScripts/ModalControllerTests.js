describe('ModalController', function () {

    beforeEach(module('MyApp'));

    var controller;
    var scope;
    var modalInstance;

    // Initialize the controller and a mock scope
    beforeEach(inject(
      function ($controller, $rootScope) {     // Don't bother injecting a 'real' modal
          scope = $rootScope.$new();
          modalInstance = {                    // Create a mock object using spies
              close: jasmine.createSpy('modalInstance.close'),
              dismiss: jasmine.createSpy('modalInstance.dismiss'),
              result: {
                  then: jasmine.createSpy('modalInstance.result.then')
              },
          };
          controller = $controller('ModalController', {
              $scope: scope,
              $modalInstance: modalInstance,
              itemArray: function () { return ['a', 'b', 'c']; },
              data:"fake data"
          });
      })
    );

    describe('Initial state', function () {
        it('should instantiate the controller properly', function () {
            expect(controller).not.toBeUndefined();
        });

        it('should close the modal', function () {
            scope.close();
            expect(modalInstance.close).toHaveBeenCalledWith();
        });

        it('should dismiss with result "cancel" when rejected', function () {
            scope.cancel();
            expect(modalInstance.dismiss).toHaveBeenCalledWith('cancel');
        });

        it('scope.data shuold be equal "fake data"', function () {
            expect(scope.data).toEqual('fake data');
        });
    });
});