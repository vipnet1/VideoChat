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

async function InitStreams() {
    try {
        const constraints = { 'video': true, 'audio': true };
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        const videoElement = document.querySelector('#localVideo');
        videoElement.srcObject = stream;
    } catch (error) {
        console.error('Error opening video camera.', error);
    }
}

async function disableVideoFromCamera() {
    const stream = document.querySelector('#localVideo').srcObject;
    stream.getVideoTracks()[0].enabled = false;
    stream.getAudioTracks()[0].enabled = false;
}

async function playVideoFromCamera() {
    const stream = document.querySelector('#localVideo').srcObject;
    stream.getVideoTracks()[0].enabled = true;
    stream.getAudioTracks()[0].enabled = true;
}

const cameras = getConnectedDevices('videoinput');
cameras.then(element => {
    if (cameras && cameras.length > 0) {
        const stream = openCamera(cameras[0].deviceId, 1280, 720);
    }
    return;
});