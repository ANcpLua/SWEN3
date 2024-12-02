document.addEventListener('DOMContentLoaded', function() {
    loadDocuments();
    document.getElementById('deleteAllButton').addEventListener('click', deleteAllDocuments);
});

async function loadDocuments() {
    try {
        const response = await fetch('/api/documents/all');
        if (!response.ok) {
            throw new Error('Failed to fetch documents');
        }

        const documents = await response.json();
        const tbody = document.getElementById('documentsTableBody');
        tbody.innerHTML = '';

        documents.forEach(doc => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${doc.id}</td>
                <td>${doc.name}</td>
                <td>${formatDate(doc.dateUploaded)}</td>
                <td class="document-actions">
                    <a href="/api/documents/${doc.id}/content" class="btn btn-sm btn-primary" target="_blank">
                        Download
                    </a>
                    <button onclick="viewMetadata(${doc.id})" class="btn btn-sm btn-info">
                        View Metadata
                    </button>
                    <button onclick="deleteDocument(${doc.id})" class="btn btn-sm btn-danger">
                        Delete
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        showToast('Error loading documents: ' + error.message, 'danger');
    }
}

async function deleteDocument(id) {
    if (!confirm('Are you sure you want to delete this document?')) {
        return;
    }

    try {
        const response = await fetch(`/api/documents/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to delete document');
        }

        showToast('Document deleted successfully', 'success');
        loadDocuments();
    } catch (error) {
        showToast('Error deleting document: ' + error.message, 'danger');
    }
}

async function deleteAllDocuments() {
    if (!confirm('Are you sure you want to delete all documents?')) {
        return;
    }

    try {
        const response = await fetch('/api/documents', {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to delete all documents');
        }

        showToast('All documents deleted successfully', 'success');
        loadDocuments();
    } catch (error) {
        showToast('Error deleting documents: ' + error.message, 'danger');
    }
}

async function viewMetadata(id) {
    try {
        const response = await fetch(`/api/documents/${id}/metadata`);
        if (!response.ok) {
            throw new Error('Failed to fetch metadata');
        }

        const metadata = await response.json();
        document.getElementById('metadataContent').textContent = JSON.stringify(metadata, null, 2);
        new bootstrap.Modal(document.getElementById('metadataModal')).show();
    } catch (error) {
        showToast('Error loading metadata: ' + error.message, 'danger');
    }
}

function showToast(message, type = 'success') {
    const toast = document.getElementById('toast');
    const toastMessage = document.getElementById('toastMessage');
    toastMessage.textContent = message;
    toast.classList.remove('bg-success', 'bg-danger');
    toast.classList.add(type === 'success' ? 'bg-success' : 'bg-danger');

    const toastInstance = new bootstrap.Toast(toast);
    toastInstance.show();
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
}
