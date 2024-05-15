const ROOM_ID = `@ViewBag.roomId`;
let userId = null;
let localStream = null;
const Peers = {};

const connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();

const startSignalR = async () => {
    await connection.start();
    await connection.invoke('JoinRoom', ROOM_ID, userId);
};

const myPeer = new Peer();

myPeer.on('open', id => {
    userId = id;
    startSignalR();
});

const videoGrid = document.querySelector('[video-grid]');
const myVideo = document.createElement('video');
myVideo.muted = true;

const wrapper = document.createElement('div');
wrapper.classList.add('video-wrapper');

const label = document.createElement('div');
label.classList.add('video-label');
label.textContent = "Me";

wrapper.appendChild(myVideo);
wrapper.appendChild(label);

navigator.mediaDevices.getUserMedia({
    audio: true,
    video: true
}).then(stream => {
    addVideoStream(myVideo, stream, wrapper);
    localStream = stream;
});

const toggleVideoButton = document.getElementById('toggleVideo');
toggleVideoButton.addEventListener('click', () => {
    const enabled = localStream.getVideoTracks()[0].enabled;
    localStream.getVideoTracks()[0].enabled = !enabled;
    toggleVideoButton.textContent = enabled ? 'Turn On Video' : 'Turn Off Video';
});

const toggleAudioButton = document.getElementById('toggleAudio');
toggleAudioButton.addEventListener('click', () => {
    const enabled = localStream.getAudioTracks()[0].enabled;
    localStream.getAudioTracks()[0].enabled = !enabled;
    toggleAudioButton.textContent = enabled ? 'Turn On Audio' : 'Turn Off Audio';
});

connection.on('user-connected', (id, userName) => {
    if (userId === id) return;
    const newUser = { id, name: userName };
    connectNewUser(id, localStream, userName);
    addUserToDropdown(newUser);
});

const addUserToDropdown = (user) => {
    const dropdown = document.getElementById('recipientDropdown');
    const option = document.createElement('a');
    option.classList.add('dropdown-item');
    option.href = '#';
    option.textContent = user.name;
    dropdown.appendChild(option);
};

connection.on('user-disconnected', id => {
    if (Peers[id]) {
        Peers[id].close();
        delete Peers[id];
    }
});

let isScreenSharing = false
myPeer.on('call', async call => {
    call.answer(localStream);

    const userVideo = document.createElement('video');
    const wrapper = document.createElement('div');
    wrapper.classList.add('video-wrapper');

    const label = document.createElement('div');
    label.classList.add('video-label');
    myPeer.on('connection', conn => {
        conn.on('data', (userName) => {
            console.log(userName);
            label.textContent = userName;
        });
    });
    wrapper.appendChild(userVideo);
    wrapper.appendChild(label);
    if (isScreenSharing) {
        call.on('stream', remoteStream => {
            const video = document.createElement('video');
            video.srcObject = remoteStream;
            video.autoplay = true;
            screenShareContent.appendChild(video);
            screenShareContainer.classList.remove('hidden');
        });
    }
    else {
        call.on('stream', userVideoStream => {
            addVideoStream(userVideo, userVideoStream, wrapper);
        });
    }

});


const addVideoStream = (video, stream, wrapper) => {
    video.srcObject = stream;
    video.addEventListener('loadedmetadata', () => {
        video.play();
    });
    videoGrid.appendChild(wrapper);
};


const connectNewUser = (userId, localStream, userName) => {
    connection.invoke('GetUserNameFromClient').then(MyName => {
        console.log(MyName);

        const call = myPeer.call(userId, localStream);
        const conn = myPeer.connect(userId);
        conn.on('open', () => {
            conn.send(MyName);
        });

        const userVideo = document.createElement('video');
        const wrapper = document.createElement('div');
        wrapper.classList.add('video-wrapper');

        const label = document.createElement('div');
        label.classList.add('video-label');
        label.textContent = userName;

        wrapper.appendChild(userVideo);
        wrapper.appendChild(label);

        call.on('stream', userVideoStream => {
            addVideoStream(userVideo, userVideoStream, wrapper);
        });

        call.on('close', () => {
            userVideo.remove();
        });

        Peers[userId] = call;
    });
};


const chatInput = document.getElementById('chatInput');
const sendButton = document.getElementById('sendButton');
const messagesList = document.getElementById('messages');

const sendMessage = () => {
    const message = chatInput.value;
    if (message.trim() !== '') {
        connection.invoke('SendMessage', ROOM_ID, userId, message);
        chatInput.value = '';
    }
};

chatInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
        sendMessage();
    }
});

sendButton.addEventListener('click', sendMessage);

connection.on('ReceiveMessage', (message, userName) => {
    const li = document.createElement('li');
    li.textContent = `${userName}: ${message}`;
    messagesList.appendChild(li);
});

const fileInput = document.getElementById('fileInput');

fileInput.addEventListener('change', (event) => {
    const files = event.target.files;
    for (const file of files) {
        sendFile(file);
    }
});

const sendFile = async (file) => {
    const reader = new FileReader();
    reader.onload = (event) => {
        const fileData = event.target.result;
        connection.invoke('SendFile', ROOM_ID, userId, file.name, fileData);
    };
    reader.readAsDataURL(file);
    fileInput.value = '';
};

connection.on('ReceiveFile', (fileName, fileData, userName) => {
    const li = document.createElement('li');
    const downloadLink = document.createElement('a');
    downloadLink.href = fileData;
    downloadLink.download = fileName;
    downloadLink.textContent = `Download: ${fileName}`;
    li.textContent = `${userName}: `;
    li.appendChild(downloadLink);
    messagesList.appendChild(li);
});

connection.on('ScreenSharingStatusChanged', (userId, status) => {
    isScreenSharing = status
    if (isScreenSharing === false) {
        screenShareContent.innerHTML = '';
        screenShareContainer.classList.add('hidden');
    }
});


const shareScreenButton = document.getElementById('shareScreenButton');
const screenShareContainer = document.querySelector('.screen-share-container');
const screenShareContent = document.querySelector('.screen-share-content');

let screenStream = null;

shareScreenButton.addEventListener('click', async () => {
    try {
        if (!screenStream) {
            document.getElementById('ShareScreenButtonText').textContent = 'Stop Share';
            screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
            connection.invoke('SetScreenSharingStatus', ROOM_ID, userId, true);
            Object.keys(Peers).forEach(peerId => {
                const call = myPeer.call(peerId, screenStream);
            });

            const video = document.createElement('video');
            video.srcObject = screenStream;
            video.autoplay = true;

            const screenShareContent = document.querySelector('.screen-share-content');
            screenShareContent.appendChild(video);

            screenShareContainer.classList.remove('hidden');
        } else {
            document.getElementById('ShareScreenButtonText').textContent = 'Share Screen';
            screenStream.getTracks().forEach(track => track.stop());
            screenStream = null;
            const screenShareContent = document.querySelector('.screen-share-content');
            screenShareContent.innerHTML = '';
            screenShareContainer.classList.add('hidden');
            connection.invoke('SetScreenSharingStatus', ROOM_ID, userId, false);
        }
    } catch (err) {
        console.error('Error accessing screen sharing:', err);
    }
});

let mediaRecorder;
let recordedChunks = [];

document.getElementById('recordScreenButton').addEventListener('click', async () => {
    if (!mediaRecorder) {
        const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
        mediaRecorder = new MediaRecorder(screenStream);

        mediaRecorder.ondataavailable = function (event) {
            if (event.data.size > 0) {
                recordedChunks.push(event.data);
            }
        };

        mediaRecorder.onstop = function () {
            const blob = new Blob(recordedChunks, {
                type: 'video/webm'
            });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = 'screen-record.webm';
            document.body.appendChild(a);
            a.click();
            setTimeout(() => {
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }, 100);
        };

        mediaRecorder.start();
        document.getElementById('recordScreenButtonText').textContent = 'Stop Recording';
        alert('Recording has started');
    } else {
        mediaRecorder.stop();
        mediaRecorder = null;
        recordedChunks = [];
        document.getElementById('recordScreenButtonText').textContent = 'Record Screen';
        alert('Recording has stopped and the video file has been downloaded');
    }
});
