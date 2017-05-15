app.controller('EditWordSuiteController', EditWordSuiteCtrl);

function EditWordSuiteCtrl($scope, $window, $modal, $routeParams, ModalService, UserService, WordSuiteService, WordTranslationService,
    LanguageService) {
    $scope.newWordTranslations = [];
    $scope.prohibitedQuizzesToEdit = [];

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
            .searchForWordTranslations(searchWord, tags, $scope.wordSuite.LanguageId, searchByTag)
                .then(
                function (wordTranslations) {
                    return wordTranslations;
                },
                function (error) {
                    ModalService.showResultModal('Search Word Translations', 'Error while searching Word Translations', false);
                });
    }

    $scope.removeExistingWordTranslation = function (index) {
        wordTranslationsToDeleteIdRange.push($scope.existingWordTranslations[index].Id);
        translationWasRemoved($scope.existingWordTranslations[index]);
        $scope.existingWordTranslations.splice(index, 1);
    }

    $scope.removeNewWordTranslation = function (index) {
        translationWasRemoved($scope.newWordTranslations[index]);
        $scope.newWordTranslations.splice(index, 1);
    }

    $scope.addWordTranslationToWordSuite = function () {
        if ($scope.selectedWordTranslation && $scope.selectedWordTranslation.Id) {
            for (var i = 0; i < $scope.existingWordTranslations.length; i++) {
                if ($scope.selectedWordTranslation.Id === $scope.existingWordTranslations[i].Id) {
                    ModalService.showResultModal('Add Word Translation', 'This Translation already exists in Word Suite', false);
                    return;
                }
            }

            for (var i = 0; i < $scope.newWordTranslations.length; i++) {
                if ($scope.selectedWordTranslation.Id === $scope.newWordTranslations[i].Id) {
                    ModalService.showResultModal('Add Word Translation', 'You have already added this Translation', false);
                    return;
                }
            }

            $scope.newWordTranslations.push($scope.selectedWordTranslation);
            translationWasAdded($scope.selectedWordTranslation);
            $scope.selectedWordTranslation = null;
        }
    }

    $scope.openImportModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'modalImport',
            controller: 'ImportWordTranslationsController',
            size: 'lg',
            resolve: {
                languageId: function () {
                    return $scope.wordSuite.LanguageId;
                }
            }
        });

        modalInstance.result.then(function (importedWordTranslations) {
            for (var i = 0; i < importedWordTranslations.length; i++) {
                var wordFound = false;

                for (var j = 0; j < $scope.newWordTranslations.length; j++) {
                    if (importedWordTranslations[i].Id === $scope.newWordTranslations[j].Id) {
                        wordFound = true;
                        break;
                    }
                }

                if (!wordFound) {
                    for (var j = 0; j < $scope.existingWordTranslations.length; j++) {
                        if (importedWordTranslations[i].Id === $scope.existingWordTranslations[j].Id) {
                            wordFound = true;
                            break;
                        }
                    }
                }

                if (!wordFound) {
                    $scope.newWordTranslations.push(importedWordTranslations[i]);
                    translationWasAdded(importedWordTranslations[i]);
                }
            }
        });
    }

    $scope.saveWordSuite = function () {
        if (checkInput() && ($scope.newWordTranslations.length || $scope.existingWordTranslations.length)) {
            for (var i = 0; i < $scope.newWordTranslations.length; i++) {
                wordTranslationsToAddIdRange.push($scope.newWordTranslations[i].Id);
            }

            if (checkIfAreAnyChanges()) {
                WordSuiteService
                    .editWordSuite({
                        Id: $scope.wordSuite.Id,
                        Name: $scope.wordSuite.Name,
                        LanguageId: $scope.wordSuite.LanguageId,
                        Threshold: $scope.wordSuite.Threshold,
                        QuizResponseTime: $scope.wordSuite.QuizResponseTime,
                        WordTranslationsToAddIdRange: wordTranslationsToAddIdRange,
                        WordTranslationsToDeleteIdRange: wordTranslationsToDeleteIdRange,
                        IsBasicInfoChanged: isBasicInfoChanged,
                        AreProhibitedQuizzesChanged: $scope.AreProhibitedQuizzesChanged,
                        ProhibitedQuizzesId: WordSuiteService.getIdFromQuizzes($scope.prohibitedQuizzesToEdit)
                    })
                    .then(
                    function (ok) {
                        showResultModal('Edit Word Suite', 'Word Suite successfully edited', true);
                    },
                    function (badRequest) {
                        showResultModal('Edit Word Suite', badRequest.Message, false);
                    });
            }
        }
    }

    $scope.createWordTranslation = function () {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'Views/CreateWordTranslation.html',
            controller: 'CreateWordTranslationController',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                languageId: function () {
                    return $scope.wordSuite.LanguageId;
                }
            }
        });
    }

    var oldName = null,
        oldThreshold = null,
        oldQuizResponseTime = null,
        isBasicInfoChanged = false,
        wordTranslationsToAddIdRange = [],
        wordTranslationsToDeleteIdRange = [],
        oldProhibitedQuizzes;


    var checkInput = function () {
        $scope.isEnteredName = Boolean($scope.wordSuite.Name);
        $scope.isEnteredThreshold = Boolean($scope.wordSuite.Threshold);
        $scope.isEnteredQuizResponseTime = Boolean($scope.wordSuite.QuizResponseTime);

        return $scope.isEnteredName && $scope.isEnteredThreshold && $scope.isEnteredQuizResponseTime;
    }

    var checkIfAreAnyChanges = function () {
        isBasicInfoChanged = oldName !== $scope.wordSuite.Name ||
                             oldThreshold !== $scope.wordSuite.Threshold ||
                             oldQuizResponseTime != $scope.wordSuite.QuizResponseTime;
        $scope.AreProhibitedQuizzesChanged = !WordSuiteService.areArraysEqual(oldProhibitedQuizzes, WordSuiteService.getIdFromQuizzes($scope.prohibitedQuizzesToEdit));
        return isBasicInfoChanged || wordTranslationsToAddIdRange.length || wordTranslationsToDeleteIdRange.length || $scope.AreProhibitedQuizzesChanged;
    }

    var showResultModal = function (title, body, success) {
        var modalInstance = $modal.open({
            templateUrl: 'messageModal',
            controller: 'MessageModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return title;
                },
                bodyText: function () {
                    return body;
                },
                success: function () {
                    return success;
                }
            }
        });

        modalInstance.result.then(function () {
            if (success) {
                $window.location.href = 'Index#/WordSuites';
            }
        });
    }

    var openWordSuite = function (id) {
        WordSuiteService
            .getWordSuiteByID(id)
                .then(function (wordSuite) {
                    $scope.wordSuite = wordSuite;
                    if ($scope.wordSuite) {
                        oldName = $scope.wordSuite.Name;
                        oldThreshold = $scope.wordSuite.Threshold;
                        oldQuizResponseTime = $scope.wordSuite.QuizResponseTime;
                        oldProhibitedQuizzes = $scope.wordSuite.ProhibitedQuizzesId;

                        WordTranslationService
                        .getWordTranslationsByWordSuiteID($scope.wordSuite.Id)
                            .then(function (wordTranslations) {
                                $scope.existingWordTranslations = wordTranslations;
                                loadRecommendedTranslations();
                            });

                        LanguageService.getLanguage($scope.wordSuite.LanguageId)
                            .then(function (language) {
                                $scope.language = language;
                            });
                        WordSuiteService.getAllQuizzes()
                            .then(
                                function (quizzes) {
                                    $scope.allQuizzes = WordSuiteService.mapQuizzesForMultiSelect(quizzes, $scope.wordSuite.ProhibitedQuizzesId);
                                },
                                function (error) {

                                });
                    }
                });
    }

    $scope.AddWordTranslation = function (translation) {
        $scope.newWordTranslations.push({
            Id: translation.Id,
            OriginalWord: translation.OriginalWord,
            TranslationWord: translation.TranslationWord
        });
        translationWasAdded(translation);
    }

    // Returns true if translation exists in word suite
    var isTranslationExists = function (wordTranslation) {
        for (var i = 0; i < $scope.existingWordTranslations.length; i++)
            if (wordTranslation.Id == $scope.existingWordTranslations[i].Id)
                return true;
        for (var i = 0; i < $scope.newWordTranslations.length; i++)
            if (wordTranslation.Id == $scope.newWordTranslations[i].Id)
                return true;

        return false;
    }

    // Loads recommended translations to add into word suite
    var loadRecommendedTranslations = function () {
        WordSuiteService
        .getExtensionTranslations($scope.wordSuite.Id)
        .then(function (response) {
            $scope.suggestionTranslations = response;
            makeSuggestionToShow();
        });
    }

    // Makes array to show suggestions in the view
    var makeSuggestionToShow = function () {
        $scope.suggestionTranslationsToShow = [];
        for (var i in $scope.suggestionTranslations)
            if (isTranslationExists($scope.suggestionTranslations[i]) == false)
                $scope.suggestionTranslationsToShow.push($scope.suggestionTranslations[i]);
    }

    // When translation was added into word suite need to remove it from suggestions
    var translationWasAdded = function (translation) {
        for (var i in $scope.suggestionTranslationsToShow)
            if ($scope.suggestionTranslationsToShow[i].Id == translation.Id) {
                $scope.suggestionTranslationsToShow.splice(i, 1);
                break;
            }
    }

    // When translation was removed from word suite need to add it into
    // word suite if it exists in loaded suggestions
    var translationWasRemoved = function (translation) {
        for (var i in $scope.suggestionTranslations)
            if ($scope.suggestionTranslations[i].Id == translation.Id) {
                // Insert element in descening order by frequency
                var inserted = false;
                for (var j in $scope.suggestionTranslationsToShow)
                    if ($scope.suggestionTranslationsToShow[j].Freq < $scope.suggestionTranslations[i].Freq) {
                        $scope.suggestionTranslationsToShow.splice(j, 0, $scope.suggestionTranslations[i]);
                        inserted = true;
                        break;
                    }
                if (!inserted)
                    $scope.suggestionTranslationsToShow.push($scope.suggestionTranslations[i]);
                break;
            }
    }

    openWordSuite($routeParams.wordSuiteId);
};
EditWordSuiteCtrl.$inject = ["$scope", "$window", "$modal", "$routeParams", "ModalService", "UserService", "WordSuiteService", "WordTranslationService",
    "LanguageService"];
