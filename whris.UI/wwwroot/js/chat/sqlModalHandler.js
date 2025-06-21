import { showToast } from './toastService.js';
import { chatInput } from './domElements.js';
import { sendMessageHandler } from './eventHandlers.js';             

const CodeMirror = window.CodeMirror;
let sqlEditorInstance = null;

export function setupSqlEditModal() {
    if (document.getElementById('sqlEditModal')) return;

    const modalHTML = `
        <div id="sqlEditModal" class="modal">
            <div class="modal-content">
                <div class="modal-header">
                    <h2>Edit & Run SQL Query</h2>
                    <span id="closeSqlModal" class="modal-close-btn">&times;</span>
                </div>
                <div class="modal-body">
                    <textarea id="sqlQueryEditTextArea"></textarea>
                </div>
                <div class="modal-footer">
                    <button id="cancelSqlEditButton" class="control-button cancel-sql-btn">Cancel</button>
                    <button id="runSqlButton" class="control-button run-sql-btn">Run</button>
                </div>
            </div>
        </div>
    `;
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    const modal = document.getElementById('sqlEditModal');
    const textArea = document.getElementById('sqlQueryEditTextArea');

    if (CodeMirror && typeof CodeMirror.fromTextArea === 'function') {
        sqlEditorInstance = CodeMirror.fromTextArea(textArea, {
            mode: 'text/x-mssql',
            theme: 'material-darker',                         
            lineNumbers: true,
            lineWrapping: true,
        });
    } else {
        console.error("CodeMirror is not available or fromTextArea is not a function. SQL Editor will be a plain textarea.");
    }


    const closeBtn = document.getElementById('closeSqlModal');
    const cancelBtn = document.getElementById('cancelSqlEditButton');
    const closeModal = () => { modal.style.display = 'none'; };

    closeBtn.onclick = closeModal;
    cancelBtn.onclick = closeModal;
    window.addEventListener('click', (event) => {
        if (event.target === modal) {
            closeModal();
        }
    });
}

export function openSqlEditor(query) {
    const modal = document.getElementById('sqlEditModal');
    if (!modal) {
        console.error("SQL Edit Modal not found in DOM. Call setupSqlEditModal first.");
        return;
    }
    const runBtn = document.getElementById('runSqlButton');

    modal.style.display = 'block';

    if (sqlEditorInstance) {
        sqlEditorInstance.setValue(query);
        setTimeout(() => {                     
            sqlEditorInstance.refresh();
            sqlEditorInstance.focus();
        }, 1);
    } else {
        const textArea = document.getElementById('sqlQueryEditTextArea');
        textArea.value = query;
        textArea.focus();
    }


    const newRunBtn = runBtn.cloneNode(true);
    runBtn.parentNode.replaceChild(newRunBtn, runBtn);

    newRunBtn.onclick = () => {
        const editedQuery = sqlEditorInstance ? sqlEditorInstance.getValue().trim() : document.getElementById('sqlQueryEditTextArea').value.trim();
        if (editedQuery) {
            chatInput.value = editedQuery;
            sendMessageHandler();                         
            modal.style.display = 'none';
        } else {
            showToast("Query cannot be empty.", "error");
        }
    };
}
