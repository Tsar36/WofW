app.controller('CreateWordTranslationController',["$scope", "$window", "$timeout", "$modal", "$modalInstance", "WordsService", "WordTranslationService", 
    "ModalService", "UserService", "ConstService", "languageId",
    function ($scope, $window, $timeout, $modal, $modalInstance, WordsService, WordTranslationService, ModalService,
        UserService, ConstService, languageId) {
    $scope.lang = languageId;
    $scope.wordsToSearch = 10;
    $scope.selectedWord = '';
    $scope.selectedTranslation = '';
    $scope.IdOfAdded = null;

    $scope.searchWords = function (searchWord) {
        return WordsService
            .searchForWords(searchWord, $scope.lang, $scope.wordsToSearch)
                .then(function (words) {
                    return words;
                });
    }

    $scope.searchTranslations = function (searchWord) {
        return WordsService
            .searchForTranslations(searchWord, $scope.wordsToSearch)
                .then(function (Translations) {
                    return Translations;
                });
    }

    var addWordTranslationToGlobalDictionary = function(word, translation){
        var modelToAdd = {
            OriginalWord: word.Value,
            TranslationWord: translation.Value,
            OriginalWordId: word.Id,
            TranslationWordId: translation.Id,
            Transcription: word.Transcription,
            Description: word.Description,
            LanguageId: word.LanguageId,
            OwnerId: UserService.getUserData().id
        }

        WordTranslationService.addWordTranslation(modelToAdd)
        .then(function (response) {

            $scope.IdOfAdded = response;

            ModalService.showResultModal(ConstService.successTitleForModal, ConstService.messageWhenWordTranslationIsAdded, true);
        },
            function (error) {
            ModalService.showResultModal(ConstService.failureTitleForModal, (String(error.Message)), false);
        });
    }

    $scope.ManipulateData = function () {
        if ($scope.selectedTranslation === '' || $scope.selectedWord === '') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
            return;
        }
        if (typeof ($scope.selectedWord) !== 'object') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhichSaysThatYouMustChooseValueFromDropdownList, false);
        }
        if (typeof ($scope.selectedWord) === 'object' && typeof ($scope.selectedTranslation) === 'object') {
            addWordTranslationToGlobalDictionary($scope.selectedWord, $scope.selectedTranslation);
        } else if (typeof ($scope.selectedWord) === 'object' && typeof ($scope.selectedTranslation) !== 'object') {
            WordsService.addWord({ Value: $scope.selectedTranslation, LanguageId: 4 })
            .then(function (response) {
                WordsService.getWordById(response)
                .then(function (response2) {
                    addWordTranslationToGlobalDictionary($scope.selectedWord, response2);
                });
            });
        }
    }

    $scope.close = function () {
        $modalInstance.close($scope.IdOfAdded);
    };

    $scope.openForWord = function () {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'Views/AddWord.html',
            controller: 'WordsController',
            size: 'lg',
            resolve: {
                language: function () {
                    return $scope.lang;
                }
            }
        });

        modalInstance.result.then(function () {
        });
    }

    $scope.openForTranslation = function () {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'Views/AddWord.html',
            controller: 'WordsController',
            size: 'lg',
            resolve: {
                language: function () {
                    return 4;
                }
            }
        });

        modalInstance.result.then(function () {
        });
    }
}]);