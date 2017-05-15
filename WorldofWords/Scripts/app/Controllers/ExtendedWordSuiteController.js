app.controller('ExtendedWordSuiteController', ["$scope", "$routeParams", "WordSuiteService",
    "GroupService", "EditUserProfileSevice", function ($scope, $routeParams, WordSuiteService,
    GroupService, EditUserProfileSevice) {
    
    GroupService.getGroupById($routeParams.groupId)
    .then(function (response) {
        $scope.group = response;
    });

    $scope.dropSuccessHandler = function ($event, index, array) {
        array.splice(index, 1);
    };

    $scope.onDrop = function ($event, $data, array) {
        array.push($data);
    };


    $scope.saveWordSuite = function () {
        $scope.teacherEditWordSuite.WordTranslationsToAddIdRange = [];

        for (var i in $scope.teacherWordTranslations)
            if ($scope.teacherWordTranslations[i].IsStudentWord) {
                $scope.teacherEditWordSuite.WordTranslationsToAddIdRange.push(
                    $scope.teacherWordTranslations[i].Id);
                $scope.teacherWordTranslations[i].IsStudentWord = false;
            }
        WordSuiteService.editWordSuite($scope.teacherEditWordSuite);
    } 

    // Removes all word translations from "array" that exist in "translationsToDelete" array
    function deleteWordTranslationsFromArray(array, translationsToDelete) {
        for (var i = array.length - 1; i >= 0; i--)
            for (var j = 0; j < translationsToDelete.length; j++)
                if (array[i].Id == translationsToDelete[j].Id) {
                    array.splice(i, 1);
                    break;
                }
    }

    // Removes not student word translations from "array"
    function deleteNotStudentWordtranslations(array) {
        for (var i = array.length - 1; i >= 0; i--)
            if (!array[i].IsStudentWord)
                array.splice(i, 1);
    }

    function init() {
        WordSuiteService.getWordSuiteByID($routeParams.suiteId)
        .then(function (response) {
            $scope.wordSuiteName = response.Name;

            return WordSuiteService.getWordSuiteByID(response.PrototypeId);
        })
        .then(function (response) {
            $scope.teacherEditWordSuite = response;

            return WordSuiteService.getWordsFromWordSuite($scope.teacherEditWordSuite.Id);
        })
        .then(function (response) {
            $scope.teacherWordTranslations = response.WordTranslations;

            return WordSuiteService.getWordsFromWordSuite($routeParams.suiteId);
        })
        .then(function (response) {
            $scope.studentWordTranslations = response.WordTranslations;
            $scope.userId = response.OwnerId;
            EditUserProfileSevice.getUserNameById(response.OwnerId)
            .then(function (response) {
                $scope.userName = response;
            });

            deleteWordTranslationsFromArray($scope.studentWordTranslations,
                $scope.teacherWordTranslations);

            deleteNotStudentWordtranslations($scope.studentWordTranslations);
            $scope.error = false;
        })
        .catch(function (fail) {
            $scope.error = true;
        });
    }

    init();
}]);