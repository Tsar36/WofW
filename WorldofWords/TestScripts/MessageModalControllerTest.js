describe('MessageModalController', function () {

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
              bodyText: "Fake bodyText",
              success: "Fake success"
          });
      })
    );

    describe('Dissmiss close and title', function () {
        it('should instantiate the controller properly', function () {
            expect(controller).not.toBeUndefined();
        });

        it('should close the modal', function () {
            scope.ok();
            expect(modalInstance.close);
        });

        it('scope.titleText shuold be equal "fake titleText" and scope.bodyText shuold be equal "fake bodyText"', function () {
            expect(scope.titleText).toEqual('Fake titleText');
            expect(scope.bodyText).toEqual('Fake bodyText');
            expect(scope.success).toEqual('Fake success');
        });
    });
})