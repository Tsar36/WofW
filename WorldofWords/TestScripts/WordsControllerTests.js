describe('WordsController', function () {
    beforeEach(module('MyApp'));

    var controller;
    var initialize;
    var $scope;
    var $modalInstance = {};
    var $modal = {};
    var ConstServiceMock = {};
    var LanguageServiceMock = {};
    var ModalServiceMock = {};
    var UserServiceMock = {};
    var WordsServiceJasmine = {};
    var toastr = {};
    var passPromise = null;
    var languageId;
    var translationLanguageId;

    beforeEach(angular.mock.inject(function (_$controller_, _$rootScope_, _$q_, $injector,ConstService,LanguageService,ModalService,UserService,WordsService) {
        $scope = _$rootScope_.$new();
        controller = _$controller_;
        WordsServiceJasmine = Object.create($injector.get("WordsService"));
        LanguageServiceJasmine = Object.create($injector.get("LanguageService"));
        $modalInstance = {
            close: jasmine.createSpy('modalInstance.close'),
            result: {
                then: jasmine.createSpy('modalInstance.result.then')
            }
        };
        $q = _$q_;
        ConstServiceMock = ConstService;
        WordsServiceMock = WordsService;
        LanguageServiceMock = LanguageService;
        ModalServiceMock = ModalService;
        UserServiceMock = UserService;
        
        toastr = {
            success: jasmine.createSpy('toastr.success'),
            error: jasmine.createSpy('toastr.error')
        }

        spyOn(WordsServiceJasmine, 'addSynonym').and.callFake(function () {
            var deferred = $q.defer();
            if (passPromise) {
                deferred.resolve(true);
            }
            else {
                deferred.reject("something went wrong");
            }
            return deferred.promise;
        });

        spyOn(LanguageServiceJasmine, 'getAllLanguages').and.callFake(function () {
            var deferred = $q.defer();
            if (passPromise) {
                deferred.resolve(true);
            }
            else {
                deferred.reject("something went wrong");
            }
            return deferred.promise;
        });
    }));

    it('if Synonym exist', function () {
       var ctrl = controller('WordsController', {
            $scope: $scope, $modal: $modal, $modalInstance: $modalInstance,
            ConstService: ConstServiceMock, LanguageService: LanguageServiceJasmine, ModalService: ModalServiceMock, UserService: UserServiceMock, WordsService: WordsServiceJasmine, toastr: toastr, languageId: languageId, translationLanguageId: translationLanguageId, initialize: initialize
        });
       passPromise = true;

        $scope.synonymsModel = {
            synonyms:[]
        }
        $scope.selectedSyn = {
            Value:"pes"
        }

        $scope.addSyn();
        $scope.$root.$digest();
        expect(WordsServiceJasmine.addSynonym).toHaveBeenCalled();
        expect($scope.synonymsModel.synonyms[0]).toEqual("pes");
        expect($scope.selectedSyn).toEqual('');
    });

    it('if Translation has value', function () {
        var ctrl = controller('WordsController', {
            $scope: $scope, $modal: $modal, $modalInstance: $modalInstance,
            ConstService: ConstServiceMock, LanguageService: LanguageServiceJasmine, ModalService: ModalServiceMock, UserService: UserServiceMock, WordsService: WordsServiceJasmine, toastr: toastr, languageId: languageId, translationLanguageId: translationLanguageId, initialize: initialize
        });
        
        $scope.synonymsModel = {
            translations: []
        }
        $scope.selectedTranslation = {
            Value: "Pes"
        }

        $scope.selectedTranslation = "Pes";

        $scope.addTran();
        expect($scope.synonymsModel.translations[0]).toEqual("Pes");
        expect($scope.selectedTranslation).toEqual('');
        
    });

    it('if Translation has not value', function () {
        var ctrl = controller('WordsController', {
            $scope: $scope, $modal: $modal, $modalInstance: $modalInstance,
            ConstService: ConstServiceMock, LanguageService: LanguageServiceJasmine, ModalService: ModalServiceMock, UserService: UserServiceMock, WordsService: WordsServiceJasmine, toastr: toastr, languageId: languageId, translationLanguageId: translationLanguageId, initialize: initialize
        });

        $scope.selectedTranslation = '';

        $scope.addTran();
        expect($scope.selectedTranslation).toEqual('');
        

    });

   });