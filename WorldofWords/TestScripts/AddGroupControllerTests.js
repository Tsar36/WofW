describe('AddGroupController', function () {
    var $scope, GroupServiceMock, data = [{ name: 'English', Id: 5 }, { name: 'German', Id: 1 }], UserServiceMock;

    beforeEach(module('MyApp'));

    describe('invocation of scope.init success', function () {
        beforeEach(inject(function ($controller, $rootScope) {
            $scope = $rootScope.$new();

            GroupServiceMock = {
                getAllCourses: function () {
                    return {
                        then: function (response) { return response(data); }
                    };
                }
            };

            $controller('AddGroupController', {
                '$scope': $scope,
                'GroupService': GroupServiceMock
            });
        }));

        it('Then the mock service should have returned the correct response', function () {
            expect($scope.init).toBeDefined();
            expect($scope.showSuccess).toBe(false);
            expect($scope.showFailure).toBe(false);
            expect($scope.showFailureEmptyName).toBe(false);
            expect($scope.failureEmptyNameMessage).toBe('Please enter the name for your group!');
        });

        it("should do something else when something happens", function () {
            expect($scope.names).toEqual(data);
            expect($scope.courseIdValue).toEqual(5);
        });
    });

    describe('invocation of scope.init return empty array', function () {
        beforeEach(inject(function ($controller, $rootScope) {
            $scope = $rootScope.$new();

            GroupServiceMock = {
                getAllCourses: function () {
                    return {
                        then: function (response) { return response([]); }
                    };
                }
            };

            $controller('AddGroupController', {
                '$scope': $scope,
                'GroupService': GroupServiceMock
            });
        }));

        it("courseIdValue should be undefined", function () {
            expect($scope.init).toBeDefined();
            expect($scope.names).toEqual([]);
            expect($scope.courseIdValue).not.toBeDefined();
        });
    });

    describe('single methods, that set bool variables', function () {
        beforeEach(inject(function ($controller, $rootScope) {
            $scope = $rootScope.$new();
            GroupServiceMock = {
                createGroup: function (groupModel) {
                    return {
                        then: function (ok) { return 'ok'; }
                    };
                },
                getAllCourses: function () {
                    return {
                        then: function (response) { return response([]); }
                    };
                }
            };

            UserServiceMock = {
                getUserData: function () { return { id: 55 } }
            };

            var controller = $controller('AddGroupController', {
                '$scope': $scope,
                'GroupService': GroupServiceMock,
                'UserService': UserServiceMock
            });
        }));

        it('call showSuccess method that set showSuccess in true', function () {
            //before call
            expect($scope.showSuccess).toBe(false);
            expect($scope.nameValue).not.toBeDefined();
            expect($scope.notifySuccess).toBeDefined();
            //call method
            $scope.nameValue = "Lv-159.Net";
            $scope.notifySuccess();
            $scope.$root.$digest();
            //after call
            expect($scope.showSuccess).toBe(true);
            expect($scope.successMessage).toEqual('Group Lv-159.Net has been added.');
        });

        it('call notifyFailure method with empty input that set failureMessage in "SomethingWrong"', function () {
            //before call
            expect($scope.showFailure).toBe(false);
            expect($scope.failureMessage).not.toBeDefined();
            expect($scope.notifyFailure).toBeDefined();
            //call method, input string equal null
            $scope.notifyFailure();
            $scope.$root.$digest();
            //after call
            expect($scope.failureMessage).toBe('SomethingWrong');
            expect($scope.showFailure).toBe(true);
        });

        it('call notifyFailure method with empty input that set failureMessage in "Failure"', function () {
            //before call
            expect($scope.showFailure).toBe(false);
            expect($scope.failureMessage).not.toBeDefined();
            expect($scope.notifyFailure).toBeDefined();
            //call method, input string equal 'Failure'
            $scope.notifyFailure('Failure');
            $scope.$root.$digest();
            //after call
            expect($scope.failureMessage).toBe('Failure');
            expect($scope.showFailure).toBe(true);
        });

        it('call notifyFailureEmptyName set showFailureEmptyName in true', function () {
            //before call
            expect($scope.showFailureEmptyName).toBe(false);
            expect($scope.notifyFailureEmptyName).toBeDefined();
            //call method
            $scope.notifyFailureEmptyName();
            $scope.$root.$digest();
            //after call
            expect($scope.showFailureEmptyName).toBe(true);
        });

        it('call hideSuccess set showSuccess in false', function () {
            //before call
            expect($scope.showSuccess).toBe(false);
            expect($scope.hideSuccess).toBeDefined();
            //call method
            $scope.hideSuccess();
            $scope.$root.$digest();
            //after call
            expect($scope.showSuccess).toBe(false);
        });

        it('call hideFailure set showSuccess in false', function () {
            //before call
            expect($scope.showSuccess).toBe(false);
            expect($scope.hideFailure).toBeDefined();
            //call method
            $scope.hideFailure();
            $scope.$root.$digest();
            //after call
            expect($scope.showSuccess).toBe(false);
        });

        it('call hideFailureEmptyName set showSuccess in false', function () {
            //before call
            expect($scope.showFailureEmptyName).toBe(false);
            expect($scope.hideFailureEmptyName).toBeDefined();
            //call method
            $scope.hideFailureEmptyName();
            $scope.$root.$digest();
            //after call
            expect($scope.showFailureEmptyName).toBe(false);
        });

    });
});