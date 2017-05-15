app.controller('SynonymQuizController', ["$scope", "$window", "$routeParams", "$modal", "QuizService", "WordSuiteService",
    function ($scope, $window, $routeParams, $modal, QuizService, WordSuiteService) {
    $scope.$on('$locationChangeStart', function (event) {
        if (!$scope.isTimeOut) {
            event.preventDefault();
            $scope.ConfirmSubmit();
        }
    });

    $scope.open = function (url, successCallback, data, errorCallback) {
        var modalInstance = $modal.open({
            animation: true,
            templateUrl: url,
            controller: 'ModalStartQuizController',
            size: 'sm',
            resolve: {
                data: function () {
                    return data;
                }
            }
        });
        modalInstance.result.then(successCallback, errorCallback);
    };

    $scope.Stop = function () {
        clearInterval($scope.timerInterval);
        timer.stop();
    };

    $scope.Start = function () {
        $scope.label = $scope.model.WordSuiteName;
        $scope.itemsPerPage = 10;
        $scope.$watch('currentPage + itemsPerPage', function () {
            sliceWords();
        });
        $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
            var h = document.getElementById("parentQuiz").offsetHeight;
            document.getElementById("left-arrow").style.height = h + "px";
            document.getElementById("right-arrow").style.height = h + "px";
        });
        timer.start();
        $scope.isTimeOut = false;
        $scope.timerInterval = setInterval(showTime, 100);
    };

    $scope.goToCourses = function () {
        $window.location.href = 'Index#/CourseDetail/' + JSON.parse(localStorage.getItem('courseId'));
    };

    $scope.getCurrentIndex = function (index) {
        return ($scope.currentPage - 1) * $scope.itemsPerPage + index;
    };

    $scope.ConfirmSubmit = function () {
        $scope.open('Confirmation', $scope.Submit);
    };

    $scope.Submit = function () {
        var message = 'Your result are saved!';
        if ($scope.isTimeOut) {
            message += ' Time is out!';
        }
        $scope.isTimeOut = true;
        $scope.Stop();

        $scope.model.Result = null;
        QuizService.checkSynonymTask($scope.model)
        .then(function (response) {
            $scope.isChecked = true;
            $scope.model = response;

            sliceWords();

            $scope.rightAnswerCount = 0;
            for (var x in $scope.model.Result) {
                if ($scope.model.Result[x]) {
                    ++$scope.rightAnswerCount;
                };
            };

            $scope.isSubmitSuccessful = true;
            $scope.open('Result', function () { }, (message + ' Result: ' + $scope.rightAnswerCount + '/' + $scope.totalItems));
        },
        function (error) {
            $window.open('An error has occured', function () { } , (error.ExceptionMessage || error.Message || "Unknown error"));
        });
    };

    var MilisecondsInSecond = 1000;
    var timer = new Timer();

    var showTime = function () {
        if (timer.getMiliseconds() > timer.getGoalTime() && timer.getGoalTime() !== 0) {
            $scope.isTimeOut = true;
            $scope.Submit();
        }
        else {
            $scope.time = timer.getGoalTime() - timer.getMiliseconds();
        }
        $scope.$apply();
    };

    var sliceWords = function () {
        var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
            end = begin + $scope.itemsPerPage;
        var synonymsOnPage = $scope.model.Synonyms.slice(begin, end);
        $scope.filteredWords = [];
        for (var i in synonymsOnPage) {
            var synonymList = "";
            for (var j in synonymsOnPage[i])
                synonymList += (j == 0 ? synonymsOnPage[i][j].Value : ", " + synonymsOnPage[i][j].Value);
            $scope.filteredWords.push(synonymList);
        }
    };

    var initialize = function () {
        $scope.isTimeOut = true;
        $scope.isChecked = false;
        $scope.currentPage = 1;

        QuizService.getSynonymTask($routeParams.wordSuiteId)
            .then(function (response) {
                $scope.model = response;

                if (!$scope.model.Synonyms.length) {
                    $scope.open('errorModal', $scope.goToCourses, 'There aren\'t any words to learn in this quiz');
                } else {
                    $scope.totalItems = $scope.model.Synonyms.length;

                    $scope.model.Answer = [];
                    for (var i = 0; i < $scope.totalItems; i++)
                        $scope.model.Answer.push('');
                    
                    $scope.time = response.QuizResponseTime * $scope.totalItems * MilisecondsInSecond;
                    timer.setGoalTime($scope.time);
                    $scope.open('startQuiz', $scope.Start, $scope.time, $scope.goToCourses);
                };
            }, function (error) {
                    $scope.open('errorModal', $scope.goToCourses, (error.ExceptionMessage || error.Message || "Unknown error"));
            });
    };

    initialize();
}]);

app.controller('ModalStartQuizController', ["$scope", "$modalInstance", "data", function ($scope, $modalInstance, data) {
    $scope.data = data;
    $scope.yes = function () {
        $modalInstance.close();
    }

    $scope.no = function () {
        $modalInstance.dismiss();
    }
}]);