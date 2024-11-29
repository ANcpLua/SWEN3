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
        showAlert('Document uploaded successfully');

        // Reset form
        titleInput.value = '';
        fileInput.value = '';

        // Redirect to documents page after short delay
        setTimeout(() => {
            window.location.href = '/documents.html';
        }, 2000);
    } catch (error) {
        showAlert('Error uploading document: ' + error.message, 'danger');
    }
});

function showAlert(message, type = 'success') {
    const messageDiv = document.getElementById('message');
    messageDiv.className = `alert alert-${type}`;
    messageDiv.textContent = message;
}