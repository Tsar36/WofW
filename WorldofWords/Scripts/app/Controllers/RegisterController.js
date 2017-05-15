app.controller('RegisterController', ["$scope", "$window", "$modal", "$modalInstance", "RegisterService", "LanguageService", "ConstService",
    function ($scope, $window, $modal, $modalInstance, RegisterService, LanguageService, ConstService) {
        var onceClicked = ConstService.zero;
        $scope.languages = [];
        $scope.avaliableLanguagesCount = ConstService.retrievingLanguages;
        $scope.strongEnough = false;
        $scope.customStyle = {};
        $scope.closeModal = function () {
            $modalInstance.close();
        };

        $scope.getAvaliableLanguages = function () {
            LanguageService.getAllLanguages().then(
                function (response) {
                    $scope.languages = response;
                    $scope.avaliableLanguagesCount = response.length;
                });
        }

        $scope.registerButtonClick = function () {
            if (!onceClicked) {
                onceClicked++;
                $scope.errorMessage = "";

                var value = {
                    login: $scope.nameValue + ' ' + $scope.surnameValue,
                    password: $scope.passwordValue,
                    email: $scope.emailValue,
                    languageId: $scope.languageId,
                    pagesUrl: window.location.protocol + '//' + window.location.host
                };
                
                if (!$scope.nameValue) {
                    $scope.errorMessage = ConstService.invalidName;
                    onceClicked = ConstService.zero;
                } else if (!$scope.surnameValue) {
                    $scope.errorMessage = ConstService.invalidName;
                    onceClicked = ConstService.zero;
                } else if (!$scope.emailValue) {
                    $scope.errorMessage = ConstService.invalidEmail;
                    onceClicked = ConstService.zero;
                } else if (!$scope.passwordValue || !$scope.repeatPasswordValue) {
                    $scope.errorMessage = ConstService.invalidPassword;
                    onceClicked = ConstService.zero;
                } else if ($scope.passwordValue !== $scope.repeatPasswordValue) {
                    $scope.errorMessage = ConstService.invalidConfirmPassword;
                    onceClicked = ConstService.zero;
                } else if (!$scope.strongEnough) {
                    $scope.errorMessage = ConstService.invalidStrengthPassword;
                    onceClicked = ConstService.zero;
                } else if (!$scope.languageId) {
                    $scope.errorMessage = ConstService.fillInError;
                    onceClicked = ConstService.zero;
                } else if ((value.email) && (value.password)) {
                    RegisterService
                        .registerUser(value)
                        .then(function (newUserId) {
                            if (newUserId) {
                                $modalInstance.close();
                                window.location.replace('/Index#/EmailSentPage');
                            } else {
                                onceClicked = ConstService.zero;
                                $scope.errorMessage = ConstService.existUser;
                            };
                        },
                        function () {
                            onceClicked = ConstService.zero;
                            $scope.errorMessage = ConstService.existUser;
                        });
                } else {
                    onceClicked = ConstService.zero;
                    $scope.errorMessage = ConstService.inputData;
                }
            } else {
                $scope.errorMessage = ConstService.pleaseWaitMessage;
            };
        };

        $scope.getAvaliableLanguages();
    }]);