app.controller('WordSuitesController', WordSuitesCtrl);

function WordSuitesCtrl($modal, $scope, $modal, ModalService, UserService, WordSuiteService, HttpRequest, HubService) {
    $scope.wordSuites = [];
    $scope.wordSuitesToShow = [];
    $scope.wordSuitesToShowCount = 0;
    $scope.showWordSuites = function () {
        var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
            end = begin + $scope.itemsPerPage;

        if ($scope.orderBy === 'Date') {
            $scope.wordSuites.sort(orderBy('Id'));
        }

        if ($scope.orderBy === 'Name') {
            $scope.wordSuites.sort(orderBy('Name'));
        }

        if ($scope.searchName) {
            var wordSuitesContainingName = [];

            for (var i = 0; i < $scope.wordSuites.length; i++) {
                if ($scope.wordSuites[i].Name.toLowerCase().indexOf($scope.searchName) != -1) {
                    wordSuitesContainingName.push($scope.wordSuites[i]);
                }
            }

            $scope.wordSuitesToShow = wordSuitesContainingName.slice(begin, end);
            $scope.wordSuitesToShowCount = wordSuitesContainingName.length;
        }
        else {
            $scope.wordSuitesToShow = $scope.wordSuites.slice(begin, end);
            $scope.wordSuitesToShowCount = $scope.wordSuites.length;
        }
    };



    $scope.wordSuiteShare = function (index) {

        var modalInstance = $modal.open({
            animation: true,
            templateUrl: 'Views/WordSuiteShare.html',
            controller: 'WordSuiteShareController',
            size: 'sm',
            resolve: {
                wordSuiteId: function () {
                    return $scope.wordSuites[index].Id;
                }
            }
        });
    };
    $scope.removeWordSuite = function (index) {
        var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Delete WordSuite';
                },
                bodyText: function () {
                    return 'Are you sure you want to delete this Word Suite?';
                }
            }
        });

        modalInstance.result.then(function (yes) {
            if (yes) {
                WordSuiteService
                    .deleteWordSuite($scope.wordSuites[index].Id)
                    .then(
                        function (success) {
                            getWordSuites(UserService.getUserData().id);
                        },
                        function (error) {
                            ModalService.showResultModal('Delete Word Suite', 'Failed to delete Word Suite. Some students may be assigned to it.', false);
                        }
                    );
            }
        });
    }

    var orderBy = function (property) {
        return function (a, b) {
            return result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
        }
    }

    var getWordSuites = function (id) {
        WordSuiteService
        .getWordSuitesByOwnerID(id)
            .then(
            function (wordSuites) {
                $scope.wordSuites = wordSuites;
                $scope.showWordSuites();
            },
            function (error) {
                ModalService.showResultModal('Load Word Suites', 'Failed to load Word Suites', false);
            });
    }

    getWordSuites(UserService.getUserData().id);

    $scope.$on('updateWordSuites', function (e) {
        $scope.$apply(function () {
            getWordSuites(UserService.getUserData().id);
        })
    });

};
WordSuitesCtrl.$inject = ["$modal", "$scope", "$modal", "ModalService", "UserService", "WordSuiteService", "HttpRequest","HubService"];