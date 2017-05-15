app.controller('LanguageController', LanguageCtrl);

function LanguageCtrl($scope, $modal, ModalService, LanguageService) {
    $scope.addLanguage = function () {
        LanguageService.addLanguage($scope.selectedLanguage)
            .then(
            function (newLanguageId) {
                if (newLanguageId > 0) {
                    ModalService.showResultModal('Add language', 'Language added successfully', true);
                    initializeLanguages();
                } else {
                    ModalService.showResultModal('Add language', $scope.selectedLanguage.Name + " already exists", false);
                }
            },
            function (error) {
                ModalService.showResultModal('Add language', 'Failed to add language', false);
            });
    }

    $scope.canAdd = false;

    $scope.checkIfAlreadyExists = function () {
        for (var i = 0; i < $scope.languages.length; i++) {
            if ($scope.languages[i].Name == $scope.selectedLanguage.Name) {
                $scope.alertMessage = $scope.selectedLanguage.Name + " already exists";
                $scope.canAdd = false;
                return;
            }
        }

        $scope.alertMessage = "";

        $scope.canAdd = true;
    }

    $scope.removeLanguage = function (index) {
        var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Delete Language';
                },
                bodyText: function () {
                    return 'Are you sure you want to delete ' + $scope.languages[index].Name +
                           '? This action will delete all ' + $scope.languages[index].Name.toLowerCase() + ' words and Word Suites.';
                }
            }
        });

        modalInstance.result.then(function (answer) {
            if (answer) {
                LanguageService
                    .removeLanguage($scope.languages[index].Id)
                        .then(
                            function (success) {
                                ModalService.showResultModal('Delete language', 'Language deleted successfully', true);
                                initializeLanguages();
                            },
                            function (error) {
                                ModalService.showResultModal('Delete language', 'Failed to delete language. It may be assigned to some courses.', false);
                            }
                        );
            }
        });
    }

    var initializeLanguageDropdownList = function () {
        LanguageService.getLanguagesList().then(
            function (response) {
                $scope.languageList = response;
            },
            function (error) {
                ModalService.showResultModal('Load languages', 'Failed to load languages list', false);
            });
    };

    var initializeLanguages = function () {
        LanguageService.getAllLanguages()
            .then(
            function (languages) {
                $scope.languages = languages;
            },
            function (error) {
                ModalService.showResultModal('Load languages', "Error while loading languages", false);
            });
    };

    initializeLanguageDropdownList();
    initializeLanguages();
}

LanguageCtrl.$inject = ["$scope", "$modal", "ModalService", "LanguageService"];