app.controller('CreateWordSuiteController', CreateWordSuiteCtrl);

function CreateWordSuiteCtrl($scope, $window, $modal, ModalService, UserService, WordSuiteService, WordTranslationService, LanguageService) {
    $scope.wordTranslationsToAdd = [];
    $scope.prohibitedQuizzesToAdd = [];
    $scope.selectTranslationLanguage;
    $scope.isTranslationLanguageSelected;
    $scope.translationLanguages;
    $scope.translationLanguage;

    $scope.languageChanged = function () {
        if ($scope.wordTranslationsToAdd.length) {
            var modalInstance = $modal.open({
                templateUrl: 'confirmModal',
                controller: 'ConfirmModalController',
                size: 'sm',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    titleText: function () {
                        return 'Change language';
                    },
                    bodyText: function () {
                        return 'This action will remove all WordTranslations from WordSuite. Do you want to continue?';
                    }
                }
            });

            modalInstance.result.then(function (yes) {
                if (yes) {
                    oldLanguage = $scope.language;
                    console.log(oldLanguage);
                    $scope.wordTranslationsToAdd = [];
                    $scope.selectedWordTranslation = null;
                } else {
                    $scope.language = oldLanguage;
                }
            });
        } else {
            oldLanguage = $scope.language;
            $scope.selectedWordTranslation = null;
        }
    }

    $scope.selectTranslationLanguage = LanguageService.getAllLanguages()
        .then(function (response) {
        $scope.isTranslationLanguageSelected = true;
        languages = angular.copy(response);
        $scope.translationLanguages = languages;
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
            .searchForWordTranslations(searchWord, tags, $scope.language.Id, searchByTag)
                .then(
                function (wordTranslations) {
                    return wordTranslations;
                },
                function (error) {
                    ModalService.showResultModal('Search Word Translations', 'Error while searching Word Translations', false);
                });
    }

    $scope.addWordTranslationToWordSuite = function () {
        if ($scope.selectedWordTranslation && $scope.selectedWordTranslation.Id) {
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

    $scope.saveWordSuite = function () {
        if (isCorrectInput() && $scope.wordTranslationsToAdd.length) {
            var wordTranslationsId = [];

            for (var i = 0; i < $scope.wordTranslationsToAdd.length; i++) {
                wordTranslationsId.push($scope.wordTranslationsToAdd[i].Id);
            }

            var wordSuite = {
                Name: $scope.name,
                LanguageId: $scope.language.Id,
                TranslationLanguageId: $scope.translationLanguage.Id,
                Threshold: parseInt($scope.threshold),
                QuizResponseTime: parseInt($scope.quizResponseTime),
                OwnerId: UserService.getUserData().id,
                WordTranslationsId: wordTranslationsId,
                ProhibitedQuizzesId: WordSuiteService.getIdFromQuizzes($scope.prohibitedQuizzesToAdd)
            }

            WordSuiteService
            .createWordSuite(wordSuite)
                .then(
                function (ok) {
                    showResultModal('Create WordSuite', 'WordSuite successfully created', true)
                },
                function (badRequest) {
                    showResultModal('Create WordSuite', badRequest.Message, false)
                });
        }
    }

    $scope.importWordTranslations = function () {
        var modalInstance = $modal.open({
            templateUrl: 'modalImport',
            controller: 'ImportWordTranslationsController',
            size: 'lg',
            resolve: {
                languageId: function () {
                    return $scope.language.Id;
                }
            }
        });

        modalInstance.result.then(
            function (importedWordTranslations) {
                for (var i = 0; i < importedWordTranslations.length; i++) {
                    var wordFound = false;

                    for (var j = 0; j < $scope.wordTranslationsToAdd.length; j++) {
                        if (importedWordTranslations[i].Id === $scope.wordTranslationsToAdd[j].Id) {
                            wordFound = true;
                            break;
                        }
                    }

                    if (!wordFound) {
                        $scope.wordTranslationsToAdd.push(importedWordTranslations[i]);
                    }
                }
            });
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
                    return $scope.language.Id;
                }
            }
        });
    }

    var languages; 

    var oldLanguage;

    var init = function () {
        WordSuiteService.getAllQuizzes()
        .then(
             function (quizzes) {
                 $scope.allQuizzes = WordSuiteService.mapQuizzesForMultiSelect(quizzes);
             },
             function (error) {
                 showResultModel('Load quizzes', 'Failed to load quizzes', false);
             });

        LanguageService.getAllLanguages()
        .then(
            function (languages) {
                $scope.languages = languages;
                getWordSuiteLocalStorageData();
            },
            function (error) {
                showResultModal('Load languages', 'Failed to load languages', false);
            });
    }

    var isCorrectInput = function () {
        $scope.isEnteredName = Boolean($scope.name);
        $scope.isEnteredLanguage = Boolean($scope.language);
        $scope.isEnteredThreshold = Boolean($scope.threshold);
        $scope.isEnteredQuizResponseTime = Boolean($scope.quizResponseTime);

        return $scope.isEnteredName && $scope.isEnteredLanguage && $scope.isEnteredThreshold && $scope.isEnteredQuizResponseTime;
    }

    var getWordSuiteLocalStorageData = function () {
        var wordSuiteData = JSON.parse(localStorage.getItem('wordSuiteData'));
        if (wordSuiteData) {
            for (var i = 0; i < $scope.languages.length; i++) {
                if ($scope.languages[i].Id === wordSuiteData.languageId) {
                    $scope.language = $scope.languages[i];
                    oldLanguage = $scope.language;
                    break;
                }
            }
            $scope.wordTranslationsToAdd = angular.copy(wordSuiteData.importedWordTranslations);
            localStorage.setItem('wordSuiteData', JSON.stringify(null));
        }
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
                if (localStorage.getItem('Location')) {
                    $window.location.href = localStorage.getItem('Location');
                } else {
                    $window.location.href = 'Index#/WordSuites';
                };
            }
        });
    }

    var filterForTranslationLanguages = function translationLanguagesForView(language, languageToBeFirst) {
        var allLanguages = angular.copy(languages);
        for (var i = 0; i < allLanguages.length; ++i) {
            if (allLanguages[i].Id == language.Id) {
                allLanguages.splice(i, 1);
            }
            else if (languageToBeFirst && languageToBeFirst.Id == allLanguages[i].Id) {
                allLanguages.splice(i, 1);
                allLanguages.splice(0, 0, languageToBeFirst);
            }
        }
        return allLanguages;
    }

    init();
};
CreateWordSuiteCtrl.$inject = ["$scope", "$window", "$modal", "ModalService", "UserService", "WordSuiteService", "WordTranslationService", "LanguageService"];