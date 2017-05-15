describe('SendRequestController', function () {
    beforeEach(angular.mock.module('MyApp'));

    var controller;
    var $scope;
    var $modalInstance = {};
    var $q = {};
    var TicketServiceJasmine = {};
    var UserServiceJasmine = {};
    var HubServiceJasmine = {};
    var toastr = {};
    var passPromise = null;

    beforeEach(angular.mock.inject(function (_$controller_, _$rootScope_, _$q_, $injector) {
        $scope = _$rootScope_.$new();
        controller = _$controller_;
        TicketServiceJasmine = Object.create($injector.get("TicketService"));
        UserServiceJasmine = Object.create($injector.get("UserService"));
        $modalInstance = {
            close: jasmine.createSpy('modalInstance.close'),
        };
        HubServiceJasmine = Object.create($injector.get("HubService"));
        $q = _$q_;
        
    }));

    describe('$scope.closeModal', function () {
        it('close modal window when close button is pressed', function () {
            var cntrl = controller('SendRequestController', {
                $scope: $scope, $modalInstance: $modalInstance
            });
            $scope.closeModal();
            expect($modalInstance.close).toHaveBeenCalled();
        });
    });
    

    describe('$scope.enableTooltip', function () {
        it('tooltip is enabled', function () {
            var cntrl = controller('SendRequestController', {
                $scope: $scope, $modalInstance: $modalInstance
            });
            $scope.request.description = "";
            $scope.request.subject = "NEw role";
            expect($scope.enableTooltip()).toBe(true);
        });

        it('tooltip is disabled', function () {
            var cntrl = controller('SendRequestController', {
                $scope: $scope, $modalInstance: $modalInstance
            });
            $scope.request.description = "pes";
            $scope.request.subject = "NEw role";
            expect($scope.enableTooltip()).toBe(false);
        });
    });
    
    describe('$scope.sendRequest', function () {
        beforeEach(function () {
            toastr = {
                success: jasmine.createSpy('toastr.success'),
                error: jasmine.createSpy('toastr.error')
            }
            spyOn(TicketServiceJasmine, 'createRequest').and.callFake(function () {
                var deferred = $q.defer();
                if (passPromise) {
                    deferred.resolve(true);
                }
                else {
                    deferred.reject("something went wrong");
                }
                return deferred.promise;
            });
            spyOn(HubServiceJasmine, 'notifyAdminsAboutNewTicket');
            spyOn(UserServiceJasmine, 'getUserData').and.returnValue({ id: 8 });
        });

        it('should send request when input is correct', function () {
            var cntrl = controller('SendRequestController', {
                $scope: $scope, $modalInstance: $modalInstance, TicketService: TicketServiceJasmine,
                toastr: toastr, HubService: HubServiceJasmine, UserService: UserServiceJasmine
            });
            passPromise = true;
            $scope.request.subject = "Bug";
            $scope.request.description = "betrayal";
            $scope.sendRequestForm = {
                $valid: true
            }
            $scope.sendRequest();
            $scope.$root.$digest();
            expect(TicketServiceJasmine.createRequest).toHaveBeenCalledWith({
                subject: $scope.request.subject,
                description: $scope.request.description,
                isReadByUser: true,
                ownerId: 8
            });
            expect(toastr.success).toHaveBeenCalledWith("Your request was successfuly sent");
            expect(HubServiceJasmine.notifyAdminsAboutNewTicket).toHaveBeenCalledWith($scope.request.subject, 8);
            expect($modalInstance.close).toHaveBeenCalled();
        });

        it('should highlight required fields when input is incorrect', function () {
            var cntrl = controller('SendRequestController', {
                $scope: $scope, $modalInstance: $modalInstance, TicketService: TicketServiceJasmine, toastr: toastr
            });
            $scope.sendRequestForm = {
                $valid: false
            }
            $scope.request.subject = "Other";
            $scope.sendRequest();
            $scope.$root.$digest();
            expect($scope.isEnteredDescription).toBe(false);
        });
    });
   



});