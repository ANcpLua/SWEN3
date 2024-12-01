document.addEventListener('DOMContentLoaded', function () {
    loadDocuments();
});

async function loadDocuments() {
    try {
        const response = await fetch('/api/documents');

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
                    <a href="/api/documents/${doc.id}" class="btn btn-sm btn-primary" download>
                        Download
                    </a>
                    <button onclick="deleteDocument(${doc.id})" class="btn btn-sm btn-danger">
                        Delete
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        showAlert('Error loading documents: ' + error.message, 'danger');
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


        showAlert('Document deleted successfully');
        loadDocuments();
    } catch (error) {
        showAlert('Error deleting document: ' + error.message, 'danger');
    }
}