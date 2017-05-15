app.controller('WordSuiteWordsController',["$scope", "$window", "$modal", "$routeParams", "WordSuiteService", "LanguageService",
    function ($scope, $window, $modal, $routeParams, WordSuiteService, LanguageService) {

    $scope.PdfWords = function () {
        $window.open("../api/wordsuite/pdfwords?id=" + $scope.wordSuite.Id);
    }

    $scope.PdfTask = function () {
        $window.open("../api/wordsuite/pdftask?id=" + $scope.wordSuite.Id);
    }

    $scope.openAddWordTranslationModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addWordTranslationModal',
            controller: 'AddWordTranslationsModalController',
            size: 'lg',
            resolve: {
                id: function () {
                    return $scope.wordSuite.Id;
                },
                languageId: function () {
                    return $scope.wordSuite.LanguageId;
                },
                existingWordTranslations: function () {
                    return $scope.wordSuite.WordTranslations;
                }
            }
        });

        modalInstance.result.then(function (success) {
            if (success) {
                initialize();
            }
        });
    }

    $scope.listen = function (word) {
        if ('speechSynthesis' in window && language) {
            var speech = new SpeechSynthesisUtterance(word);
            speech.lang = language;
            window.speechSynthesis.speak(speech);
        }
    }

    $scope.remove = function (index) {
        var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Delete Word Translation';
                },
                bodyText: function () {
                    return 'Are you sure you want to delete this Word Translation?';
                }
            }
        });

        modalInstance.result.then(function (answer) {
            if (answer) {
                WordSuiteService
                    .removeWordProgress({
                    WordSuiteId: $scope.wordSuite.Id,
                    WordTranslationId: $scope.wordSuite.WordTranslations[index].Id
                })
                .then(function (ok) {
                    initialize();
                },
                function (badRequest) {
                    console.log('fail');
                });
            }
        });
    }

    $scope.sort = function (order, property) {
        $scope.wordSuite.WordTranslations.sort(orderBy(property));
        if (!order) {
            $scope.wordSuite.WordTranslations.reverse();
        };
    };

    var language;
    $scope.records = [];

    var initialize = function () {
        WordSuiteService.getWordsFromWordSuite($routeParams.wordSuiteId)
        .then(function (response) {
            $scope.wordSuite = response;
            getLanguageCode($scope.wordSuite.LanguageId);
            for (var i = 0; i < response.WordTranslations.length; i++) {
                $scope.records[i] = new Object();
                $scope.records[i].Content = null;
            }
        });
    };

    var getLanguageCode = function (languageId) {
        LanguageService.getAllLanguages()
        .then(function (languages) {
            var languageName;
            for (var i = 0; i < languages.length; i++) {
                if (languages[i].Id === languageId) {
                    languageName = languages[i].Name;
                    break;
                }
            }
            if (languageName) {
                switch (languageName) {
                    case 'English':
                        language = 'en-US';
                        break;
                    case 'German':
                        language = 'de-DE';
                        break;
                    case 'French':
                        language = 'fr-FR';
                        break;
                }
            }
        });
    }

    var orderBy = function (property) {
        return function (a, b) {
            return (String(a[property]).toLowerCase() < String(b[property]).toLowerCase()) ?
                -1 : (String(a[property]).toLowerCase() > String(b[property]).toLowerCase()) ? 1 : 0;
        }
    };

    initialize();
}]);