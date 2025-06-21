import { openSqlEditorHandler } from './eventHandlers.js';                 
import { chatInput } from './domElements.js';
import { sendMessageHandler } from './eventHandlers.js';         

const Prism = window.Prism;

export function createSqlLog(executedQueries) {
    const logContainer = document.createElement('div');
    logContainer.className = 'sql-log-container message-section';         

    const toggleBtn = document.createElement('button');
    toggleBtn.className = 'toggle-visibility-btn small-btn';         
    toggleBtn.innerHTML = `Show SQL Log <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 10l5 5 5-5z"/></svg>`;

    const content = document.createElement('div');
    content.className = 'collapsible-content sql-log-entries';         
    content.style.display = 'none';

    toggleBtn.onclick = () => {
        const isHidden = content.style.display === 'none';
        content.style.display = isHidden ? 'block' : 'none';
        toggleBtn.innerHTML = isHidden ?
            `Hide SQL Log <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 14l5-5 5 5z"/></svg>` :
            `Show SQL Log <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 10l5 5 5-5z"/></svg>`;
    };

    executedQueries.forEach((record, index) => {
        const recordDiv = document.createElement('div');
        recordDiv.className = 'sql-log-record' + (record.success ? ' success' : ' error');         

        const statusIcon = record.success ? '✅' : '❌';
        const detailsHeader = document.createElement('div');
        detailsHeader.className = 'sql-log-details-header';         
        detailsHeader.innerHTML = `<strong>Attempt ${index + 1}:</strong> ${statusIcon} Query (${record.executionTimeMs}ms)`;

        if (record.error) {
            const errorText = document.createElement('p');
            errorText.className = 'sql-log-error-text';         
            errorText.textContent = `Error: ${record.error}`;
            detailsHeader.appendChild(errorText);
        }

        const pre = document.createElement('pre');
        pre.className = 'language-sql';         
        const code = document.createElement('code');
        code.textContent = record.query;
        pre.appendChild(code);
        if (Prism) {
            Prism.highlightElement(code);
        }

        const actionsDiv = document.createElement('div');
        actionsDiv.className = 'sql-log-actions';         

        const rerunBtn = document.createElement('button');
        rerunBtn.className = 'control-button small-btn';         
        rerunBtn.textContent = 'Re-run';
        rerunBtn.onclick = () => {
            chatInput.value = record.query;
            sendMessageHandler();                                 
        };

        const editBtn = document.createElement('button');
        editBtn.className = 'control-button small-btn';         
        editBtn.textContent = 'Edit & Run';
        editBtn.onclick = () => {
            openSqlEditorHandler(record.query);
        };

        actionsDiv.appendChild(rerunBtn);
        actionsDiv.appendChild(editBtn);

        recordDiv.appendChild(detailsHeader);
        recordDiv.appendChild(pre);
        recordDiv.appendChild(actionsDiv);
        content.appendChild(recordDiv);
    });

    logContainer.appendChild(toggleBtn);
    logContainer.appendChild(content);
    return logContainer;
}
