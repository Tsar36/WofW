app.controller('AddWordTranslationsModalController', AddWordTranslationsCtrl);

function AddWordTranslationsCtrl ($scope, $modalInstance, id, languageId, existingWordTranslations, ModalService, WordTranslationService, WordSuiteService,
    LanguageService) {
    $scope.languageId = languageId;
    $scope.wordTranslationsToAdd = [];

    LanguageService.getLanguage(languageId)
       .then(function (language) {
           $scope.language = language;
       });

    $scope.searchWordTranslations = function (searchWord) {
        var searchByTag = false;
        var tags = [];

        if (searchWord.indexOf('#') != -1) {
            searchByTag = true;

            var newTag;
            var tagExp = /#[^\[\]\{\}#]*/g
            while ((newTag = tagExp.exec(searchWord)) !== null) {
                tags.push(newTag[0].trim().replace('#', ''));
            }
            searchWord = "";
        }
        return WordTranslationService
            .searchForWordTranslations(searchWord, tags, $scope.languageId, searchByTag)
                .then(
                function (wordTranslations) {
                    return wordTranslations;
                },
                function (error) {
                    ModalService.showResultModal('Search Word Translations', 'Error while searching Word Translations', false);
                });
    }

    $scope.addWordTranslation = function () {
        if ($scope.selectedWordTranslation && $scope.selectedWordTranslation.Id) {
            for (var i = 0; i < existingWordTranslations.length; i++) {
                if ($scope.selectedWordTranslation.Id === existingWordTranslations[i].Id) {
                    ModalService.showResultModal('Add Word Translation', 'This Translation already exists in Word Suite', false);
                    return;
                }
            }

            for (var i = 0; i < $scope.wordTranslationsToAdd.length; i++) {
                if ($scope.selectedWordTranslation.Id === $scope.wordTranslationsToAdd[i].Id) {
                    ModalService.showResultModal('Add Word Translation', 'You have already added this Translation', false);
                    return;
                }
            }

            $scope.wordTranslationsToAdd.push($scope.selectedWordTranslation);
            $scope.selectedWordTranslation = null;
        }
    }

    $scope.removeWordTranslation = function (index) {
        $scope.wordTranslationsToAdd.splice(index, 1);
    }

    $scope.save = function () {
        var wordTranslationsToAddRange = [];

        for (var i = 0; i < $scope.wordTranslationsToAdd.length; i++) {
            wordTranslationsToAddRange.push({
                WordSuiteId: id,
                WordTranslationId: $scope.wordTranslationsToAdd[i].Id,
                IsStudentWord: true
            });
        }

        WordSuiteService
            .addWordProgresses(wordTranslationsToAddRange)
            .then(
            function (ok) {
                $modalInstance.close(true);
            },
            function (badRequest) {
                ModalService.showResultModal('Add Word Translations', 'Failed to add Word Translations to the Word Suite', false);
            });
    }

    $scope.close = function () {
        $modalInstance.dismiss();
    }
}
AddWordTranslationsCtrl.$inject = ["$scope", "$modalInstance", "id", "languageId", "existingWordTranslations", "ModalService", "WordTranslationService", "WordSuiteService", "LanguageService"];