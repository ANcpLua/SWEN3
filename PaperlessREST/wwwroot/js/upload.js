document.getElementById('uploadForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const formData = new FormData();
    const titleInput = document.getElementById('title');
    const fileInput = document.getElementById('file');

    formData.append('title', titleInput.value);
    formData.append('file', fileInput.files[0]);

    try {
        const response = await fetch('/api/documents/upload', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            throw new Error('Upload failed');
        }

        const result = await response.json();
        showAlert('Document uploaded successfully', 'success');

        titleInput.value = '';
        fileInput.value = '';

        setTimeout(() => {
            window.location.href = '/documents.html';
        }, 2000);
    } catch (error) {
        showAlert('Error uploading document: ' + error.message, 'danger');
    }
});

function showAlert(message, type = 'success') {
    const toasty = document.getElementById('toasty');
    const toastyMessage = document.getElementById('toastyMessage');
    const toastyTitle = document.getElementById('toastyTitle');

    toastyTitle.textContent = type === 'success' ? 'Success' : 'Error';
    toastyMessage.textContent = message;

    toasty.className = `toast align-items-center text-bg-${type} border-0`;
    const toast = new bootstrap.Toast(toasty);
    toast.show();
}