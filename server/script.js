console.log("Hello, world!");

// TODO : add hands https://google.github.io/mediapipe/solutions/hands.html

const videoElement = document.getElementsByClassName('input_video')[0];
const canvasElement = document.getElementsByClassName('output_canvas')[0];
const canvasCtx = canvasElement.getContext('2d');
const landmarkContainer = document.getElementsByClassName('landmark-grid-container')[0];
const grid = new LandmarkGrid(landmarkContainer);

const indices = [2, 5, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22]

const socket = new WebSocket('ws://localhost:8080');
socket.binaryType = 'arraybuffer';

let clientCounter = 0;

// Send a counter message every second from the client
const clientInterval = setInterval(() => {
    clientCounter++;
    const counterMessage = { counter: clientCounter, from: 'client' };
    socket.send(MessagePack.encode(counterMessage));
}, 1000);

socket.addEventListener('open', (event) => {
    console.log('Connected to WebSocket server');
});

socket.addEventListener('message', (event) => {
    const message = MessagePack.decode(new Uint8Array(event.data));
    console.log(`Received message: ${JSON.stringify(message)}`);
    clearInterval(clientInterval)
});

socket.addEventListener('close', (event) => {
    console.log('Disconnected from WebSocket server');
});

function sendPoseLandmarks(poseLandmarks) {
    console.log(`Received landmarks: ${JSON.stringify(poseLandmarks)}`);
    let envelope = {
        type : "pose",
        content : {}
    }
    for (index of indices) {
        envelope.content[index] = poseLandmarks[index]
    }
    socket.send(MessagePack.encode(envelope));
}

function onResults(results) {
  if (!results.poseLandmarks) {
    grid.updateLandmarks([]);
    return;
  }

  sendPoseLandmarks(results.poseLandmarks);

  canvasCtx.save();
  canvasCtx.clearRect(0, 0, canvasElement.width, canvasElement.height);
  canvasCtx.drawImage(results.segmentationMask, 0, 0,
                      canvasElement.width, canvasElement.height);

  // Only overwrite existing pixels.
  canvasCtx.globalCompositeOperation = 'source-in';
  canvasCtx.fillStyle = '#00FF00';
  canvasCtx.fillRect(0, 0, canvasElement.width, canvasElement.height);

  // Only overwrite missing pixels.
  canvasCtx.globalCompositeOperation = 'destination-atop';
  canvasCtx.drawImage(
      results.image, 0, 0, canvasElement.width, canvasElement.height);

  canvasCtx.globalCompositeOperation = 'source-over';
  drawConnectors(canvasCtx, results.poseLandmarks, POSE_CONNECTIONS,
                 {color: '#00FF00', lineWidth: 4});
  drawLandmarks(canvasCtx, results.poseLandmarks,
                {color: '#FF0000', lineWidth: 2});
  canvasCtx.restore();

  grid.updateLandmarks(results.poseWorldLandmarks);
}

const pose = new Pose({locateFile: (file) => {
  return `https://cdn.jsdelivr.net/npm/@mediapipe/pose/${file}`;
}});
pose.setOptions({
  modelComplexity: 1,
  smoothLandmarks: true,
  enableSegmentation: true,
  smoothSegmentation: true,
  minDetectionConfidence: 0.5,
  minTrackingConfidence: 0.5
});
pose.onResults(onResults);

const camera = new Camera(videoElement, {
  onFrame: async () => {
    await pose.send({image: videoElement});
  },
  width: 1280,
  height: 720
});
camera.start();