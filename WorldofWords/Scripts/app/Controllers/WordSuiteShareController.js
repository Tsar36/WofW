app.controller('WordSuiteShareController',["$scope", "$modalInstance", "$modal", "UserService", "WordSuiteService", "ModalService", "HubService", 
    "wordSuiteId",function ($scope, $modalInstance, $modal, UserService, WordSuiteService, ModalService, HubService, wordSuiteId) {


        $scope.init = function () {
            $scope.teachersRoleId = 2;
            $scope.teachersList = [];
            $scope.teachersToShow = []; 
            $scope.teachersToShare = {
                teachersId: [],
                wordSuiteId : wordSuiteId
            };

            WordSuiteService.getTeacherList($scope.teachersRoleId)
                     .then(
                     function (teacherList) {
                         $scope.teachersList = teacherList;
                         $scope.showTeachers();
                     }
                     );
        }
        
        $scope.showTeachers = function () {
            var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
                end = begin + $scope.itemsPerPage;

            if ($scope.searchName) {
                var searchResult = [];
                for (var i = 0; i < $scope.teachersList.length; ++i) {
                    if (($scope.searchName && $scope.teachersList[i].Name.toLowerCase().indexOf($scope.searchName.toLowerCase()) !== -1))
                        searchResult.push($scope.teachersList[i]);
                }

                $scope.teachersToShow = searchResult.slice(begin,end);
                $scope.teachersToShowCount = searchResult.length;

            }
            else {

                $scope.teachersToShow = $scope.teachersList.slice(begin,end);
                $scope.teachersToShowCount = $scope.teachersList.length;
              
            }
        };

            $scope.closeModal = function () {
                $modalInstance.close();
            };

            $scope.submitModal = function () {
                var modalInstance = $modal.open({
                    templateUrl: 'confirmModal',
                    controller: 'ConfirmModalController',
                    size: 'sm',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {

                        titleText: function () {
                            return 'Sharing Word Suites';
                        },
                        bodyText: function () {
                            return 'Submit changes?';
                        }
                    }
                });

                modalInstance.result.then(function (yes) {
                    if (yes) {
                        WordSuiteService
                            .shareWordSuite($scope.teachersToShare)
                            .then(
                                function (success) {
                                    ModalService.showResultModal('Share word suite', 'Word suite have been succesfully shared', true);
                                    HubService.notifyAboutSharedWordSuites($scope.teachersToShare.teachersId)
                                    $scope.closeModal();
                                },
                                function (error) {
                                    ModalService.showResultModal('Share word suite', 'Word suite have not been shared', false);
                                    $scope.closeModal();
                                }
                            );
                    }
                }
                )
            };


            $scope.init();
            
        }]);
