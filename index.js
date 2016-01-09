var edge = require('edge');
var dllPath = 'mixer/CoreAudio.dll';
var mute = edge.func({
	source:require('path').join(__dirname, 'test.cs'),
	references:[dllPath],
	methodName:'setMutePID'
});

var getDevices = edge.func({
	source:require('path').join(__dirname, 'test.cs'),
	references:[dllPath],
	methodName:'getDevices'
});

var _setVolumeForPID = edge.func({
	source:require('path').join(__dirname, 'test.cs'),
	references:[dllPath],
	methodName:'setVolumeForPID'
});

module.exports = {
	getDevices:getDevices,
	mute:mute,
	setVolumeForPID: function(pid,vol,cb) {
		var opt = {
			pid:pid,
			volume:vol
		};
		_setVolumeForPID(opt,cb);
	}
}



