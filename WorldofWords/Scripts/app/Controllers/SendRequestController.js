app.controller('SendRequestController', ["$scope", "$modalInstance", "TicketService", "ConstService", "toastr", "HubService", "UserService",
    function ($scope, $modalInstance, TicketService, ConstService, toastr, HubService, UserService) {
        var initialise = function () {
            $scope.request = {
                subject: "",
                description: ""
            };
            activate();
        };
        $scope.closeModal = function () {
            $modalInstance.close();
        };

        $scope.subjects = ConstService.subjectsForRequest;

        $scope.enableTooltip = function () {
            return !(Boolean($scope.request.description) && Boolean($scope.request.subject));
        };

        $scope.sendRequest = function () {
            if ($scope.sendRequestForm.$valid) {
                var requestForSend = {
                    subject: $scope.request.subject,
                    description: $scope.request.description,
                    isReadByUser: true,
                    ownerId: UserService.getUserData().id
                };
                TicketService.createRequest(requestForSend)
                    .then(function (ok) {
                        toastr.success("Your request was successfuly sent");
                        HubService.notifyAdminsAboutNewTicket(requestForSend.subject, requestForSend.ownerId);
                        $scope.closeModal();
                    }, function (badRequest) {
                        $scope.closeModal();
                        toastr.error(ConstService.messageErrorOnServerSide);
                    });
            }
            else {
                $scope.isEnteredDescription = Boolean($scope.request.description);
                $scope.isEnteredSubject = Boolean($scope.request.subject);
            }
        };

        function activate() {
            HubService.initialize();
        }

        initialise();

    }]);