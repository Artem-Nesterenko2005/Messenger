async function searchInterlocutors(searchTerm) {
    if (!searchTerm || searchTerm.trim() === '') {
        clearSearchResults();
        return;
    }

    try {
        showLoadingIndicator();

        const response = await fetch(`/SearchInterlocutor?interlocutorSubName=${encodeURIComponent(searchTerm)}`);

        if (!response.ok) {
            throw new Error(`Ошибка HTTP: ${response.status}`);
        }

        const interlocutors = await response.json();

        displaySearchResults(interlocutors);

    } catch (error) {
        console.error('Ошибка при поиске:', error);
        showErrorMessage('Произошла ошибка при поиске');
    } finally {
        hideLoadingIndicator();
    }
}

function displaySearchResults(interlocutors) {
    const resultsContainer = document.getElementById('searchResultsContainer');

    if (!resultsContainer) {
        console.error('Контейнер для результатов поиска не найден');
        return;
    }

    resultsContainer.innerHTML = '';

    if (!interlocutors || interlocutors.length === 0) {
        resultsContainer.innerHTML = '<div class="no-results">Собеседники не найдены</div>';
        return;
    }

    const list = document.createElement('div');
    list.className = 'interlocutors-list';

    interlocutors.forEach(interlocutor => {
        const interlocutorElement = createInterlocutorElement(interlocutor);
        list.appendChild(interlocutorElement);
    });

    resultsContainer.appendChild(list);
}

function createInterlocutorElement(interlocutor) {
    const div = document.createElement('div');
    div.className = 'interlocutor-item';
    div.innerHTML = `
        <div class="interlocutor-info">
            <span class="interlocutor-name">${escapeHtml(interlocutor.name)}</span>
            ${interlocutor.username ?
            `<span class="interlocutor-username">@${escapeHtml(interlocutor.username)}</span>` :
            ''}
        </div>
        <button class="btn btn-primary btn-sm" onclick="openChat('${interlocutor.id}')">
            Открыть чат
        </button>
    `;
    return div;
}

function openChat(interlocutorId) {
    window.location.href = `/OpenChat?interlocutorId=${interlocutorId}`;
}

function clearSearchResults() {
    const container = document.getElementById('searchResultsContainer');
    if (container) {
        container.innerHTML = '';
    }
}

function showLoadingIndicator() {
    const container = document.getElementById('searchResultsContainer');
    if (container) {
        container.innerHTML = '<div class="loading">Поиск...</div>';
    }
}

function hideLoadingIndicator() {
}

function showErrorMessage(message) {
    const container = document.getElementById('searchResultsContainer');
    if (container) {
        container.innerHTML = `<div class="error">${message}</div>`;
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

let searchTimeout;
function debouncedSearch(input) {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        searchInterlocutors(input.value);
    }, 300);
}