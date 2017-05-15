app.controller('GlobalDictionaryController', ["$scope", "$window", "$timeout", "$modal", "WordTranslationService", "ConstService",
    "LanguageService", "ModalService", "WordsService","toastr","TagService",
    function ($scope, $window, $timeout, $modal, WordTranslationService, ConstService, LanguageService, ModalService,WordsService,toastr,TagService) {
        var languages;
        $scope.initialize = function () {
        $scope.selectedPage = 1;
        $scope.searchValue = '';
        $scope.isLanugageSelected = false;
        $scope.WORDS_ON_PAGE = ConstService.WORDS_ON_PAGE;
        $scope.wordsAmount = 0;
        $scope.tagsAmount = 0;
        $scope.searchTag = '';
        $scope.selectedOriginalLanguage;
        $scope.selectedTranslationLanguage;
        LanguageService.getAllLanguages()
        .then(function (response) {
            languages = angular.copy(response);
            $scope.originalLanguages = angular.copy(response);
            $scope.translationLanguages = angular.copy(response);
        });
    }

        $scope.changeDataToShow = function () {
        var regExp = /^#/;
        if ($scope.searchValue.match(regExp)) {
            $scope.searchTag = $scope.searchValue.slice(1, $scope.searchValue.length);
            TagService.getAmountOfTagsBySearchValue($scope.searchTag, $scope.selectedOriginalLanguage.Id, $scope.selectedTranslationLanguage.Id)
         .then(function (response) {
             $scope.tagsAmount = response;
             console.log($scope.tagsAmount);
             TagService.getWordsByTagValue(($scope.selectedPaarchge - 1) * ConstService.WORDS_ON_PAGE,
                     Math.min($scope.selectedPage * ConstService.WORDS_ON_PAGE, $scope.tagsAmount),
                     $scope.selectedOriginalLanguage.Id, $scope.searchTag, $scope.selectedTranslationLanguage.Id)
                 .then(function (response) {
                     $scope.toShow = response;
                 });
         }
           ) }
        else if ($scope.searchValue === '') {
               WordTranslationService.getAmountOfWordsBySpecificLanguage($scope.selectedOriginalLanguage.Id, $scope.selectedTranslationLanguage.Id, $scope.selectedPartsOfSpeech)
            .then(function (response) {
                $scope.wordsAmount = response;
                WordTranslationService.getWordsFromInterval(($scope.selectedPage - 1) * ConstService.WORDS_ON_PAGE,
                    Math.min($scope.selectedPage * ConstService.WORDS_ON_PAGE, $scope.wordsAmount),
                    $scope.selectedOriginalLanguage.Id, $scope.selectedTranslationLanguage.Id, $scope.selectedPartsOfSpeech)
        .then(function (response) {
            $scope.toShow = response;
        });
            });
        } else {
            WordTranslationService.getAmountOfWordsBySearchValue($scope.searchValue, $scope.selectedOriginalLanguage.Id, $scope.selectedTranslationLanguage.Id, $scope.selectedPartsOfSpeech)
        .then(function (response) {
            $scope.wordsAmount = response;
            WordTranslationService.getWordsBySearchValueFromInterval(($scope.selectedPage - 1) * ConstService.WORDS_ON_PAGE,
                Math.min($scope.selectedPage * ConstService.WORDS_ON_PAGE, $scope.wordsAmount),
                $scope.selectedOriginalLanguage.Id, $scope.searchValue, $scope.selectedTranslationLanguage.Id, $scope.selectedPartsOfSpeech)
            .then(function (response) {
                 $scope.toShow = response;
            });
        });
    }
    }

    function returnLanguagesExcept(language, languageToBeFirst) {
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

    $scope.selectOriginalLanguage = function () {
        $scope.isLanguageSelected = true;
        $scope.translationLanguages = returnLanguagesExcept($scope.selectedOriginalLanguage, $scope.selectedTranslationLanguage);


        LanguageService.getPartsOfSpeech($scope.selectedOriginalLanguage.Id)
        .then(function (responce) {
            $scope.partsOfSpeechForGD = LanguageService.mapPartsOfSpeechForGlobalDictionary(responce);
            $scope.selectedPartsOfSpeech = angular.copy($scope.partsOfSpeechForGD);
        });
      

        if ($scope.isTranslationLanguageSelected) {
            $scope.selectedPage = 1;
            $scope.searchValue = '';
            $scope.changeDataToShow();
        }
    }

    $scope.selectTranslationLanguage = function () {
        $scope.originalLanguages = returnLanguagesExcept($scope.selectedTranslationLanguage, $scope.selectedOriginalLanguage);
        $scope.isTranslationLanguageSelected = true;
        $scope.selectedPage = 1;
        $scope.searchValue = '';
        $scope.changeDataToShow();
    }

    $scope.open = function () {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'Views/AddWord.html',
            controller: 'WordsController',
            size: 'md',
            resolve: {
                languageId: function () {
                    return $scope.selectedOriginalLanguage.Id;
                },
                translationLanguageId: function () {
                    return $scope.selectedTranslationLanguage.Id;
                }
            }
        }).result.finally(function () {
            $scope.changeDataToShow();
        });
    }

    $scope.openImportModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'modalImport',
            controller: 'ImportWordTranslationsController',
            size: 'lg',
            resolve: {
                languageId: function () {
                    return $scope.selectedOriginalLanguage.Id;
                }
            }
        });

        modalInstance.result.then(function (importedWordTranslations) {
            if (importedWordTranslations) {
                $scope.importedWordTranslations = importedWordTranslations;
                $scope.openImportResultModal('Import Word Translations', 'Word Translations successfully imported', true, 'Create Word Suite from Word Translations');
            } else {
                $scope.openImportResultModal('Import Word Translations', 'Failed to import Word Translations', false, null);
            }
            $scope.changeDataToShow();
        });
    }

    $scope.openImportResultModal = function (title, body, success, actionButtonText) {
        var modalInstance = $modal.open({
            templateUrl: 'messageWithActionModal',
            controller: 'MessageWithActionModalController',
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
                },
                actionButtonText: function () {
                    return actionButtonText;
                }
            }
        });

        modalInstance.result.then(function (action) {
            if (success) {
                if (action) {
                    $scope.createWordSuite();
                }
            }
        });
    }

    $scope.createWordSuite = function () {
        var wordSuiteData = {
            importedWordTranslations: $scope.importedWordTranslations,
            languageId: Number($scope.selectedOriginalLanguageFromOptions)
        }
        localStorage.setItem('wordSuiteData', JSON.stringify(wordSuiteData));
        $window.location.href = 'Index#/CreateWordSuite';
    }
    
    //oleh works here
    $scope.isSelected = function (word) {
        return $scope.selectedWord === word;
    }

    $scope.selectWord = function (word) {
        if (!$scope.isSelected(word)) {
            onceSelected = true;
            WordTranslationService.getInformationAboutWordStrings(word.OriginalWord, word.LanguageId, $scope.selectedTranslationLanguage.Id).
                then(function (response) {
                    $scope.selectedWord = word;
                    $scope.fullWordInformation = response;
                }, function (error) {
                    console.log(error);
                });
        }
        else {
            $scope.selectedWord = null;
            onceSelected = false;
        }
    }

    $scope.editWord = function (OriginalWord) {
        
        var modalInstance = $modal.open({
            templateUrl: 'Views/EditWord.html',
            controller: 'EditWordController',
            size: 'md',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                wordV: function () {
                    return OriginalWord;
                },
                originLanguageId: function () {
                    return $scope.selectedOriginalLanguage.Id;
                },
                translationLanguageId: function () {
                    return $scope.selectedTranslationLanguage.Id;
                }
            }
        });
        modalInstance.result.finally(function () {
            $scope.changeDataToShow();
        });
        
    }

    $scope.removeWord = function (wordId) {
        var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Delete Word';
                },
                bodyText: function () {
                    return 'Are you sure you want to delete this word ? It could be in some Word Suites.';
                }
            }
        });
        modalInstance.result.then(function (yes) {
            if (yes) {
                WordsService
                    .deleteWord(wordId)
                    .then(
                        function (success) {
                            $scope.changeDataToShow();
                            toastr.success("Your word was successfuly deleted");
                        },
                        function (error) {
                            toastr.error("This word is in some Word Suites");
                        }
                    );
            }
        });
    }

    $scope.initialize();
}]);
