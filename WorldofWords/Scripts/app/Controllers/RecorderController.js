app.controller('RecorderController', ['$scope', '$window', '$timeout', '$interval', 'RecorderService', 'ConstService', 'ShareScopeService', 'toastr',
    function ($scope, $window, $timeout, $interval, RecorderService, ConstService, ShareScopeService, toastr) {
    var initialize = function () {
        $window.URL = $window.URL || $window.webkitURL;
        $window.AudioContext = $window.AudioContext || $window.webkitAudioContext;
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia;
       
        $scope.isRecording = false;
        $scope.isPlaying = false;
        $scope.canBePlayed = false;
        $scope.canBeDeleted = true;
        $scope.recordButtonClass = 'btn btn-default';
        $scope.playButtonClass = 'btn btn-default';
        $scope.recordProgressBarClass = 'progress hideable';
        $scope.recordProgressBarState = 0;
        $scope.playProgressBarClass = 'progress hideable';
        $scope.disableComment = true;
        $scope.playProgressBarState = 0;
        $scope.submitButtonClass = 'btn btn-default no-margin-left no-margin-right pull-right';
        $scope.submit = false;
        $scope.canBeSubmited = true;

        $scope.currentTime = 0;
        $scope.totalTime = 4;
        $scope.currentPlayerTime = 0;
        $scope.totalPlayerTime = 0;

        $scope.$on('modal_closed', function () {
            RecorderService.onClose();
            if (mediaStreamSource != null)
                mediaStreamSource.disconnect();
        });
    }

    var mediaStreamSource;
    var recorderIsInitialized = false;
    var connectionFailed = false;

    var output = {
        Content: [],
        Description: '',
        WordId: 0
    };
    
    var onFailConnectionToMicrophone = function (e) {
        $scope.recordButtonPressed();
        toastr.error(ConstService.unableToConnectToMicrophone);
        connectionFailed = true;
        $scope.submit = true;
        $scope.canBeSubmited = false;
        stopRecording();
    }

    var onSuccessConnectionToMicrophone = function (source) {
        var context = new AudioContext()
        mediaStreamSource = context.createMediaStreamSource(source);
        RecorderService.initializeSource(mediaStreamSource);
        recorderIsInitialized = true;
        startRecording();
    }

    function initRecorder() {
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia;
        if (navigator.getUserMedia) {
            navigator.getUserMedia({ audio: true },
                onSuccessConnectionToMicrophone,
                onFailConnectionToMicrophone);
        } else {
            toastr.error(ConstService.unableToConnectToMicrophone);
        }
    }

    var startRecording = function () {
        RecorderService.record();
        $timeout(stopRecording, 4000);
    }

    var stopRecording = function () {
            $scope.recordButtonClass = !connectionFailed ? 'btn  btn-primary' : 'btn  btn-default';
            $scope.recordProgressBarClass = 'progress hideable';
            $scope.canBePlayed = true;
            $scope.isRecording = false;
            $scope.disableComment = false;
            $scope.canBeDeleted = true;
            resetProgressBars('record');

            RecorderService.stop();
    }

    var playRecord = function () {
        resetProgressBars('play');
        $interval(trackTime, 1000, $scope.totalPlayerTime);
        RecorderService.play();  
    }

    var stopPlayingRecord = function () {
        RecorderService.stopPlaying();
        resetProgressBars('play');
    }


    var resetProgressBars = function (resolver) {
        switch (resolver) {
            case 'record':
                $scope.recordProgressBarState = 0.25;
                $scope.currentTime = 1;
                break;
            case 'play':
                $scope.playProgressBarState = 0;
                $scope.currentPlayerTime = 0;
                $scope.totalPlayerTime = Math.ceil(RecorderService.getTotalTime());
                if ($scope.totalPlayerTime === 0) { $scope.totalPlayerTime = 1; }
                if ($scope.totalPlayerTime > 4) { $scope.totalPlayerTime = 4; }
                break;
        }
    }

    var deleteRecord = function () {
        RecorderService.deleteRecord();
    }

    var trackTime = function () {
        if ($scope.playProgressBarState < 1) {
            $scope.currentPlayerTime++;
            $scope.playProgressBarState = ($scope.currentPlayerTime / $scope.totalPlayerTime) <= 1 ? ($scope.currentPlayerTime / $scope.totalPlayerTime) : 1;

            if ($scope.playProgressBarState === 1) { $scope.playButtonClass = 'btn btn-default'; }
        } 
    }

    $scope.recordButtonPressed = function () {
            $scope.isRecording = !$scope.isRecording;
            if ($scope.isRecording) {
                $scope.recordButtonClass = 'btn btn-danger active';
                $scope.recordProgressBarClass = 'progress hideable expand';
                $scope.canBeDeleted = false;
                resetProgressBars('record');
                $interval(function () {
                    if ($scope.currentTime < 4) { $scope.currentTime++; }
                    $scope.recordProgressBarState = $scope.currentTime / $scope.totalTime;
                }, 1000, 4);
                if (!recorderIsInitialized && !connectionFailed) {
                    initRecorder();
                } else  if (!connectionFailed) {
                    startRecording();
                }
            } else {
                stopRecording();
            }
    }

    $scope.playButtonPressed = function () {
        $scope.isPlaying = !$scope.isPlaying;
        $scope.canBeDeleted = !$scope.canBeDeleted;
        if ($scope.isPlaying) {
            $scope.playButtonClass = 'btn btn-primary active';
            $scope.playProgressBarClass = 'progress hideable expand';
            $scope.commentAreaClass = 'form-control comment pull-right trim';
            $scope.canBeDeleted = false;
            playRecord();
        } else {
            $scope.stopButtonPressed();
        }
    }

    $scope.stopButtonPressed = function () {
        $scope.isPlaying = false;
        $scope.playButtonClass = 'btn btn-default';
        $scope.playProgressBarClass = 'progress hideable';
        $scope.commentAreaClass = 'form-control comment pull-right';
        $scope.canBeDeleted = true;
        resetProgressBars('play');
        stopPlayingRecord();
    }

    $scope.deleteButtonPressed = function () {
        deleteRecord();
        $scope.canBePlayed = false;
        $scope.recordButtonClass = 'btn btn-default';
        $scope.disableComment = true;
    }

    $scope.submitButtonPressed = function () {
        $scope.submit = !$scope.submit;
        if ($scope.submit) {
            $scope.submitButtonClass = 'btn btn-success no-margin-left no-margin-right pull-right';
            $scope.tickColor = '#fff';
            RecorderService.fillRecord(output);
            $scope.input[0] = output;
            $scope.input[0].Description = $scope.recordDescription;
            toastr.success("Record submited");
        } else {
            $scope.submitButtonClass = 'btn btn-default no-margin-left no-margin-right pull-right';
            $scope.tickColor = '#41be47';
        }
    }

    initialize();
}]);