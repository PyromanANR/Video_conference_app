const copyLinkButton = document.getElementById('copyLinkButton');

copyLinkButton.addEventListener('click', () => {
    const meetingLink = window.location.href;
    navigator.clipboard.writeText(meetingLink).then(() => {
        alert('Meeting link copied to clipboard!');
    }).catch((error) => {
        console.error('Failed to copy meeting link: ', error);
    });
});

document.getElementById("toggleChatButton").addEventListener("click", function() {
    var chatSection = document.querySelector(".chat-section");
    chatSection.classList.toggle("hidden");
});

