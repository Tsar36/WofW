describe('UnassignedModalController', function () {
    // arrange
    beforeEach(module('MyApp'));
    var scope;
    var controller;
    var modalInstance = {};

    beforeEach(inject(function (_$controller, $rootScope) {
        scope = $rootScope.$new();
        modalInstance = {
            close: jasmine.createSpy('modalInstance.close')
        };

        controller = _$controller('UnassignedModalController', {
            $scope: scope,
            $modalInstance: modalInstance
        });
    }));


    it('modal instance should be closed', function () {
        // act
        scope.actionResult();

        //assert
        expect(modalInstance.close).toHaveBeenCalled();
    });
});