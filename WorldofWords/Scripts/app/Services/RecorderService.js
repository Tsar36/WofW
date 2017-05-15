app.service('RecorderService', ['ShareScopeService', function (ShareScopeService) {
    var bufferSize = 16384;
    var context;
    var node;
    var currentBuffers = [];
    var recording = false;
    var readyToPlay = false;
    var playback;
    var currentCallBack;
    var numberOfInputChannels = 2;
    var numberOfOutputChannels = 2;
    var processInputAudio = function (input) {
        if (!recording) return false;
        worker.postMessage({
            command: 'record',
            buffer: [
                input.inputBuffer.getChannelData(0),
                input.inputBuffer.getChannelData(1)
            ]
        });
    }

    var workerCode =
        'var recLength = 0;' +
        'var recBuffersL = [],' +
            'recBuffersR = [];' +

        'this.onmessage = function (e) {' +
            'switch (e.data.command) {' +
                'case "record":' +
                    'record(e.data.buffer);' +
                    'break;' +
                'case "getBuffers":' +
                    'getBuffers();' +
                    'break;' +
                'case "makePlaybackBuffers":' +
                    'makePlaybackBuffers();' +
                    'break;' +
                'case "clear":' +
                    'clear();' +
                    'break;' +
            '} ' +
        '}; ' +
        'function record(inputBuffer) {' +
            'recBuffersL.push(inputBuffer[0]);' +
            'recBuffersR.push(inputBuffer[1]);' +
            'recLength += inputBuffer[0].length;' +
        '} ' +

        'function getBuffers() {' +
            'var buffers = [];' +
            'buffers.push(mergeBuffers(recBuffersL, recLength));' +
            'buffers.push(mergeBuffers(recBuffersR, recLength));' +
            'this.postMessage(buffers);' +
        '} ' +

        'function clear() {' +
            'recLength = 0;' +
            'recBuffersL = [];' +
            'recBuffersR = [];' +
        '} ' +

        'function mergeBuffers(recBuffers, recLength) {' +
            'var result = new Float32Array(recLength);' +
            'var offset = 0;' +
            'for (var i = 0; i < recBuffers.length; i++) {' +
                'result.set(recBuffers[i], offset);' +
                'offset += recBuffers[i].length;' +
            '} ' +
            'return result;' +
        '}';

    var worker = new Worker(
            window.URL.createObjectURL(
                new Blob([workerCode], { type: 'application/javascript' })
                )
        );

    var objectToGo = {
        content: [],
        wordId: 0,
        description: ''
    };

    this.initializeSource = function (source) {
        context = source.context;
        node = source.context.createScriptProcessor(bufferSize, numberOfInputChannels, numberOfOutputChannels);
        node.onaudioprocess = processInputAudio;
        node.connect(context.destination);
        source.connect(node);
    }

    var initializePlayback = function () {
        playback = context.createBufferSource();
        playback.buffer = context.createBuffer(numberOfOutputChannels, currentBuffers[0].length, 47250);
        playback.buffer.copyToChannel(currentBuffers[0], 0);
        playback.buffer.copyToChannel(currentBuffers[1], 1);

        playback.connect(context.destination);
    }

    var makePlaybackBuffer = function () {
        currentCallBack = function (e) {
            currentBuffers = e.data;
            readyToPlay = true;
            currentCallBack = null;
        };
        worker.postMessage({
            command: 'getBuffers'
        });
    }

    this.clear = function () {
        currentCallBack = function (e) {
            currentBuffers = [];
            canGetBlayback = e;
        }
        worker.postMessage({ command: 'clear' });
    }

    this.record = function () {
        worker.postMessage({ command: 'clear' });
        recording = true;
    }

    this.stop = function () {
        recording = false;
        currentCallBack = function (e) {
            recorderBufferLength = e.data;
            currentCallBack = null;
        }
        makePlaybackBuffer();
    }

    this.play = function () {
        do {
        } while (!readyToPlay);
        initializePlayback();
        playback.start(0);
    }

    this.stopPlaying = function () {
        playback.stop();
    }

    this.getTotalTime = function () {
        return playback != null ? playback.buffer.duration : 0;
    }

    this.deleteRecord = function () {
        readyToPlay = false;
        currentBuffers = [];
        worker.postMessage({
            command: 'clear'
        });
    }

    this.fillRecord = function (record) {
        if (readyToPlay) {
            var arrayBufferToPass = _.toArray(currentBuffers[0]);
            if (arrayBufferToPass.length > 185000) {
                arrayBufferToPass.splice(185000, arrayBufferToPass.length - 185000);
            }
            record.Content = arrayBufferToPass;
        }
        else {
            return false;
        }
    };

    worker.onmessage = function (e) {
        if (currentCallBack != null) {
            currentCallBack(e);
        }
    }

    this.onClose = function () {
        if (context != undefined)
            context.close();
        if (node != null)
            node.disconnect();
        delete worker;
        delete this;
    }
    
}]);