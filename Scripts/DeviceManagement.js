

async function getConnectedDevices(type) {
    const devices = await navigator.mediaDevices.enumerateDevices();
    return devices.filter(device => device.kind === type)
}

// Open camera with at least minWidth and minHeight capabilities
async function openCamera(cameraId, minWidth, minHeight) {
    const constraints = {
        'audio': { 'echoCancellation': true },
        'video': {
            'deviceId': cameraId,
            'width': { 'min': minWidth },
            'height': { 'min': minHeight }
        }
    }

    return await navigator.mediaDevices.getUserMedia(constraints);
}

async function InitAudioStream() { //true if success
    try {
        const constraints = { 'audio': true };
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        const audioElement = document.querySelector('#localAudio');
        audioElement.srcObject = stream;
        console.log('init mic')
        return true;
    } catch (error) {
        console.error('Error opening just mic.', error);
    }
    return false;
}

async function InitVideoStream() { //true if success
    try {
        const constraints = { 'video': true };
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        const videoElement = document.querySelector('#localVideo');
        videoElement.srcObject = stream;
        console.log('init camera')
        return true;
    } catch (error) {
        console.error('Error opening video camera.', error);
    }
    return false;
}

function TryAddStream(pc) {
    const localVideo = document.querySelector('#localVideo');
    if (localVideo.srcObject !== null) {
        localVideo.srcObject.getTracks().forEach((track) => {
            pc.addTrack(track, localVideo.srcObject)
        })
    }
    if (localAudio.srcObject !== null) {
        const localAudio = document.querySelector('#localAudio');
        localAudio.srcObject.getTracks().forEach((track) => {
            pc.addTrack(track, localAudio.srcObject)
        })
    }
}


async function InitStreams(isCamOpen, isAudioOpen) { //3 - audio and video ready. 2 - just audio. 1 - just videio. 0 - nothing.

    //booleans
    const audioYES = await InitAudioStream();
    const videoYES = await InitVideoStream();

    if (videoYES) {
        if (audioYES) {
            return 3;
        }
        else
            return 1;
    }
    else {
        if (audioYES) {
            return 2;
        }
        else {
            console.log('could init nothing')
            return 0;
        }
    }
}

async function disableVideo() {
    const stream = document.querySelector('#localVideo').srcObject;
    stream.getVideoTracks()[0].enabled = false;
}

async function playVideo() {
    const stream = document.querySelector('#localVideo').srcObject;
    stream.getVideoTracks()[0].enabled = true;
}

async function disableAudio() {
    const stream = document.querySelector('#localAudio').srcObject;
    stream.getAudioTracks()[0].enabled = false;
}

async function playAudio() {
    const stream = document.querySelector('#localAudio').srcObject;
    stream.getAudioTracks()[0].enabled = true;
}

const cameras = getConnectedDevices('videoinput');
cameras.then(element => {
    if (cameras && cameras.length > 0) {
        const stream = openCamera(cameras[0].deviceId, 1280, 720);
    }
    return;
});