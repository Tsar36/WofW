app.service('TinyPlayerService', [function () {
    const CHANNELS = 2;

    var buffers = [];
    var context = new AudioContext();
    var playback;

    var clear = function () {
        buffers = [];
    };

    var initializePlayback = function () {
        playback = context.createBufferSource();
        playback.onended = clear;
        playback.buffer = context.createBuffer(CHANNELS, buffers[0].length, 44100);
        for (var i = 0; i < CHANNELS; i++) {
            playback.buffer.copyToChannel(buffers[i], i);
        }

        playback.connect(context.destination);
    }

    this.initializeRecord = function (rec) {
        buffers = rec;
    }

    this.play = function () {
        if (buffers.length > 0) {
            initializePlayback();
            playback.start(0);
        }
    }
}]);