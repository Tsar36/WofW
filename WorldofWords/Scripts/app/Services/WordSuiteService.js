app.service('WordSuiteService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {
    this.createWordSuite = function (wordSuite) {
        var deferred = $q.defer();
        HttpRequest.post("api/wordsuite/CreateWordSuite", wordSuite, deferred);
        return deferred.promise;
    }

    this.editWordSuite = function (wordSuite) {
        var deferred = $q.defer();
        HttpRequest.post("api/wordsuite/EditWordSuite", wordSuite, deferred);
        return deferred.promise;
    }

    this.getWordSuitesByOwnerID = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("api/wordsuite/GetTeacherWordSuites?id=" + id, deferred);
        return deferred.promise;
    }

    this.getWordSuitesByLanguageID = function (languageId) {
        var deferred = $q.defer();
        HttpRequest.get("api/wordsuite/GetWordSuitesByLanguageId?languageId=" + languageId, deferred);
        return deferred.promise;
    }

    this.getWordSuiteByID = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("api/wordsuite/GetWordSuiteByID?id=" + id, deferred);
        return deferred.promise;
    }

    this.getWordsFromWordSuite = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("api/TrainingWordSuite/AllWords?id=" + id, deferred);
        return deferred.promise;
    }

    this.getExtensionTranslations = function (baseWordSuiteId) {
        var deferred = $q.defer();
        HttpRequest.get("api/TrainingWordSuite/ExtensionTranslationsFreq?baseWordSuiteId=" + baseWordSuiteId, deferred);
        return deferred.promise;
    }

    this.getPrintVersion = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('api/wordsuite/' + id + '/pdf', deferred);
        return deferred.promise;
    }

    this.addWordProgresses = function (wordProgresses) {
        var deferred = $q.defer();
        HttpRequest.post("api/wordsuite/AddWordProgresses", wordProgresses, deferred);
        return deferred.promise;
    }

    this.removeWordProgress = function (wordProgress) {
        var deferred = $q.defer();
        HttpRequest.post("api/wordsuite/RemoveWordProgresses", wordProgress, deferred);
        return deferred.promise;
    }

    this.deleteWordSuite = function (id) {
        var deferred = $q.defer();
        HttpRequest.delete("api/WordSuite?id=" + id, deferred);
        return deferred.promise;
    }

    this.getTeacherList = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('api/TeacherList/GetByRoleId?roleId=' + id, deferred);
        return deferred.promise;
    }

    this.shareWordSuite = function (teachersToShare) {
        var deferred = $q.defer();
        HttpRequest.post("api/wordsuite/ShareWordSuite",teachersToShare,deferred);
        return deferred.promise;
    }

    this.getAllQuizzes = function () {
        var deferred = $q.defer();
        HttpRequest.get("api/wordsuite/allQuizzes", deferred);
        return deferred.promise;
    }

    // functions overloading - if selectedQuizzesId === undefined (function with one argument) -> mapForCreate
    //                         else ->  mapForEdit
    this.mapQuizzesForMultiSelect = function (quizzes, selectedQuizzesId) {
        if (selectedQuizzesId === undefined) {
            quizzes.forEach(function (item, i, arr) {
                item.isSelected = false;
            });
        }
        else {
            quizzes.forEach(function (quiz, i, quizzesArr) {
                selectedQuizzesId.some(function (selectedQuizId, j, selectedQuizzesIdArr) {
                    if (quiz.Id == selectedQuizId) {
                        quiz.isSelected = true;
                        return true;
                    }
                    else{
                        quiz.isSelected = false;
                    }
                });
            });
        }

        return quizzes;
    }

    this.getIdFromQuizzes = function (quizzes) {
        var ids = [];
        quizzes.forEach(function (item, i, arr) {
            ids.push(item.Id);
        });

        return ids;
    }

    this.areArraysEqual = function (arr1, arr2) {  
        if (!Array.isArray(arr1) || !Array.isArray(arr2)) {
            return false;
        }

        if (arr1.length != arr2.length) {
            return false;
        }
        
        arr1.sort();
        arr2.sort();
        for (var i = 0; i < arr1.length; ++i) {
            if (arr1[i] !== arr2[i]) {
                return false;
            }
        }
        return true;
    }

}]);